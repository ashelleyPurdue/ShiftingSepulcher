using System.Collections.Generic;
using Godot;

namespace ShiftingSepulcher
{
    public class Chompweed : Node2D
    {
        [Export] public float TrackingTargetDuration = 1;
        [Export] public float LungeDuration = 0.2f;
        [Export] public float PauseAfterLungeDuration = 0.5f;
        [Export] public float RecoverDuration = 0.5f;

        [Export] public float LungeDistance = 4 * 32;

        [Export] public float StemCutDamageMult = 2;
        [Export] public float FreeHeadSpeed = 4 * 32;
        [Export] public float FreeHeadRotSpeedDeg = 360;

        private Area2D _aggroCircle => GetNode<Area2D>("%AggroCircle");
        private Area2D _freeHeadAggroCircle => GetNode<Area2D>("%FreeHeadAggroCircle");

        private HurtBox _headHurtBox => GetNode<HurtBox>("%HeadHurtBox");
        private HurtBox _stemCutHurtBox => GetNode<HurtBox>("%StemCutHurtBox");
        private HitBox _hitBox => GetNode<HitBox>("%HitBox");

        private KinematicBody2D _head => GetNode<KinematicBody2D>("%Head");
        private ChompweedHeadModel _headModel => GetNode<ChompweedHeadModel>("%Head/ChompweedHeadModel");

        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");

        private StateMachine _sm;
        private Node2D _aggroTarget;
        private Vector2 _lastAggroTargetPos;

        public Chompweed()
        {
            _sm = new StateMachine(this);
        }

        public override void _PhysicsProcess(float delta)
        {
            if (IsInstanceValid(_aggroTarget))
                _lastAggroTargetPos = _aggroTarget.GlobalPosition;
        }

        public void OnRespawning()
        {
            _sm.ChangeState(Idle);
        }

        public void OnDying()
        {
            _sm.ChangeState(Dead);
        }

        public void OnStemCutTookDamage(HitBox hitbox)
        {
            var healthPoints = this.GetComponent<HealthPointsComponent>();

            // HACK: Fix a bug that causes the chompweed to become immortal.
            // If the player manages to damage the stem AND the head in the
            // exact same frame, AND that attack happens to kill the chompweed,
            // then there's a chance that this event will run _after_ OnDead().
            // If that happens, we need to make sure we DON'T switch to the
            // decapitated state, because that would effectively "resurrect" it
            // with 0 hp.
            if (_sm.CurrentState == Dead)
                return;

            // Receive extra damage when the stem is cut
            int damage = Mathf.RoundToInt(StemCutDamageMult * hitbox.Damage);
            healthPoints.TakeDamage(damage);

            // Let the head start roaming free
            _sm.ChangeState(FreeHeadIdling);
        }

        private bool TryAcquireAggroTarget(Area2D aggroCircle)
        {
            _aggroTarget = SearchForAggroTarget();

            if (IsInstanceValid(_aggroTarget))
            {
                _lastAggroTargetPos = _aggroTarget.GlobalPosition;
                return true;
            }

            return false;

            Node2D SearchForAggroTarget()
            {
                // Prefer attacking the player
                foreach (var body in aggroCircle.GetOverlappingBodies())
                {
                    if (body is Player p)
                        return p;
                }

                // If the player isn't there, search for another enemy.
                // Don't attack other chompweeds.
                foreach (var other in aggroCircle.GetOverlappingAreas())
                {
                    if (!(other is HurtBox otherHurtBox))
                        continue;

                    if (otherHurtBox.FindAncestor<Chompweed>() == null)
                        return otherHurtBox;

                    // TODO: Ignore dead enemies, so the chompweed doesn't
                    // attack seemingly-empty space.  The current abstractions
                    // make this hard right now >.<
                }

                return null;
            }
        }

        private float AngleToTargetRad()
        {
            float raw = _head.GlobalPosition.AngleToPoint(_lastAggroTargetPos);
            return raw + Mathf.Deg2Rad(90);
        }

        private void SetStemEnabled(bool enabled)
        {
            _headModel.StemVisible = enabled;
            _stemCutHurtBox.Enabled = enabled;
        }

        private readonly IState Idle = new IdleState();
        private class IdleState : State<Chompweed>
        {
            public override void _StateEntered()
            {
                Owner._head.Position = Vector2.Zero;
                Owner._animator.ResetAndPlay("Idle");
            }

            public override void _PhysicsProcess(float delta)
            {
                if (Owner.TryAcquireAggroTarget(Owner._aggroCircle))
                {
                    ChangeState(Owner.TrackingTarget);
                }
            }
        }

        private readonly IState TrackingTarget = new TrackingTargetState();
        private class TrackingTargetState : State<Chompweed>
        {
            private float _timer;
            private float _headStartAngle;

            public override void _StateEntered()
            {
                _timer = 0;
                _headStartAngle = Owner._head.Rotation;

                Owner._animator.ResetAndPlay(
                    name: "WindingUp",
                    customSpeed: 1 / Owner.TrackingTargetDuration
                );
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer += delta;

                // Rotate towards the target
                float t = _timer / Owner.TrackingTargetDuration;

                Owner._head.Rotation = Mathf.LerpAngle(
                    _headStartAngle,
                    Owner.AngleToTargetRad(),
                    Mathf.Sqrt(t) // Rotate fast at the start, and slow at the end
                );

                // Lunge when the timer is done
                if (_timer >= Owner.TrackingTargetDuration)
                {
                    Owner._head.Rotation = Owner.AngleToTargetRad();
                    ChangeState(Owner.Lunging);
                }
            }
        }

