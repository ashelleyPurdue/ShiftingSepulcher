using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace ShiftingSepulcher
{
    public class StatueSummonerAI : BaseComponent<StatueSummoner>, IOnRoomTransitionFinished
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
            _coroutine.StopCoroutine();
        }

        public void OnRoomTransitionFinished()
        {
            _coroutine.StartCoroutine(PlayingIntro);
        }

        public void OnTookDamage()
        {
            if (!_enemy.IsAlive)
                return;

            int intendedPhase = PhaseAtHealth(_hp.Health);

            if (_currentPhase != intendedPhase)
            {
                _currentPhase = intendedPhase;
                _coroutine.StartCoroutine(LevitatingBetweenPhases);
            }
        }

        public void OnDead()
        {
            _coroutine.StopCoroutine();
        }

        private async Task PlayingIntro(CancellationToken cancel)
        {
            Entity.StartIntro();
            await ForAttackToFinish(cancel);

            _currentPhase = 0;
            _coroutine.StartCoroutine(_phases[_currentPhase]);
        }

        private async Task NoMinionsPhase(CancellationToken cancel)
        {
            while (true)
            {
                Entity.StartHammerSwing();
                await ForAttackToFinish(cancel);

                Entity.StartComboSwing();
                await ForAttackToFinish(cancel);

                Entity.StartSpinAttack();
                await ForAttackToFinish(cancel);

                for (int i = 0; i < 2; i++)
                {
                    Entity.StartLeap();
                    await ForAttackToFinish(cancel);
                }
            }
        }

        private async Task LevitatingBetweenPhases(CancellationToken cancel)
        {
            Entity.StartLevitating();
            await ForAttackToFinish(cancel);

            _coroutine.StopCoroutine();
            _coroutine.StartCoroutine(_phases[_currentPhase]);
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

                Entity.StartHammerSwing();
                await ForAttackToFinish(cancel);

                Entity.StartComboSwing();
                await ForAttackToFinish(cancel);

                Entity.StartSpinAttack();
                await ForAttackToFinish(cancel);

                for (int i = 0; i < 2; i++)
                {
                    Entity.StartLeap();
                    await ForAttackToFinish(cancel);
                }
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
