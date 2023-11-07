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

        public LightAttackState(Player player) : base(player)
        {
            _playerWeapon = Player.transform.GetComponentInChildren<PlayerWeapon>();
            _sprite = _playerWeapon.gameObject.GetComponent<SpriteRenderer>();
        }

        public override void EnterState()
        {
            _startTime = Player.ElapsedTime;
            _playerWeapon.TurnOnHitbox();
            _sprite.enabled = true;
            List<EnemyHealth> hitEnemies = _playerWeapon.GetEnemiesInHitbox();

            foreach (var enemy in hitEnemies)
            {
                enemy.Damage(Stats.LightAttackDamage);
            }
        }

        public override void UpdateState()
        {
            if (Player.ElapsedTime > _startTime + 0.05f)
                Player.SetState(PlayerStateType.Move);
        }

        public override void ExitState()
        {
            _sprite.enabled = false;
            Player.SetLightAttackCooldown();
        }
    }
}
