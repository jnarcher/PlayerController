using UnityEngine;

public class Grappleable : MonoBehaviour
{
    private IEnemyController _controller;

    private void Awake()
    {
        _controller = GetComponent<IEnemyController>();
    }

    public void Freeze() => _controller.Freeze();

    public void UnFreeze() => _controller.UnFreeze();
}
