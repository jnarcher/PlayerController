namespace PlayerStateMachine
{
    public abstract class PlayerState
    {
        protected Player Player;
        public PlayerStateType Type { get; protected set; }
        protected InputInfo InputInfo;
        protected TriggerInfo TriggerInfo;
        protected PlayerStats Stats => GameManager.Instance.PlayerStats;

        public PlayerState(Player player, PlayerStateType stateType)
        {
            Player = player;
            Type = stateType;
            InputInfo = player.GetComponent<InputInfo>();
            TriggerInfo = player.GetComponent<TriggerInfo>();
        }

        public virtual void EnterState() { }
        public virtual void ExitState() { }
        public virtual void UpdateState() { }
        public virtual void FixedUpdateState() { }
    }

}
