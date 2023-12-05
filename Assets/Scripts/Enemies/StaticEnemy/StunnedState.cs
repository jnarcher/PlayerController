using UnityEngine;

namespace StaticEnemy
{
    public class StunnedState : StaticEnemyState
    {
        private float _stateTimer;
        private Vector2 _stunDirection;

        public StunnedState(StaticEnemyFSM enemyController, StaticEnemyStateType type) : base(enemyController, type) { }

        public override void EnterState()
        {
            base.EnterState();
            _stateTimer = _controller.Stats.StunTime;
            _stunDirection = _controller.DirectionHitFrom;
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
            float lerpVal = GameManager.Instance.KnockbackCurves.Basic.Evaluate(1 - _stateTimer / _controller.Stats.StunTime);
            float newXSpeed = _controller.HitStrength * _controller.Stats.KnockbackWeight * lerpVal;
            float newYSpeed = _controller.HitStrength * _controller.Stats.KnockbackWeight * lerpVal;
            _controller.SetVelocity(newXSpeed * _stunDirection.x, newYSpeed * _stunDirection.y);
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