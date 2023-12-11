using UnityEngine;

[CreateAssetMenu(menuName = "Player Sounds")]
public class PlayerSounds : ScriptableObject
{
    public AudioClip Move;
    public AudioClip GroundJump;
    public AudioClip AirJump;
    public AudioClip WallSlide;
    public AudioClip Slide;
    public AudioClip Dash;
}
