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

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();
            // Debug.Log(_controller.Velocity.x);
            float newXVel = Mathf.MoveTowards(
                _controller.Velocity.x,
                0, // this enemy stands still in patrol state
                100 * Time.fixedDeltaTime
            );
            _controller.SetVelocity(newXVel, _controller.Velocity.y);
        }
    }
}
