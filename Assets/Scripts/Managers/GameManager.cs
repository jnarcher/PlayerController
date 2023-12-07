using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerStats PlayerStats;
    public Inventory Inventory;
    public KnockbackCurves KnockbackCurves;

    public PlayerStateMachine.Player PlayerController;
    public GameObject PlayerObject;
    public PlayerHealth PlayerHealth;

    [SerializeField] private float _hitFreezeDuration;

    public bool PlayerCanMove { get; private set; } = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance);
        else
            Instance = this;
    }

    #region Player Health

    private bool _isRespawning = false;
    public void RespawnPlayer()
    {
        if (_isRespawning) return;

        UIManager.Instance.CrossFadeOut();
        FreezePlayer();
        StartCoroutine(WaitToResetPlayer());
    }

    private IEnumerator WaitToResetPlayer()
    {
        _isRespawning = true;
        yield return new WaitForSecondsRealtime(1f);
        PlayerController.Respawn();
        yield return new WaitForSecondsRealtime(0.2f);
        UIManager.Instance.CrossFadeIn();
        yield return new WaitForSecondsRealtime(0.5f);
        UnFreezePlayer();
        _isRespawning = false;
    }

    #endregion

    public void HitFreeze() => StartCoroutine(DoFreeze());
    private IEnumerator DoFreeze()
    {
        float cachedTimeScale = Time.timeScale;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(_hitFreezeDuration);
        Time.timeScale = cachedTimeScale;
    }

    private float _lerpTimeScaleDuration;
    private float _lerpTimeScaleStartTime;
    private float _lerpTimeScaleTarget = 1;
    private float _lerpTimeScaleStart = 1;
    private float _time;
    private bool _lerping;
    private IEnumerator timeScaleLerpCoroutine;

    private void Update()
    {
        _time += Time.fixedDeltaTime;
    }

    private void FixedUpdate()
    {
        HandleTimeScaleLerp();
    }

    private void HandleTimeScaleLerp()
    {
        if (!_lerping)
            return;

        if (_time > _lerpTimeScaleStartTime + _lerpTimeScaleDuration)
        {
            _lerping = false;
            Time.timeScale = _lerpTimeScaleTarget;
            return;
        }

        Time.timeScale = Mathf.Lerp(
            _lerpTimeScaleStart,
            _lerpTimeScaleTarget,
            (_time - _lerpTimeScaleStartTime) / _lerpTimeScaleDuration
        );
    }

    public void LerpTimeScale(float targetTimeScale, float transitionTime)
    {
        _lerpTimeScaleDuration = transitionTime;
        _lerpTimeScaleTarget = targetTimeScale;
        _lerpTimeScaleStart = Time.timeScale;
        _lerpTimeScaleStartTime = _time;
        _lerping = true;
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1;
        _lerping = false;
    }

    public void FreezePlayer() => PlayerCanMove = false;
    public void UnFreezePlayer() => PlayerCanMove = true;
}