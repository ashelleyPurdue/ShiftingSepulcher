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
        [Export] public float FreeHeadIdleDuration = 0.5f;
        [Export] public float FreeHeadLungeDuration = 0.2f;
        [Export] public float FreeHeadLungeDistance = 3 * 32;

        private Area2D _aggroCircle => GetNode<Area2D>("%AggroCircle");
        private Area2D _freeHeadAggroCircle => GetNode<Area2D>("%FreeHeadAggroCircle");
        private Area2D _headHurtBox => GetNode<Area2D>("%HeadHurtBox");
        private Area2D _stemCutHurtBox => GetNode<Area2D>("%StemCutHurtBox");
        private Area2D _hitBox => GetNode<Area2D>("%HitBox");
        private Node2D _head => GetNode<Node2D>("%Head");

        private StateMachine _sm;
        private Node2D _aggroTarget;

        public override void _Ready()
        {
            _sm = new StateMachine(this);
        }

        public void OnRespawning()
        {
            _sm.ChangeState(Idle);
        }

        public void OnDead()
        {
            _sm.ChangeState(Dead);
        }

        public void OnStemCutTookDamage(HitBox hitbox)
        {
            var healthPoints = this.GetComponent<HealthPointsComponent>();

            // Receive extra damage when the stem is cut
            int damage = Mathf.RoundToInt(StemCutDamageMult * hitbox.Damage);
            healthPoints.TakeDamage(damage);

            // Let the head start roaming free
            _sm.ChangeState(FreeHeadIdling);
        }

        private Node2D SearchForAggroTarget(Area2D aggroCircle)
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
            }

            return null;
        }

        private readonly IState Idle = new IdleState();
        private class IdleState : State<Chompweed>
        {
            public override void _StateEntered()
            {
                Owner._head.Position = Vector2.Zero;
            }

            public override void _PhysicsProcess(float delta)
            {
                Owner._aggroTarget = Owner.SearchForAggroTarget(Owner._aggroCircle);

                if (IsInstanceValid(Owner._aggroTarget))
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
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer += delta;

                // Rotate towards the target
                float t = _timer / Owner.TrackingTargetDuration;

                Owner._head.Rotation = Mathf.LerpAngle(
                    _headStartAngle,
                    AngleToTargetRad(),
                    Mathf.Sqrt(t) // Rotate fast at the start, and slow at the end
                );

                // Lunge when the timer is done
                if (_timer >= Owner.TrackingTargetDuration)
                {
                    Owner._head.Rotation = AngleToTargetRad();
                    ChangeState(Owner.Lunging);
                }
            }

            private float AngleToTargetRad()
            {
                float raw = Owner._head.GlobalPosition.AngleToPoint(
                    Owner._aggroTarget.GlobalPosition
                );

                return raw + Mathf.Deg2Rad(90);
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

                Vector2 targetPos = Owner._aggroTarget.GlobalPosition;
                _lungeDir = Owner.ToLocal(targetPos).Normalized();
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer += delta;

                Vector2 startPos = Vector2.Zero;
                Vector2 endPos = _lungeDir * Owner.LungeDistance;

                float t = _timer / Owner.LungeDuration;
                Owner._head.Position = startPos.LinearInterpolate(endPos, t);

                if (_timer >= Owner.LungeDuration)
                {
                    Owner._head.Position = endPos;
                    ChangeState(Owner.PausingAfterLunge);
                }
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
            private float _timer;

            public override void _StateEntered()
            {
                _timer = Owner.FreeHeadIdleDuration;
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer -= delta;

                if (_timer <= 0)
                    ChangeState(Owner.FreeHeadLunging);
            }
        }

        private readonly IState FreeHeadLunging = new FreeHeadLungingState();
        private class FreeHeadLungingState : State<Chompweed>
        {
            private float _timer;
            private Vector2 _velocity;

            public override void _StateEntered()
            {
                _timer = Owner.FreeHeadLungeDuration;
                _velocity = ChooseLungeVelocity();
                Owner._head.Rotation = _velocity.Angle() - Mathf.Deg2Rad(90);
            }

            public override void _PhysicsProcess(float delta)
            {
                Owner._head.Position += _velocity * delta;
                _timer -= delta;

                if (_timer <= 0)
                    ChangeState(Owner.FreeHeadIdling);
            }

            private Vector2 ChooseLungeVelocity()
            {
                float speed = Owner.FreeHeadLungeDistance / Owner.FreeHeadLungeDuration;

                // Find a target to lunge at
                var aggroTarget = Owner.SearchForAggroTarget(Owner._freeHeadAggroCircle);

                // Choose a random direction if there is nobody around to lunge
                // at
                if (!IsInstanceValid(aggroTarget))
                {
                    float angle = GD.Randf() * Mathf.Deg2Rad(360);
                    return new Vector2(
                        speed * Mathf.Cos(angle),
                        speed * Mathf.Sin(angle)
                    );
                }

                // Move directly at the target if there is one
                var dir = aggroTarget.GlobalPosition - Owner._head.GlobalPosition;
                dir = dir.Normalized();

                return speed * dir;
            }
        }

        private readonly IState Dead = new DeadState();
        private class DeadState : State<Chompweed>
        {
            public override void _StateEntered()
            {
                // TODO: Play a death animation
                SetEnabled(false);
            }

            public override void _StateExited()
            {
                SetEnabled(true);
            }

            private void SetEnabled(bool enabled)
            {
                Owner._headHurtBox.Monitoring = enabled;
                Owner._headHurtBox.Monitorable = enabled;

                Owner._stemCutHurtBox.Monitoring = enabled;
                Owner._stemCutHurtBox.Monitorable = enabled;

                Owner._hitBox.Monitoring = enabled;
                Owner._hitBox.Monitorable = enabled;

                Owner._head.Visible = enabled;
            }
        }
    }
}
