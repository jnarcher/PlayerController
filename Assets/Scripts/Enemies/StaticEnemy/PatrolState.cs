using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaticEnemy
{
    public class PatrolState : StaticEnemyState
    {
        public PatrolState(StaticEnemyFSM enemyController, StaticEnemyStateType type) : base(enemyController, type) { }

        public override void EnterState()
        {
            base.EnterState();
            TimeEntered = _controller.ElapsedTime;
        }

        public override void UpdateState()
        {
            base.UpdateState();
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();
        }
    }
}
