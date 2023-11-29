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

        public LayerMask LineOfSightLayers;

        public StaticEnemyState State { get; protected set; }
        private Dictionary<StaticEnemyStateType, StaticEnemyState> _stateDict;

        public float ElapsedTime { get; private set; }
        public Vector2 Velocity => _rb.velocity;
        public Vector2 Position => _rb.position;

        public Vector2 TargetPosition = Vector2.zero;

        public float AgroRange = 10f; // ! NEEDS TO BE MOVED TO A STAT SHEET

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();

            _stateDict = new()
            {
                [StaticEnemyStateType.Patrol] = new PatrolState(this, StaticEnemyStateType.Patrol),
                [StaticEnemyStateType.Pursue] = new PursueState(this, StaticEnemyStateType.Pursue),
                // [EnemyStateType.Attack] = new AttackState(this),
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
            ApplyGravity();
        }

        public void SetState(StaticEnemyStateType stateType)
        {
            State.ExitState();
            State = _stateDict[stateType];
            State.EnterState();
        }

        public void SetVelocity(float x, float y) => _rb.velocity = new Vector2(x, y);
        public void SetVelocity(Vector2 v) => SetVelocity(v.x, v.y);

        public void AddVelocity(float x, float y) => _rb.velocity += new Vector2(x, y);
        public void AddVelocity(Vector2 v) => AddVelocity(v.x, v.y);

        private void ApplyGravity() => AddVelocity(200 * Time.fixedDeltaTime * Vector2.down);

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(Position, AgroRange);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(TargetPosition, 0.3f);
            }
        }
    }
}