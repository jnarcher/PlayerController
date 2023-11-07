using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

namespace PlayerStateMachine
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(TriggerInfo))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(InputInfo))]
    public class Player : MonoBehaviour
    {
        // Component References
        private Rigidbody2D _rb;
        private Animator _anim;
        private TriggerInfo _trigs;
        private PlayerStats Stats => GameManager.Instance.PlayerStats;
        public GameObject GrappleAimIndicator;

        // State Management
        public PlayerState State { get; private set; }
        public PlayerStateType StateType;
        private Dictionary<PlayerStateType, PlayerState> _stateDict;

        // Physics
        public Vector2 Velocity => _rb.velocity;
        public bool IsFacingRight { get; private set; }
        public float ElapsedTime { get; private set; }
        private float _gravity;
        private float _maxFallSpeed;

        // Tracked Stats
        public int AirJumpsRemaining { get; private set; }
        public bool DashAvailable { get; private set; }
        public List<GameObject> ActiveGrapplePoints { get; private set; }
        public GameObject SelectedGrapplePoint { get; private set; }
        public bool CanAttack { get; private set; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
            _trigs = GetComponent<TriggerInfo>();
            _stateDict = new()
            {
                [PlayerStateType.Move] = new MoveState(this),
                [PlayerStateType.Dash] = new DashState(this),
                [PlayerStateType.GrappleAim] = new GrappleAimState(this),
                [PlayerStateType.GrappleLaunch] = new GrappleLaunchState(this),
                [PlayerStateType.LightAttack] = new LightAttackState(this),
            };
            ActiveGrapplePoints = new();
        }

        private void Start()
        {
            // Set defaults
            _gravity = Stats.RisingGravity;
            _maxFallSpeed = Stats.MaxFallSpeed;

            // Set starting state
            State = _stateDict[PlayerStateType.Move];
            State.EnterState();
        }

        private void Update()
        {
            ElapsedTime += Time.deltaTime;
            HandleDashCooldown();
            HandleLightAttackCooldown();
            State.UpdateState();
        }

        private void FixedUpdate()
        {
            State.FixedUpdateState();
            ApplyGravity();
            ClampVelocity();
        }


        public void SetState(PlayerStateType stateType)
        {
            StateType = stateType;
            State.ExitState();
            State = _stateDict[stateType];
            State.EnterState();
        }

        public void SetGravity(float gravity) => _gravity = gravity;
        private void ApplyGravity() => AddVelocity(_gravity * Time.deltaTime * Vector2.down);

        public void SetVelocity(float x, float y) => _rb.velocity = new Vector2(x, y);
        public void SetVelocity(Vector2 v) => SetVelocity(v.x, v.y);

        public void AddVelocity(float x, float y) => _rb.velocity += new Vector2(x, y);
        public void AddVelocity(Vector2 v) => AddVelocity(v.x, v.y);


        public void SetFallSpeed(float s) => _maxFallSpeed = s;
        private void ClampVelocity()
        {
            _rb.velocity = new(_rb.velocity.x, Mathf.Max(-_maxFallSpeed, _rb.velocity.y));
        }

        public void SetAnimation(string stateName)
        {
            // SetAnimationSpeed(1);
            // _anim.Play($"Base Layer.Player-{stateName}");
        }

        public void SetAnimationSpeed(float speed) => _anim.speed = speed;

        public void SetFacing(bool isFacingRight)
        {
            transform.localEulerAngles = new Vector3(
                transform.rotation.x,
                isFacingRight ? 0 : 180,
                transform.rotation.z
            );
            IsFacingRight = isFacingRight;
        }

        public void DecrementAirJump() => AirJumpsRemaining--;
        public void ResetAirJumps() => AirJumpsRemaining = Stats.AirJumpCount;


        private float _timeDashed;
        public void SetDashCooldown()
        {
            _timeDashed = ElapsedTime;
            DashAvailable = false;
        }
        private void HandleDashCooldown()
        {
            if (!DashAvailable && ElapsedTime >= _timeDashed + Stats.GroundDashCooldown)
                ResetDash();
        }

        public void ResetDash() => DashAvailable = true;

        private float _timeLightAttacked;
        public void SetLightAttackCooldown()
        {
            _timeLightAttacked = ElapsedTime;
            CanAttack = false;
        }
        private void HandleLightAttackCooldown()
        {
            if (!CanAttack && ElapsedTime >= _timeLightAttacked + Stats.LightAttackCooldown)
                CanAttack = true;
        }

        public void AddActiveGrapplePoint(GameObject grapplePoint)
        {
            if (ActiveGrapplePoints.Contains(grapplePoint)) return;
            ActiveGrapplePoints.Add(grapplePoint);
        }

        public void RemoveActiveGrapplePoint(GameObject grapplePoint)
        {
            if (!ActiveGrapplePoints.Contains(grapplePoint)) return;
            ActiveGrapplePoints.Remove(grapplePoint);
        }

        public void SetSelectedGrapplePoint(GameObject go) => SelectedGrapplePoint = go;

        private float _lerpStartTime = float.MinValue;
        private float _lerpDuration;
        public float CurrentLerpValue => (ElapsedTime - _lerpStartTime) / _lerpDuration;
        public void LerpMoveAcceleration(float duration)
        {
            _lerpStartTime = ElapsedTime;
            _lerpDuration = duration;
        }
    }
}