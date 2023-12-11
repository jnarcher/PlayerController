using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource _musicSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlaySound(AudioClip clip) => PlaySound(clip, (Vector2)Camera.main.transform.position);

    public void PlaySound(AudioClip clip, Vector2 position)
    {
        if (clip == null)
        {
            Debug.LogError("SoundManager (PlaySound): null audio clip");
            return;
        }

        GameObject obj = new GameObject("Sound");
        obj.transform.position = position;

        // destroy object after sound clip is done
        obj.AddComponent<EffectCleanUp>()
            .SetTime(clip.length + 0.5f)
            .SetObject(obj);

        // play sound
        obj.AddComponent<AudioSource>()
            .PlayOneShot(clip);
    }
}