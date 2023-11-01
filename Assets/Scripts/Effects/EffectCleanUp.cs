using UnityEngine;

public class EffectCleanUp : MonoBehaviour
{
    public float TimeUntilDestroyed = 5f;
    private void Start()
    {
        Destroy(gameObject, TimeUntilDestroyed);
    }
}
