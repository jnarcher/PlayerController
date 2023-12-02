using UnityEngine;

namespace StaticEnemy
{
    public class GrappleFreezeState : StaticEnemyState
    {
        public GrappleFreezeState(StaticEnemyFSM enemyController, StaticEnemyStateType type) : base(enemyController, type) { }

        public override void EnterState()
        {
            base.EnterState();
            _controller.SetGravity(0f);
            _controller.SetVelocity(Vector2.zero);
        }

        public override void ExitState()
        {
            base.ExitState();
            _controller.SetGravity(_controller.Stats.Gravity);
        }
    }
}
