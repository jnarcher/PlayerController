using UnityEngine;

namespace StaticEnemy
{
    public class PatrolState : StaticEnemyState
    {
        public PatrolState(StaticEnemyFSM enemyController, StaticEnemyStateType type) : base(enemyController, type) { }

        public override void EnterState()
        {
            base.EnterState();
        }

        public override void UpdateState()
        {
            base.UpdateState();
            CheckForPlayer();
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();
            float newXVel = Mathf.MoveTowards(
                _controller.Velocity.x,
                0, // this enemy stands still in patrol state
                _controller.Stats.Acceleration * Time.fixedDeltaTime
            );
            _controller.SetVelocity(newXVel, _controller.Velocity.y);
        }


        private void CheckForPlayer()
        {
            bool targetIsTowardsRight = GameManager.Instance.PlayerController.transform.position.x - _controller.Position.x > 0;
            if (_controller.TriggerInfo.CanSeePlayer)
                if (!_controller.TriggerInfo.IsNearLedge || _controller.IsFacingRight != targetIsTowardsRight)
                    _controller.SetState(StaticEnemyStateType.Pursue);
        }
    }
}
