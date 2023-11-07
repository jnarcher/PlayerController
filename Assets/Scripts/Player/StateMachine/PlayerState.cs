namespace PlayerStateMachine
{
    public abstract class PlayerState
    {
        protected Player Player;
        protected InputInfo InputInfo;
        protected TriggerInfo TriggerInfo;
        protected PlayerStats Stats => GameManager.Instance.PlayerStats;

        public PlayerState(Player player)
        {
            Player = player;
            InputInfo = player.GetComponent<InputInfo>();
            TriggerInfo = player.GetComponent<TriggerInfo>();
        }

        public virtual void EnterState() { }
        public virtual void ExitState() { }
        public virtual void UpdateState() { }
        public virtual void FixedUpdateState() { }
    }

}
