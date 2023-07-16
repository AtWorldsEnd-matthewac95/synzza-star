using System;

namespace AWE.Synzza {
    public sealed class BattleCoordinator {
        private const uint MAX_BATTLE_COUNT = 1;

        private readonly Battle[] _coordinatedBattles = new Battle[MAX_BATTLE_COUNT];
        private int _emptyBattleIndex = 0;

        public event BattleEndDelegate OnBattleEnd;

        private void OnCoordinatedBattleEnd(Battle battle) {
            for (int i = 0; i < _coordinatedBattles.Length; ++i) {
                if (battle == _coordinatedBattles[i]) {
                    _coordinatedBattles[i] = null;
                    _emptyBattleIndex = Math.Min(i, _emptyBattleIndex);
                }
            }

            OnBattleEnd?.Invoke(battle);
        }

        public void AddBattle(Battle battle) {
            _coordinatedBattles[_emptyBattleIndex++] = battle;

            while (_emptyBattleIndex < _coordinatedBattles.Length && _coordinatedBattles[_emptyBattleIndex] != null) {
                ++_emptyBattleIndex;
            }

            battle.OnBattleEnd += OnCoordinatedBattleEnd;
        }
    }
}
