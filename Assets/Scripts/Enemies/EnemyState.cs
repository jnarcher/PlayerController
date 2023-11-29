namespace EnemyStateMachine
{
    public abstract class EnemyState
    {
        protected IEnemy _controller;
        public readonly EnemyStateType Type;

        public EnemyState(IEnemy enemyController, EnemyStateType type)
        {
            _controller = enemyController;
            Type = type;
        }

        public virtual void EnterState() { }
        public virtual void ExitState() { }
        public virtual void UpdateState() { }
        public virtual void FixedUpdateState() { }
    }

    public enum EnemyStateType
    {
        Searching,
        Patrolling,
        Pursuing,
        Attacking,
        Damaged,
        Death,
    }
}