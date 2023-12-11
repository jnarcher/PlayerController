using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    [SerializeField] private PlayerSounds _sounds;
    [SerializeField] AudioSource _playerAudioSource;

    private Dictionary<PlayerSoundType, AudioClip> _clips;

    private void Awake()
    {
        _clips = new()
        {
            [PlayerSoundType.GroundJump] = _sounds.GroundJump,
            [PlayerSoundType.AirJump] = _sounds.AirJump,
            [PlayerSoundType.WallJump] = _sounds.WallJump,
            [PlayerSoundType.Attack1] = _sounds.Attack1,
            [PlayerSoundType.Attack2] = _sounds.Attack2,
            [PlayerSoundType.Dash] = _sounds.Dash,
            [PlayerSoundType.Slide] = _sounds.Slide,
            [PlayerSoundType.GrappleAim] = _sounds.GrappleAim,
            [PlayerSoundType.GrappleLaunch] = _sounds.GrappleLaunch,
            [PlayerSoundType.Hit] = _sounds.Hit,
            [PlayerSoundType.Death] = _sounds.Death,
        };
    }

    private void Update()
    {
        if (_isWalking && !_playerAudioSource.isPlaying)
        {
            _playerAudioSource.clip = _sounds.Footstep;
            _playerAudioSource.Play();
        }
        else if (!_isWalking && _playerAudioSource.isPlaying && _playerAudioSource.clip == _sounds.Footstep)
        {
            _playerAudioSource.Stop();
        }
        else if (_isWallSliding && !_playerAudioSource.isPlaying)
        {
            _playerAudioSource.clip = _sounds.WallSlide;
            _playerAudioSource.Play();
        }
        else if (!_isWallSliding && _playerAudioSource.isPlaying && _playerAudioSource.clip == _sounds.WallSlide)
        {
            _playerAudioSource.Stop();
        }
    }

    public void PlaySound(PlayerSoundType soundType)
    {
        SoundManager.Instance.PlaySound(_clips[soundType]);
    }


    private bool _isWalking;
    public void StartWalking() => _isWalking = true;
    public void StopWalking() => _isWalking = false;

    private bool _isWallSliding;
    public void StartWallSliding() => _isWallSliding = true;
    public void StopWallSliding() => _isWallSliding = false;
}
