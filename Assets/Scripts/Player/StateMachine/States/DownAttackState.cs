using UnityEngine;

namespace PlayerStateMachine
{
    public class DownAttackState : PlayerState
    {
        public DownAttackState(Player player) : base(player) { }

        public override void EnterState()
        {
            Debug.Log("Down Attack");
            Player.SetState(PlayerStateType.Move);
        }
    }
}