using Godot;

namespace ShiftingSepulcher
{
    public class StatueSummoner : KinematicBody2D, IChallenge
    {
        [Export] public float SummonAngleDeg = 90;
        [Export] public float MaxSummonRadius = 32 * 100;

        [Export] public float MinionSummonInterval = 5;

        private MinionManager _minionManager => GetNode<MinionManager>("%MinionManager");
        private RayCast2D _minionSpawnRay => GetNode<RayCast2D>("%MinionSpawnRay");

        private float _summonTimer = 0;

        bool IChallenge.IsSolved() => !this.GetComponent<EnemyComponent>().IsAlive;

        public override void _PhysicsProcess(float delta)
        {
            bool dead = !this.GetComponent<EnemyComponent>().IsAlive;
            Visible = !dead;

            if (dead)
                return;

            _summonTimer -= delta;

            if (_summonTimer <= 0)
            {
                _summonTimer = MinionSummonInterval;
                SummonMinion();
            }
        }

        private void SummonMinion()
        {
            var minion = _minionManager.SummonMinion();
            minion.GlobalPosition = GetSummonPosition();
        }

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
    }
}
