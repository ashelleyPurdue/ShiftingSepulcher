using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace ShiftingSepulcher
{
    public class StatueSummonerAI : BaseComponent<StatueSummoner>
    {
        private readonly CoroutineRunner _coroutine = new CoroutineRunner();

        public StatueSummonerAI()
        {
            AddChild(_coroutine);
        }

        public override void _EntityReady()
        {
            base._EntityReady();
        }

        public void OnRespawning()
        {
            _coroutine.StartCoroutine(AttackLoop);
        }

        private async Task AttackLoop(CancellationToken cancel)
        {
            while (true)
            {
                Entity.StartLeap();
                await ForAttackToFinish(cancel);
            }
        }

        private Task ForAttackToFinish(CancellationToken cancel)
        {
            return _coroutine.ForSignal(Entity, nameof(StatueSummoner.BecameIdle), cancel);
        }
    }
}
