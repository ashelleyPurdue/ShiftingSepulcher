using System;
using System.Collections.Generic;
using Godot;

namespace ShiftingSepulcher
{
    public class MinionManager : Node, IOnRoomEnter
    {
        [Export] public PackedScene MinionPrefab;

        public int MinionCount => _minions.Count;

        private List<EnemyComponent> _minions = new List<EnemyComponent>();

        public void OnRoomEnter()
        {
            DeleteAllMinions();
        }

        public Node2D SummonMinion()
        {
            var entity = MinionPrefab.Instance<Node2D>();
            var enemy = entity.GetComponent<EnemyComponent>();

            if (!IsInstanceValid(enemy))
            {
                throw new Exception("Summoned minion does not have an EnemyComponent");
            }

            this.GetRoom().AddChild(entity);
            _minions.Add(enemy);

            // Clean up after the minion when it's dead
            // TODO: Wait until after its death animation finishes, rather than
            // the moment its HP hits zero
            enemy.Connect(
                nameof(EnemyComponent.Dead),
                this,
                nameof(OnMinionDead),
                new Godot.Collections.Array(enemy)
            );

            return entity;
        }

        public void KillAllMinions()
        {
            foreach (var m in _minions)
            {
                GD.PushError($"Killing {m.GetEntity().Name}");
                m.GetComponent<HealthPointsComponent>().Kill();
            }
        }

        public void DeleteAllMinions()
        {
            foreach (var m in _minions)
            {
                GD.PushError($"Queueing {m.GetEntity().Name} for deletion(DeleteAllMinions)");
                m.GetEntity().QueueFree();
            }

            GD.PushError("Clearing minion list");
            _minions.Clear();
        }

        private void OnMinionDead(EnemyComponent minion)
        {
            GD.PushError($"Removing {minion.GetEntity().Name} from list");
            _minions.Remove(minion);

            GD.PushError($"Queueing {minion.GetEntity().Name} for deletion(OnMinionDead)");
            minion.GetEntity().QueueFree();
        }
    }
}
