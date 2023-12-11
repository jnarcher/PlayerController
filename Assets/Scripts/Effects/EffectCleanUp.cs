using UnityEngine;

public class EffectCleanUp : MonoBehaviour
{
    [SerializeField] private float _timeToDestroy = 5f;
    [SerializeField] private GameObject _objectToDestroy;

    private void Start() =>
        Destroy(_objectToDestroy != null ? _objectToDestroy : gameObject, _timeToDestroy);

    public EffectCleanUp SetTime(float time)
    {
        _timeToDestroy = time;
        return this;
    }

    public EffectCleanUp SetObject(GameObject obj)
    {
        _objectToDestroy = obj;
        return this;
    }
}
