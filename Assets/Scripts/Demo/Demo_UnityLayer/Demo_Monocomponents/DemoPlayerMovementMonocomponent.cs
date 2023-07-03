using AWE.Synzza.UnityLayer;
using UnityEngine;

namespace AWE.Synzza.Demo.UnityLayer {
    public class DemoPlayerMovementMonocomponent : MonoBehaviour {
        [SerializeField] private BattlerMonocomponent[] _enemies;

        private int _enemyIndex;

        private void Awake() {
            _enemyIndex = Random.Range(0, _enemies.Length);
        }

        public BattlerMonocomponent GetNextEnemy() {
            _enemyIndex = (_enemyIndex + 1) % _enemies.Length;

            if (_enemyIndex < _enemies.Length) {
                return _enemies[_enemyIndex];
            } else {
                Debug.LogError($"{GetType().Name} \"{gameObject.name}\" appears to have an empty array of type {typeof(BattlerMonocomponent).Name}!");
                return null;
            }
        }
    }
}
