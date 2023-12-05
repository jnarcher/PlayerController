using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace StaticEnemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class StaticEnemyFSM : MonoBehaviour, IEnemyController
    {
        public EnemyStats Stats;

        #region INTERFACE

        public void Kill() => Destroy(gameObject); // TODO: update later
        public EnemyStats GetStats() => Stats;
        public void Freeze() => SetState(StaticEnemyStateType.GrappleFreeze);
        public void UnFreeze() => SetState(StaticEnemyStateType.Patrol);

        public void AirLaunch(bool toRight)
        {
            AirLaunchIsRight = toRight;
            SetState(StaticEnemyStateType.AirLaunched);
        }

        public void Stun() => SetState(StaticEnemyStateType.Stunned);

        public Vector2 DirectionHitFrom { get; set; }
        public float HitStrength { get; set; }

        #endregion

        private Rigidbody2D _rb;
        public Animator Animator { get; private set; }

        public StaticEnemyState State { get; protected set; }
        private Dictionary<StaticEnemyStateType, StaticEnemyState> _stateDict;

        public float ElapsedTime { get; private set; }
        public Vector2 Velocity => _rb.velocity;
        public Vector2 Position => _rb.position;
        public bool IsFacingRight { get; private set; } = true;
        private float _gravity;

        public bool AirLaunchIsRight;

        public EnemyTriggerInfo TriggerInfo { get; private set; }

        [HideInInspector] public Vector2 TargetPosition;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();

            TriggerInfo = GetComponent<EnemyTriggerInfo>();

            _stateDict = new()
            {
                [StaticEnemyStateType.Patrol] = new PatrolState(this, StaticEnemyStateType.Patrol),
                [StaticEnemyStateType.Pursue] = new PursueState(this, StaticEnemyStateType.Pursue),
                [StaticEnemyStateType.AirLaunched] = new AirLaunchedState(this, StaticEnemyStateType.AirLaunched),
                [StaticEnemyStateType.Stunned] = new StunnedState(this, StaticEnemyStateType.Stunned),
                [StaticEnemyStateType.GrappleFreeze] = new GrappleFreezeState(this, StaticEnemyStateType.GrappleFreeze),
            };
        }

        private void Start()
        {
            _gravity = Stats.Gravity;

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
            Animator.SetFloat("Speed", Mathf.Abs(Velocity.x));
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

        private void ApplyGravity() => AddVelocity(_gravity * Time.fixedDeltaTime * Vector2.down);
        public void SetGravity(float gravity) => _gravity = gravity;

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