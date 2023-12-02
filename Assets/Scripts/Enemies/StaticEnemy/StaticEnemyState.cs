using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaticEnemy
{
    public abstract class StaticEnemyState
    {
        protected StaticEnemyFSM _controller;
        protected float TimeEntered;
        public readonly StaticEnemyStateType Type;

        public StaticEnemyState(StaticEnemyFSM enemyController, StaticEnemyStateType type)
        {
            _controller = enemyController;
            Type = type;
        }

        public virtual void EnterState()
        {
            TimeEntered = _controller.ElapsedTime;
        }
        public virtual void ExitState() { }
        public virtual void UpdateState() { }
        public virtual void FixedUpdateState() { }
    }

    public enum StaticEnemyStateType
    {
        Patrol,
        Pursue,
        AirLaunched,
    }
}