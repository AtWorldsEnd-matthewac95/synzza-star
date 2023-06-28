using AWE.Synzza.UnityLayer;
using AWE.Synzza.UnityLayer.Monocomponents;
using UnityEngine;

namespace AWE.Synzza.Demo.UnityLayer.Monocomponents {
    public class DemoPlayerMovementMonocomponent : MonoBehaviour {
        [SerializeField] private EnemyBattlerMonocomponent[] _enemies;

        private int _enemyIndex;

        private void Awake() {
            _enemyIndex = Random.Range(0, _enemies.Length);
        }

        public EnemyBattlerMonocomponent GetNextEnemy() {
            _enemyIndex = (_enemyIndex + 1) % _enemies.Length;

            if (_enemyIndex < _enemies.Length) {
                return _enemies[_enemyIndex];
            } else {
                Debug.LogError($"{GetType().Name} \"{gameObject.name}\" appears to have an empty array of type {typeof(EnemyBattlerMonocomponent).Name}!");
                return null;
            }
        }
    }
}
