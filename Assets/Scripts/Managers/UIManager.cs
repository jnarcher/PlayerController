using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject DeathPanel;
    public GameObject PausePanel;

    private GameObject _currentPanel;

    public Animator CrossFadeAnimator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance);
        else
            Instance = this;
    }

    private float _time;

    private void Update()
    {
        _time += Time.deltaTime;
    }

    public void CrossFadeOut()
    {
        CrossFadeAnimator.ResetTrigger("FadeIn");
        CrossFadeAnimator.SetTrigger("FadeOut");
    }
    public void CrossFadeIn()
    {
        CrossFadeAnimator.ResetTrigger("FadeOut");
        CrossFadeAnimator.SetTrigger("FadeIn");
    }
}
