using Godot;

namespace ShiftingSepulcher
{
    public class StatueSummoner : KinematicBody2D, IChallenge
    {
        [Signal] public delegate void BecameIdle();

        [Export] public PackedScene ShockwavePrefab;

        [Export] public float SummonAngleDeg = 90;
        [Export] public float MaxSummonRadius = 32 * 100;

        [Export] public float MinionSummonInterval = 5;

        [Export] public float LeapRiseDuration = 0.37f;
        [Export] public float LeapPauseDuration = 0.5f;
        [Export] public float LeapFallDuration = 0.1f;
        [Export] public float LeapRecoverDuration = 1f;

        [Export] public float HammerAimDuration = 0.5f;
        [Export] public float HammerSwingTime = 0.2f;
        [Export] public float HammerRecoveryTime = 1;
        [Export] public float HammerShockwaveStartRadius = 16;
        [Export] public float HammerShockwaveEndRadius = 48;
        [Export] public float HammerShockwaveDamage = 1;

        bool IChallenge.IsSolved() => !this.GetComponent<EnemyComponent>().IsAlive;

        public int MinionCount => _minionManager.MinionCount;

        private MinionManager _minionManager => GetNode<MinionManager>("%MinionManager");
        private RayCast2D _minionSpawnRay => GetNode<RayCast2D>("%MinionSpawnRay");

        private CollisionShape2D _bodyShape => GetNode<CollisionShape2D>("%BodyShape");
        private HurtBox _hurtBox => GetNode<HurtBox>("%HurtBox");
        private HitBox _contactHitBox => GetNode<HitBox>("%ContactHitBox");
        private Node2D _hammerShockwaveSpawn => GetNode<Node2D>("%HammerShockwaveSpawn");

        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");

        private Node2D _aggroTarget;

        private StateMachine _sm;

        public StatueSummoner()
        {
            _sm = new StateMachine(this);
        }


        public override void _EnterTree()
        {
            _aggroTarget = GetTree().FindPlayer();
        }

        public void OnRespawning()
        {
            _sm.ChangeState(Idle);
        }


        // Methods for the AI to call
        public void StartLeap() => _sm.ChangeState(LeapRising);
        public void StartHammerSwing() => _sm.ChangeState(AimingHammer);

        public void SummonMinion()
        {
            var minion = _minionManager.SummonMinion();
            minion.GlobalPosition = GetSummonPosition();
            minion.GetComponent<EnemyComponent>().DisableLootDrops = true;
        }

        // States
        private readonly IState Idle = new IdleState();
        private class IdleState : State<StatueSummoner>
        {
            public override void _StateEntered()
            {
                Owner._animator.ResetAndPlay("Idle");
                Owner.EmitSignal(nameof(BecameIdle));
            }
        }

        private readonly IState LeapRising = new LeapRisingState();
        private class LeapRisingState : PauseState<StatueSummoner>
        {
            public override float Duration => Owner.LeapRiseDuration;
            public override IState NextState => Owner.LeapPausing;

            private Vector2 _startPos;
            private float _startAngle;
            public override void _StateEntered()
            {
                base._StateEntered();

                Owner.DisableAllCollision();
                _startPos = Owner.Position;
                _startAngle = Owner.Rotation;

                Owner._animator.PlayFixedDuration("LeapRising", Owner.LeapRiseDuration);
            }

            public override void _PhysicsProcess(float delta)
            {
                base._PhysicsProcess(delta);

                var targetPos = Owner._aggroTarget.GlobalPosition - Owner.GetRoom().GlobalPosition;
                float t = Mathf.Clamp(PercentComplete, 0, 1);

                float targetAngle = Mathf.Deg2Rad(0);
                Owner.Rotation = Mathf.LerpAngle(_startAngle, targetAngle, t);
                Owner.Position = _startPos.LinearInterpolate(targetPos, t);
            }
        }

        private readonly IState LeapPausing = new LeapPausingState();
        private class LeapPausingState : PauseState<StatueSummoner>
        {
            public override float Duration => Owner.LeapPauseDuration;
            public override IState NextState => Owner.LeapFalling;
        }

        private readonly IState LeapFalling = new LeapFallingState();
        private class LeapFallingState : PauseState<StatueSummoner>
        {
            public override float Duration => Owner.LeapFallDuration;
            public override IState NextState => Owner.LeapRecovering;

