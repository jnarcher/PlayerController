using System.Collections.Generic;
using UnityEngine;

namespace StaticEnemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class StaticEnemyFSM : MonoBehaviour
    {

        public EnemyStats Stats;

        private Rigidbody2D _rb;
        private Animator _anim;

        public StaticEnemyState State { get; protected set; }
        private Dictionary<StaticEnemyStateType, StaticEnemyState> _stateDict;

        public float ElapsedTime { get; private set; }
        public Vector2 Velocity => _rb.velocity;
        public Vector2 Position => _rb.position;
        public bool IsFacingRight { get; private set; } = true;

        public EnemyTriggerInfo TriggerInfo { get; private set; }

        [HideInInspector] public Vector2 TargetPosition;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();

            TriggerInfo = GetComponent<EnemyTriggerInfo>();

            _stateDict = new()
            {
                [StaticEnemyStateType.Patrol] = new PatrolState(this, StaticEnemyStateType.Patrol),
                [StaticEnemyStateType.Pursue] = new PursueState(this, StaticEnemyStateType.Pursue),
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

        private void ApplyGravity() => AddVelocity(Stats.Gravity * Time.fixedDeltaTime * Vector2.down);

        public void SetFacing(bool isFacingRight)
        {
            transform.localEulerAngles = new Vector3(
                transform.rotation.x,
                isFacingRight ? 0 : 180,
                transform.rotation.z
            );
            IsFacingRight = isFacingRight;
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(TargetPosition, 0.3f);
            }
        }
    }
}