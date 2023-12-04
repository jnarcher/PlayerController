using System.Collections.Generic;
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
        [Header("Component References")]
        private Rigidbody2D _rb;
        private Animator _anim;
        private TriggerInfo _trigs;
        private PlayerStats Stats => GameManager.Instance.PlayerStats;
        public GameObject GrappleAimIndicator;
        private SpriteRenderer _sprite;

        [Header("Effects")]
        public GameObject HitEffect;
        public GameObject RunEffect;
        public GameObject LandingEffect;
        public GameObject GroundJumpEffect;
        public GameObject AirJumpEffect;
        public GameObject WallJumpEffect;
        public GameObject WallSlideEffect;
        public GameObject SlideEffect;
        public GameObject DashEffect;
        public GameObject GrappleEffect;


        public Animator Animator => _anim;
        // Set by animations for attacks
        [HideInInspector] public Vector2 AnimatedVelocity;
        // Set by animations to signal a state change
        [HideInInspector] public bool AnimationCompleteTrigger;

        // State Management
        public PlayerState State { get; private set; }
        public PlayerStateType StateType { get; private set; }
        private Dictionary<PlayerStateType, PlayerState> _stateDict;

        // Physics
        public Vector2 Velocity => _rb.velocity;
        public Vector2 Position => _rb.position;
        public bool IsFacingRight { get; private set; } = true;
        public float ElapsedTime { get; private set; }
        private float _gravity;
        private float _maxFallSpeed;

        // Tracked Stats
        public int AirJumpsRemaining { get; private set; }
        public bool DashAvailable { get; private set; }
        public List<GameObject> ActiveGrapplePoints { get; private set; }
        public GameObject SelectedGrapplePoint { get; private set; }
        public bool AttackOffCooldown { get; private set; }
        [HideInInspector] public Vector2 HitDirection;
        [HideInInspector] public bool IsInIFrames;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
            _trigs = GetComponent<TriggerInfo>();
            _sprite = GetComponent<SpriteRenderer>();
            _stateDict = new()
            {
                [PlayerStateType.Hit] = new HitState(this),
                [PlayerStateType.Move] = new MoveState(this),
                [PlayerStateType.Dash] = new DashState(this),
                [PlayerStateType.GrappleAim] = new GrappleAimState(this),
                [PlayerStateType.GrappleLaunch] = new GrappleLaunchState(this),
                [PlayerStateType.GroundAttack1] = new GroundAttack1State(this),
                [PlayerStateType.GroundAttack2] = new GroundAttack2State(this),
                [PlayerStateType.AirAttack1] = new AirAttack1State(this),
                [PlayerStateType.AirAttack2] = new AirAttack2State(this),
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
            HandleAttackCooldown();
            HandleLerpTimeScale();
            HandleAnimations();
            HandleInvincibility();
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

        public void ResetPhysics()
        {
            SetVelocity(Vector2.zero);
            SetGravity(Stats.FallingGravity);
            ResetDash();
            ResetAirJumps();
            ResetAttack();
            SelectedGrapplePoint = null;
            HitDirection = Vector2.zero;
        }

        public void SetGravity(float gravity) => _gravity = gravity;
        private void ApplyGravity() => AddVelocity(_gravity * Time.fixedDeltaTime * Vector2.down);

        public void SetVelocity(float x, float y) => _rb.velocity = new Vector2(x, y);
        public void SetVelocity(Vector2 v) => SetVelocity(v.x, v.y);

        public void AddVelocity(float x, float y) => _rb.velocity += new Vector2(x, y);
        public void AddVelocity(Vector2 v) => AddVelocity(v.x, v.y);


        public void SetFallSpeed(float s) => _maxFallSpeed = s;
        private void ClampVelocity()
        {
            _rb.velocity = new(_rb.velocity.x, Mathf.Max(-_maxFallSpeed, _rb.velocity.y));
        }

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
            if (!DashAvailable && _trigs.OnGround && ElapsedTime >= _timeDashed + Stats.GroundDashCooldown)
                ResetDash();
        }

        public void ResetDash() => DashAvailable = true;

        private float _timeAttacked;
        public void UseAttack()
        {
            _timeAttacked = ElapsedTime;
            AttackOffCooldown = false;
        }
        public void ResetAttack() => AttackOffCooldown = true;

        private void HandleAttackCooldown()
        {
            if (_trigs.LandedThisFrame || _trigs.LeftGroundThisFrame)
                AttackOffCooldown = true;
            else if (!AttackOffCooldown && _trigs.OnGround && ElapsedTime >= _timeAttacked + Stats.GroundAttackCooldown)
                AttackOffCooldown = true;
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

        #region LERPING

        private float _lerpMovementStartTime = float.MinValue;
        private float _lerpMovementDuration;
        public float CurrentMovementLerpValue => (ElapsedTime - _lerpMovementStartTime) / _lerpMovementDuration;
        public void LerpMoveAcceleration(float duration)
        {
            _lerpMovementStartTime = ElapsedTime;
            _lerpMovementDuration = duration;
        }

        private float _lerpTimeScaleStartTime = float.MinValue;
        private float _lerpTimeScaleDuration;
        private float _lerpTimeScaleTarget = 1;
        private float _lerpTimeScaleStart = 1;
        public float CurrentTimeScaleLerpValue => (ElapsedTime - _lerpTimeScaleStartTime) / _lerpTimeScaleDuration;
        public void LerpTimeScale(float targetTimeScale, float transitionSpeed)
        {
            _lerpTimeScaleStartTime = ElapsedTime;
            _lerpTimeScaleDuration = transitionSpeed;
            _lerpTimeScaleTarget = targetTimeScale;
            _lerpTimeScaleStart = Time.timeScale;
        }

        private void HandleLerpTimeScale()
        {
            if (ElapsedTime < _lerpTimeScaleStartTime + _lerpTimeScaleDuration && Time.timeScale != _lerpTimeScaleTarget)
                Time.timeScale = Mathf.Lerp(_lerpTimeScaleStart, _lerpTimeScaleTarget, CurrentTimeScaleLerpValue);
        }

        #endregion

        #region ANIMATION

        private void HandleAnimations()
        {
            _anim.SetBool("IsRunning", Mathf.Abs(Velocity.x) > 0.01f);
            _anim.SetBool("InAir", !_trigs.OnGround);
            _anim.SetBool("OnWall", _trigs.OnWall);
            _anim.SetFloat("VerticalVelocity", Velocity.y);
        }

        #endregion

        public void Respawn()
        {
            GameObject checkpoint = CheckpointManager.Instance.GetRespawnCheckpoint();
            ResetPhysics();
            transform.position = checkpoint.transform.GetChild(0).position;
        }

        public void Knockback(Vector2 knockback)
        {
            LerpMoveAcceleration(0.2f);
            SetVelocity(knockback);
        }

        private float _timeInvincibilityStart = float.MinValue;
        private float _timeInvincibilityStop;
        public void GiveInvincibility(float time)
        {
            _trigs.PlayerHurtbox.enabled = false;
            _timeInvincibilityStart = ElapsedTime;
            _timeInvincibilityStop = _timeInvincibilityStart + time;
            IsInIFrames = true;
        }

        public void Hit(Vector2 direction)
        {
            HitDirection = direction;
            SetState(PlayerStateType.Hit);
        }

        public void StopInvincibility()
        {
            _trigs.PlayerHurtbox.enabled = true;
            IsInIFrames = false;
        }

        private void HandleInvincibility()
        {
            if (ElapsedTime > _timeInvincibilityStop)
                StopInvincibility();

            if (IsInIFrames)
            {
                _sprite.color = Color.grey;
                // TODO: Implement logic for I frame animation (flashing character or something)
            }
            else
                _sprite.color = Color.white;
        }
    }
}
