using UnityEngine;

namespace StaticEnemy
{
    public class AirLaunchedState : StaticEnemyState
    {
        private float _stateTimer;

        public AirLaunchedState(StaticEnemyFSM enemyController, StaticEnemyStateType type) : base(enemyController, type) { }

        public override void EnterState()
        {
            base.EnterState();
            _stateTimer = _controller.Stats.AirLaunchedStateLength;
        }

        public override void UpdateState()
        {
            base.UpdateState();
            _stateTimer -= Time.deltaTime;

            CheckStateTransition();
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();
            _controller.SetVelocity(
                new Vector2(
                    _controller.Stats.AirLaunchCurveStrengthX * (_controller.AirLaunchIsRight ? 1 : -1) * _controller.Stats.AirLaunchVelocityX.Evaluate(1 - _stateTimer / _controller.Stats.AirLaunchedStateLength),
                    _controller.Stats.AirLaunchCurveStrengthY * _controller.Stats.AirLaunchVelocityY.Evaluate(1 - _stateTimer / _controller.Stats.AirLaunchedStateLength)
                )
            );
        }

        private void CheckStateTransition()
        {
            if (_stateTimer < 0f)
                _controller.SetState(StaticEnemyStateType.Patrol);
        }
    }
}