        private readonly IState Lunging = new LungingState();
        private class LungingState : State<Chompweed>
        {
            private float _timer;
            private Vector2 _lungeDir;

            public override void _StateEntered()
            {
                _timer = 0;

                Vector2 targetPos = Owner._lastAggroTargetPos;
                _lungeDir = Owner.ToLocal(targetPos).Normalized();

                Owner._animator.ResetAndPlay(
                    name: "Lunge",
                    customSpeed: 1 / Owner.LungeDuration
                );
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer += delta;

                Vector2 startPos = Vector2.Zero;
                Vector2 endPos = _lungeDir * Owner.LungeDistance;

                float t = _timer / Owner.LungeDuration;
                var pos = startPos.LinearInterpolate(endPos, t);
                MoveToPosAndCollide(pos);

                if (_timer >= Owner.LungeDuration)
                {
                    MoveToPosAndCollide(endPos);
                    ChangeState(Owner.PausingAfterLunge);
                }
            }

            private void MoveToPosAndCollide(Vector2 localPos)
            {
                // Start the collision test from the start point, in case
                // something gets between the head and the flower
                Owner._head.Position = Vector2.Zero;
                Owner._head.ForceUpdateTransform();

                var destPosGlobal = Owner._head.GetParent<Node2D>().ToGlobal(localPos);
                var deltaPos = destPosGlobal - Owner._head.GlobalPosition;
                Owner._head.MoveAndCollide(deltaPos);
            }
        }

        private readonly IState PausingAfterLunge = new PausingAfterLungeState();
        private class PausingAfterLungeState : State<Chompweed>
        {
            private float _timer;

            public override void _StateEntered()
            {
                _timer = Owner.PauseAfterLungeDuration;
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer -= delta;

                if (_timer <= 0)
                    ChangeState(Owner.Recovering);
            }
        }

        private readonly IState Recovering = new RecoveringState();
        private class RecoveringState : State<Chompweed>
        {
            private float _timer;
            private Vector2 _startPos;

            public override void _StateEntered()
            {
                _timer = 0;
                _startPos = Owner._head.Position;

                Owner._animator.ResetAndPlay(
                    name: "Recover",
                    customSpeed: 1 / Owner.RecoverDuration
                );
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer += delta;

                Vector2 endPos = Vector2.Zero;
                float t = _timer / Owner.RecoverDuration;
                Owner._head.Position = _startPos.LinearInterpolate(endPos, t);

                if (_timer >= Owner.RecoverDuration)
                {
                    Owner._head.Position = endPos;
                    ChangeState(Owner.Idle);
                }
            }
        }

        private readonly IState FreeHeadIdling = new FreeHeadIdlingState();
        private class FreeHeadIdlingState : State<Chompweed>
        {
            public override void _StateEntered()
            {
                Owner._animator.ResetAndPlay("FreeHeadIdle");
                Owner.SetStemEnabled(false);
            }

            public override void _PhysicsProcess(float delta)
            {
                if (Owner.TryAcquireAggroTarget(Owner._freeHeadAggroCircle))
                    ChangeState(Owner.FreeHeadAggro);
            }

            public override void _StateExited()
            {
                Owner.SetStemEnabled(true);
            }
        }

        private readonly IState FreeHeadAggro = new FreeHeadAggroState();
        private class FreeHeadAggroState : State<Chompweed>
        {
            private KnockbackableVelocityComponent _headVel => Owner
                ._head
                .GetComponent<KnockbackableVelocityComponent>();

            public override void _StateEntered()
            {
                Owner._animator.ResetAndPlay("FreeHeadChase");
                Owner.SetStemEnabled(false);
            }

            public override void _PhysicsProcess(float delta)
            {
                if (!Owner.TryAcquireAggroTarget(Owner._freeHeadAggroCircle))
                {
                    ChangeState(Owner.FreeHeadIdling);
                    return;
                }

                // Rotate towards the target
                Owner._head.Rotation = AngleMath.MoveToward(
                    Owner._head.Rotation,
                    Owner.AngleToTargetRad(),
                    Mathf.Deg2Rad(Owner.FreeHeadRotSpeedDeg) * delta
                );

                // Move in the direction we're facing
                var dir = Vector2.Down.Rotated(Owner._head.Rotation);
                _headVel.WalkVelocity = dir * Owner.FreeHeadSpeed;
            }

            public override void _StateExited()
            {
                Owner.SetStemEnabled(true);
                _headVel.WalkVelocity = Vector2.Zero;
            }
        }

        private readonly IState Dead = new DeadState();
        private class DeadState : State<Chompweed>
        {
            public override void _StateEntered()
            {
                Owner._animator.ResetAndPlay("Death");
                SetEnabled(false);
            }

            public override void _StateExited()
            {
                SetEnabled(true);
            }

            private void SetEnabled(bool enabled)
            {
                Owner._headHurtBox.Enabled = enabled;
                Owner._hitBox.Enabled = enabled;

                Owner.SetStemEnabled(enabled);
            }
        }
    }
}