            public override void _StateEntered()
            {
                base._StateEntered();
                Owner._animator.PlayFixedDuration("LeapFalling", Duration);
            }
        }

        private readonly IState LeapRecovering = new LeapRecoveringState();
        private class LeapRecoveringState : PauseState<StatueSummoner>
        {
            public override float Duration => Owner.LeapRecoverDuration;
            public override IState NextState => Owner.Idle;

            public override void _StateEntered()
            {
                base._StateEntered();
                Owner.ResetCollision();
                Owner._animator.ResetAndPlay("LeapRecovering");
            }
        }

        private readonly IState AimingHammer = new AimingHammerState();
        private class AimingHammerState : PauseState<StatueSummoner>
        {
            public override float Duration => Owner.HammerAimDuration;
            public override IState NextState => Owner.SwingingHammer;

            private float _startAngle;

            public override void _StateEntered()
            {
                base._StateEntered();
                _startAngle = Owner.Rotation;
            }

            public override void _PhysicsProcess(float delta)
            {
                base._PhysicsProcess(delta);

                float targetAngle = Owner.GlobalPosition.AngleToPoint(Owner._aggroTarget.GlobalPosition);
                targetAngle += Mathf.Deg2Rad(90);
                Owner.Rotation = Mathf.LerpAngle(_startAngle, targetAngle, PercentComplete);
            }
        }

        private readonly IState SwingingHammer = new SwingingHammerState();
        private class SwingingHammerState : PauseState<StatueSummoner>
        {
            public override float Duration => Owner.HammerSwingTime;
            public override IState NextState => Owner.RecoveringHammer;

            public override void _StateEntered()
            {
                base._StateEntered();
                Owner._animator.PlayFixedDuration("HammerSwing", Duration);
            }

            public override void _StateExited()
            {
                var shockwave = Owner.ShockwavePrefab.Instance<Shockwave>();
                shockwave.Ignore(Owner.GetComponent<HealthPointsComponent>());
                shockwave.StartRadius = Owner.HammerShockwaveStartRadius;
                shockwave.EndRadius = Owner.HammerShockwaveEndRadius;

                Owner.AddChild(shockwave);
                shockwave.GlobalPosition = Owner._hammerShockwaveSpawn.GlobalPosition;
            }
        }

        private readonly IState RecoveringHammer = new RecoveringHammerState();
        private class RecoveringHammerState : PauseState<StatueSummoner>
        {
            public override float Duration => Owner.HammerRecoveryTime;
            public override IState NextState => Owner.Idle;
        }


        // Helper methods

        private Vector2 GetSummonPosition()
        {
            Player aggroTarget = this.GetTree().FindPlayer();

            // Choose a random angle for the minion to spawn at, relative to the
            // target.
            float angleOffsetDeg = (float)GD.RandRange(
                -SummonAngleDeg / 2,
                SummonAngleDeg / 2
            );
            float angleOffsetRad = Mathf.Deg2Rad(angleOffsetDeg);

            float angleToTargetRad = aggroTarget.GlobalPosition.AngleToPoint(GlobalPosition);
            float angleRad = angleOffsetRad + angleToTargetRad;

            Vector2 summonDir = new Vector2(
                Mathf.Cos(angleRad),
                Mathf.Sin(angleRad)
            );

            // Do a raycast to ensure the minion is spawned inside the room
            _minionSpawnRay.ClearExceptions();
            _minionSpawnRay.AddException(aggroTarget);
            _minionSpawnRay.GlobalPosition = aggroTarget.GlobalPosition;
            _minionSpawnRay.CastTo = summonDir * MaxSummonRadius;
            _minionSpawnRay.ForceUpdateTransform();
            _minionSpawnRay.ForceRaycastUpdate();

            return _minionSpawnRay.GetCollisionPoint();
        }

        private void DisableAllCollision()
        {
            _bodyShape.Disabled = true;
            _hurtBox.Enabled = false;
            _contactHitBox.Enabled = false;
        }

        private void ResetCollision()
        {
            _bodyShape.Disabled = false;
            _hurtBox.Enabled = true;
            _contactHitBox.Enabled = true;
        }
    }
}
