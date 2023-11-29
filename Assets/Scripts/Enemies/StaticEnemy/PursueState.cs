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
            // Vector2 dir = (targetPosition - _controller.Position).normalized;
            // RaycastHit2D hit = Physics2D.Raycast(_controller.Position, dir, _controller.AgroRange, _controller.LineOfSightLayers);

            // if ((bool)hit)
            // targetPosition = hit.collider.transform.position;

            if (Vector2.Distance(GameManager.Instance.Player.transform.position, _controller.Position) < _controller.AgroRange)
            {
                targetPosition = GameManager.Instance.Player.transform.position;
                _controller.TargetPosition = targetPosition;
            }
        }

        private void MoveTowardsTarget()
        {
            float xDirection = targetPosition.x < _controller.Position.x ? -1 : 1;
            _controller.SetVelocity(10f * xDirection, _controller.Velocity.y); // ! MAKE THIS VELOCITY IN A STATS SHEET

            // TODO: REMOVE THIS LATER

            if (Mathf.Abs(_controller.Position.x - targetPosition.x) < 1f)
                _controller.SetState(StaticEnemyStateType.Patrol);
        }
    }
}