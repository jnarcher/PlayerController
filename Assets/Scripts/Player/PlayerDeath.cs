using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public void KillPlayer()
    {
        GameObject checkpoint = CheckpointManager.Instance.GetRespawnCheckpoint();
        transform.position = checkpoint.transform.GetChild(0).position;
    }
}
