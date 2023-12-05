using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject DeathPanel;
    public GameObject PausePanel;

    private GameObject _currentPanel;

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

    private float _timeDeathTransitionStart = float.MinValue;
    public void SetDeathScreen()
    {
        _currentPanel = DeathPanel;
        _currentPanel.SetActive(true);
    }
}
