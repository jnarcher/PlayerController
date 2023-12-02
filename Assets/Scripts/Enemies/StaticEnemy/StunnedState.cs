using UnityEngine;

namespace StaticEnemy
{
    public class StunnedState : StaticEnemyState
    {
        private float _stateTimer;

        public StunnedState(StaticEnemyFSM enemyController, StaticEnemyStateType type) : base(enemyController, type) { }

        public override void EnterState()
        {
            base.EnterState();
            _stateTimer = _controller.Stats.StunTime;
            _controller.SetGravity(0f);
            _controller.Animator.SetBool("Stunned", true);
        }

        public override void UpdateState()
        {
            base.UpdateState();
            _stateTimer -= Time.deltaTime;

            CheckStateTransitions();
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();
            _controller.SetVelocity(_controller.Velocity.x, 0f);
        }

        public override void ExitState()
        {
            base.ExitState();
            _controller.SetGravity(_controller.Stats.Gravity);
            _controller.Animator.SetBool("Stunned", false);
        }

        private void CheckStateTransitions()
        {
            if (_stateTimer < 0f)
                _controller.SetState(StaticEnemyStateType.Patrol);
        }
    }
}