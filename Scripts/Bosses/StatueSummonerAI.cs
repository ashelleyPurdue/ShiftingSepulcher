using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace ShiftingSepulcher
{
    public class StatueSummonerAI : BaseComponent<StatueSummoner>
    {
        private EnemyComponent _enemy => this.GetComponent<EnemyComponent>();
        private HealthPointsComponent _hp => this.GetComponent<HealthPointsComponent>();

        private readonly CoroutineRunner.Coroutine[] _phases;
        private readonly CoroutineRunner _coroutine;

        private int _currentPhase = 0;

        public StatueSummonerAI()
        {
            _phases = new CoroutineRunner.Coroutine[]
            {
                NoMinionsPhase,
                MinionsAddedPhase
            };

            _coroutine = new CoroutineRunner();
            AddChild(_coroutine);
        }

        public void OnRespawning()
        {
            _currentPhase = 0;
            _coroutine.StartCoroutine(_phases[_currentPhase]);
        }

        public void OnTookDamage()
        {
            if (_enemy.IsDead)
                return;

            int intendedPhase = PhaseAtHealth(_hp.Health);

            if (_currentPhase != intendedPhase)
            {
                _currentPhase = intendedPhase;
                _coroutine.StartCoroutine(_phases[_currentPhase]);
            }
        }

        public void OnDead()
        {
            _coroutine.StopCoroutine();
        }

        private async Task NoMinionsPhase(CancellationToken cancel)
        {
            while (true)
            {
                Entity.StartLeap();
                await ForAttackToFinish(cancel);
            }
        }

        private async Task MinionsAddedPhase(CancellationToken cancel)
        {
            while (true)
            {
                while (Entity.MinionCount < 2)
                {
                    Entity.SummonMinion();
                    await _coroutine.ForPhysicsSeconds(0.5f, cancel);
                }

                Entity.StartLeap();
                await ForAttackToFinish(cancel);

                Entity.StartLeap();
                await ForAttackToFinish(cancel);
            }
        }

        private Task ForAttackToFinish(CancellationToken cancel)
        {
            return _coroutine.ForSignal(Entity, nameof(StatueSummoner.BecameIdle), cancel);
        }

        private int PhaseAtHealth(int health)
        {
            int damageTaken = _hp.MaxHealth - health;

            return (damageTaken < 3)
                ? 0
                : 1;
        }
    }
}
