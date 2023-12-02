using UnityEngine;

namespace StaticEnemy
{
    public class DamagedState : StaticEnemyState
    {
        public DamagedState(StaticEnemyFSM enemyController, StaticEnemyStateType type) : base(enemyController, type) { }
    }
}