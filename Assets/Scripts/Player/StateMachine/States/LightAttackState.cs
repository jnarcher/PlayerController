using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayerStateMachine
{
    public class LightAttackState : PlayerState
    {
        private float _startTime;
        private PlayerWeapon _playerWeapon;
        private SpriteRenderer _sprite;

        private float _attackTime;

        public LightAttackState(Player player) : base(player)
        {
            _playerWeapon = Player.transform.GetComponentInChildren<PlayerWeapon>();
            _sprite = _playerWeapon.gameObject.GetComponent<SpriteRenderer>();
        }

        public override void EnterState()
        {
            _startTime = Player.ElapsedTime;
            _playerWeapon.TurnOnHitbox();
            // _sprite.enabled = true;
            List<EnemyHealth> hitEnemies = _playerWeapon.GetEnemiesInHitbox();

            foreach (var enemy in hitEnemies)
            {
                enemy.Damage(Stats.LightAttackDamage);
            }

            Player.Animator.SetTrigger("Attack1");

            AnimationClip[] clips = Player.Animator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
            {
                if (clip.name == "Player-Attack")
                    _attackTime = clip.length;
            }

            if (!TriggerInfo.OnGround)
            {
                Player.SetGravity(0);
                Player.SetVelocity(Player.Velocity.x, 0);
            }
            else Player.SetVelocity(0, 0);
        }

        public override void UpdateState()
        {
            if (Player.ElapsedTime > _startTime + _attackTime)
                Player.SetState(PlayerStateType.Move);
        }

        public override void ExitState()
        {
            // _sprite.enabled = false;
            Player.SetLightAttackCooldown();
            Player.SetGravity(GameManager.Instance.PlayerStats.RisingGravity);
        }
    }
}
