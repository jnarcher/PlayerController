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
                100 * Time.fixedDeltaTime
            );
            _controller.SetVelocity(newXVel, _controller.Velocity.y);
        }


        private void CheckForPlayer()
        {
            Vector2 playerPos = GameManager.Instance.Player.transform.position;
            float distToPlayer = Vector2.Distance(playerPos, _controller.Position);

            if (distToPlayer <= _controller.AgroRange)
                _controller.SetState(StaticEnemyStateType.Pursue);
        }
    }
}
