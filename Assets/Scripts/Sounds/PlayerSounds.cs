using UnityEngine;

[CreateAssetMenu(menuName = "Player Sounds")]
public class PlayerSounds : ScriptableObject
{
    public AudioClip Footstep;
    public AudioClip GroundJump;
    public AudioClip AirJump;
    public AudioClip WallJump;
    public AudioClip GrappleAim;
    public AudioClip GrappleLaunch;
    public AudioClip WallSlide;
    public AudioClip Slide;
    public AudioClip Dash;
    public AudioClip Hit;
    public AudioClip Death;
    public AudioClip Attack1;
    public AudioClip Attack2;
}

public enum PlayerSoundType
{
    GroundJump,
    AirJump,
    WallJump,
    GrappleAim,
    GrappleLaunch,
    Slide,
    Dash,
    Hit,
    Death,
    Attack1,
    Attack2
}
