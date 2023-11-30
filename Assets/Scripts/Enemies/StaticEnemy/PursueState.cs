using PlayerStateMachine;
using Unity.Collections;
using UnityEngine;

namespace StaticEnemy
{
    public class PursueState : StaticEnemyState
    {
        public PursueState(StaticEnemyFSM enemyController, StaticEnemyStateType type) : base(enemyController, type) { }

        private Vector2 targetPosition;

        public override void UpdateState()
        {
            base.UpdateState();
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();
            UpdateTarget();
            MoveTowardsTarget();
        }

        private void UpdateTarget()
        {
            if (_controller.TriggerInfo.CanSeePlayer)
            {
                targetPosition = GameManager.Instance.Player.transform.position;
                _controller.TargetPosition = targetPosition;
            }
        }

        private void MoveTowardsTarget()
        {
            float xDirection = targetPosition.x < _controller.Position.x ? -1 : 1;
            _controller.SetVelocity(_controller.Stats.Speed * xDirection, _controller.Velocity.y); // ! MAKE THIS VELOCITY IN A STATS SHEET

            // TODO: REMOVE THIS LATER

            if (Mathf.Abs(_controller.Position.x - targetPosition.x) < 1f)
                _controller.SetState(StaticEnemyStateType.Patrol);
        }
    }
}