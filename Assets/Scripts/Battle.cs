namespace AWE.Synzza {
    public class Battle {
        public bool IsEnded { get; private set; }
        public IWorld World { get; private set; }

        public event BattleEndDelegate OnBattleEnd;

        public Battle(IWorld world) {
            IsEnded = false;
            World = world;
            World.OnExit += End;
        }

        private void End() {
            IsEnded = true;
            OnBattleEnd?.Invoke(this);
        }
    }
}
