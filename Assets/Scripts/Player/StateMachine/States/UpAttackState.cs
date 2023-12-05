using UnityEngine;

namespace PlayerStateMachine
{
    public class UpAttackState : PlayerState
    {
        public UpAttackState(Player player) : base(player) { }

        public override void EnterState()
        {
            Debug.Log("UpAttack");
            Player.SetState(PlayerStateType.Move);
        }
    }
}
