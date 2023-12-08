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
        private Rigidbody2D _rb;
        private Animator _anim;
        private TriggerInfo _trigs;
        private PlayerStats Stats => GameManager.Instance.PlayerStats;
        private SpriteRenderer _sprite;

        [Header("Component References")]
        public GameObject GrappleAimIndicator;

        [Header("Effects")]
        public GameObject HitEffect;
        public GameObject RunEffect;
        public GameObject LandingEffect;
        public GameObject GroundJumpEffect;
        public GameObject AirJumpEffect;
        public GameObject WallJumpEffect;
        public ParticleSystem WallSlideParticles;
        public GameObject SlideEffect;
        public GameObject DashEffect;
        public GameObject GrappleEffect;

        public Animator Animator => _anim;
        // Set by animations for attacks
        [HideInInspector] public Vector2 AnimatedVelocity;
        public bool AnimationCompleteTrigger { get; private set; }

        // State Management
        public PlayerState State { get; private set; }
        private Dictionary<PlayerStateType, PlayerState> _stateDict;

        // Physics
        public Vector2 Velocity => _rb.velocity;
        public Vector2 Position => _rb.position;
        public bool IsFacingRight { get; private set; } = true;
        public float ElapsedTime { get; private set; }
        private float _gravity;
        private float _maxFallSpeed;

        // Tracked Stats
        public List<GameObject> ActiveGrapplePoints { get; private set; }
        public GameObject SelectedGrapplePoint { get; private set; }
        public bool LastWallRight { get; private set; }
        public int AirJumpsRemaining { get; private set; }
        [HideInInspector] public Vector2 HitDirection;
        [HideInInspector] public bool IsInIFrames;

        // Cooldowns
        public bool AttackOffCooldown { get; private set; }
        public bool DashOffCooldown { get; private set; }
        public bool GrappleOffCooldown { get; private set; }


        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
            _trigs = GetComponent<TriggerInfo>();
            _sprite = GetComponent<SpriteRenderer>();
            _stateDict = new()
            {
                [PlayerStateType.Hit] = new HitState(this, PlayerStateType.Hit),
                [PlayerStateType.Move] = new MoveState(this, PlayerStateType.Move),
                [PlayerStateType.Dash] = new DashState(this, PlayerStateType.Dash),
                [PlayerStateType.GrappleAim] = new GrappleAimState(this, PlayerStateType.GrappleAim),
                [PlayerStateType.GrappleLaunch] = new GrappleLaunchState(this, PlayerStateType.GrappleLaunch),
                [PlayerStateType.GroundAttack1] = new GroundAttack1State(this, PlayerStateType.GroundAttack1),
                [PlayerStateType.GroundAttack2] = new GroundAttack2State(this, PlayerStateType.GroundAttack2),
                [PlayerStateType.AirAttack1] = new AirAttack1State(this, PlayerStateType.AirAttack1),
                [PlayerStateType.AirAttack2] = new AirAttack2State(this, PlayerStateType.AirAttack2),
                [PlayerStateType.UpAttack] = new UpAttackState(this, PlayerStateType.UpAttack),
                [PlayerStateType.DownAttack] = new DownAttackState(this, PlayerStateType.DownAttack),
            };
            ActiveGrapplePoints = new();
        }

        private void Start()
        {
            State = _stateDict[PlayerStateType.Move];
            State.EnterState();
            ResetPhysics();
        }

        private void Update()
        {
            ElapsedTime += Time.deltaTime;
            HandleCooldowns();
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
            ResetSelectedGrapplePoint();
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

        public void SetLastWallDirection(bool lastWallRight) => LastWallRight = lastWallRight;

        public void DecrementAirJump() => AirJumpsRemaining--;
        public void ResetAirJumps() => AirJumpsRemaining = GameManager.Instance.Inventory.AirJumps;

        #region COOLDOWNS

        private void HandleCooldowns()
        {
            HandleDashCooldown();
            HandleAttackCooldown();
            HandleGrappleCooldown();
        }

        private float _timeDashed = float.MinValue;
        public void ResetDash() => DashOffCooldown = true;
        public void SetDashCooldown()
        {
            _timeDashed = ElapsedTime;
            DashOffCooldown = false;
        }
        private void HandleDashCooldown()
        {
            if (!DashOffCooldown && _trigs.OnGround && ElapsedTime >= _timeDashed + Stats.GroundDashCooldown)
                ResetDash();
        }

        private float _timeAttacked = float.MinValue;
        public void ResetAttack() => AttackOffCooldown = true;
        public void SetAttackCooldown()
        {
            _timeAttacked = ElapsedTime;
            AttackOffCooldown = false;
        }
        private void HandleAttackCooldown()
        {
            if (
                _trigs.LandedThisFrame || _trigs.LeftGroundThisFrame ||
                (!AttackOffCooldown && _trigs.OnGround && ElapsedTime >= _timeAttacked + Stats.GroundAttackCooldown)
            ) ResetAttack();
        }

        private float _timeGrappled = float.MinValue;
        public void ResetGrapple() => GrappleOffCooldown = true;
        public void SetGrappleCooldown()
        {
            GrappleOffCooldown = false;
            _timeGrappled = ElapsedTime;
        }
        private void HandleGrappleCooldown()
        {
            if (ElapsedTime >= _timeGrappled + Stats.GrappleFailCooldown)
                ResetGrapple();
        }

        #endregion

        #region GRAPPLE POINT MANAGEMENT

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
        public void ResetSelectedGrapplePoint() => SelectedGrapplePoint = null;

        #endregion

        #region LERPING

        private float _lerpMovementStartTime = float.MinValue;
        private float _lerpMovementDuration;
        public float CurrentMovementLerpValue => (ElapsedTime - _lerpMovementStartTime) / _lerpMovementDuration;
        public void LerpMoveAcceleration(float duration)
        {
            _lerpMovementStartTime = ElapsedTime;
            _lerpMovementDuration = duration;
        }

        #endregion

        #region ANIMATION

        private void HandleAnimations()
        {
            _anim.SetBool("IsRunning", Mathf.Abs(Velocity.x) > 0.01f);
            _anim.SetBool("InAir", !_trigs.OnGround);
            _anim.SetBool("OnWall", GameManager.Instance.Inventory.WallSlideAndJump && _trigs.OnWall);
            _anim.SetFloat("VerticalVelocity", Velocity.y);
        }

        public void SetAnimationCompleteTrigger() => AnimationCompleteTrigger = true;
        public bool TryUseAnimationCompleteTrigger()
        {
            if (AnimationCompleteTrigger)
            {
                AnimationCompleteTrigger = false;
                return true;
            }
            return false;
        }

        #endregion

        public void Respawn()
        {
            GameObject checkpoint = CheckpointManager.Instance.GetRespawnCheckpoint();
            transform.position = checkpoint.transform.GetChild(0).position;
            SetState(PlayerStateType.Move);
            ResetPhysics();
        }

        public void Knockback(Vector2 knockback)
        {
            LerpMoveAcceleration(0.2f);
            SetVelocity(knockback);
        }

        public void Hit(Vector2 direction)
        {
            if (State.Type == PlayerStateType.Hit) return;
            HitDirection = direction;
            SetState(PlayerStateType.Hit);
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

        public void StopInvincibility()
        {
            _trigs.PlayerHurtbox.enabled = true;
            IsInIFrames = false;
        }

        private void HandleInvincibility()
        {
            if (IsInIFrames && ElapsedTime > _timeInvincibilityStop)
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
