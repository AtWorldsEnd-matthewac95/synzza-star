using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    public class UnityCoroutine : IEnumerator {
        private readonly IEnumerator<ICoWait> _coroutine;
        private object _current = null;

        public event CoroutineFinishedDelegate OnFinished;

        public UnityCoroutine(in IEnumerator<ICoWait> coroutine) => _coroutine = coroutine;
        public UnityCoroutine(in IEnumerable<ICoWait> coroutine) => _coroutine = coroutine.GetEnumerator();

        private bool ToUnityLayer(in ICoWait wait, out object converted) {
            converted = wait.WaitType switch {
                CoWaitType.WaitForSeconds => wait is CoWaitForSeconds waitForSeconds ? new WaitForSeconds(waitForSeconds.Duration) : null,
                CoWaitType.WaitUntil => wait is CoWaitUntil waitUntil ? new WaitUntil(waitUntil.Until) : null,
                CoWaitType.WaitWhile => wait is CoWaitWhile waitWhile ? new WaitWhile(waitWhile.While) : null,
                _ => null,
            };
            return converted != null;
        }

        public object Current => _current;
        public bool MoveNext() {
            var isNextValid = _coroutine.MoveNext() && ToUnityLayer(_coroutine.Current, out _current);

            if (!isNextValid) {
                OnFinished?.Invoke(this);
            }

            return isNextValid;
        }

        public void Reset() {
            _coroutine.Reset();
            _current = null;
        }

        public bool IsContaining(in IEnumerator<ICoWait> enumerator) => _coroutine == enumerator;
    }
}
