using Godot;

namespace ShiftingSepulcher
{
    public class StatueSummoner : KinematicBody2D
    {
        [Export] public float MinionSummonInterval = 5;

        private MinionManager _minionManager => GetNode<MinionManager>("%MinionManager");

        private float _summonTimer = 0;


        public override void _PhysicsProcess(float delta)
        {
            bool dead = this.GetComponent<EnemyComponent>().IsDead;
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

            float spawnRadius = 100;
            float angleRad = Mathf.Deg2Rad((float)GD.RandRange(0, 360));

            minion.Position = new Vector2(
                spawnRadius * Mathf.Cos(angleRad),
                spawnRadius * Mathf.Sin(angleRad)
            );

            minion.Position += this.GetPosRelativeToAncestor(this.GetRoom());
        }
    }
}
