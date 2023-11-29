using System.Collections.Generic;
using UnityEngine;

namespace StaticEnemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class StaticEnemyFSM : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private Animator _anim;

        public StaticEnemyState State { get; protected set; }
        private Dictionary<StaticEnemyStateType, StaticEnemyState> _stateDict;

        public float ElapsedTime { get; private set; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();

            _stateDict = new()
            {
                [StaticEnemyStateType.Patrol] = new PatrolState(this, StaticEnemyStateType.Patrol),
                // [EnemyStateType.Searching] = new SearchingState(this),
                // [EnemyStateType.Pursuing] = new PursuingState(this),
                // [EnemyStateType.Attacking] = new AttackingState(this),
                // [EnemyStateType.Damaged] = new DamagedState(this),
                // [EnemyStateType.Death] = new DeathState(this),
            };
        }

        private void Start()
        {
            State = _stateDict[StaticEnemyStateType.Patrol];
            State.EnterState();
        }

        private void Update()
        {
            ElapsedTime += Time.deltaTime;
            State.UpdateState();
        }

        private void FixedUpdate()
        {
            State.FixedUpdateState();
        }

        public void SetVelocity(float x, float y) => _rb.velocity = new Vector2(x, y);
        public void SetVelocity(Vector2 v) => SetVelocity(v.x, v.y);

        public void AddVelocity(float x, float y) => _rb.velocity += new Vector2(x, y);
        public void AddVelocity(Vector2 v) => AddVelocity(v.x, v.y);
    }
}