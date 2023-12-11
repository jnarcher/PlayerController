using UnityEngine;

namespace PlayerStateMachine
{
    public class DeathState : PlayerState
    {
        public DeathState(Player player, PlayerStateType stateType) : base(player, stateType) { }

        public override void EnterState()
        {
            Player.Sounds.PlaySound(PlayerSoundType.Death);
        }
    }
}