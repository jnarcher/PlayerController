using System.Runtime.ExceptionServices;
using PlayerStateMachine;
using Unity.Collections;
using UnityEngine;

namespace StaticEnemy
{
    public class PursueState : StaticEnemyState
    {
        public PursueState(StaticEnemyFSM enemyController, StaticEnemyStateType type) : base(enemyController, type) { }

        private Vector2 targetPosition;

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();
            UpdateTarget();
            MoveTowardsTarget();
            HandleTurn();
            HandleStateChange();
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
            // Check if there is a ledge in front of enemy
            bool targetIsTowardsRight = targetPosition.x - _controller.Position.x > 0;
            if (_controller.TriggerInfo.IsNearLedge && targetIsTowardsRight == _controller.IsFacingRight)
            {
                _controller.SetState(StaticEnemyStateType.Patrol);
                return;
            }

            float xDirection = targetPosition.x < _controller.Position.x ? -1 : 1;
            float newXVel = Mathf.MoveTowards(
                _controller.Velocity.x,
                xDirection * _controller.Stats.Speed,
                _controller.Stats.Acceleration * Time.fixedDeltaTime
            );
            _controller.SetVelocity(newXVel, _controller.Velocity.y);
        }

        private void HandleTurn()
        {
            if (_controller.IsFacingRight && _controller.Velocity.x < 0)
                _controller.SetFacing(false);
            else if (!_controller.IsFacingRight && _controller.Velocity.x > 0)
                _controller.SetFacing(true);
        }

        private void HandleStateChange()
        {
            if (Mathf.Abs(_controller.Position.x - targetPosition.x) < 1f)
                _controller.SetState(StaticEnemyStateType.Patrol);
        }
    }
}