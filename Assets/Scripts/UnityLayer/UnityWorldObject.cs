using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    public class UnityWorldObject : IWorldObject {
        public Transform transform { get; }
        public bool IsMobile { get; set; }

        protected MonoBehaviour _mono;
        public MonoBehaviour Mono => _mono;

        private List<UnityCoroutine> _activeCoroutines = new();

        public UnityWorldObject(Transform T, bool isMobile = true) {
            transform = T;
            IsMobile = isMobile;

            if (!transform.gameObject.TryGetComponent(out _mono)) {
                _mono = null;
            }
        }
        public UnityWorldObject(MonoBehaviour mono, bool isMobile = true) {
            _mono = mono;
            transform = _mono.transform;
            IsMobile = isMobile;
        }

        public bool TrySetMono(MonoBehaviour mono) {
            if (_mono != null) {
                return mono == _mono;
            }

            var success = mono.transform == transform;

            if (success) {
                _mono = mono;
            }

            return success;
        }

        public float3 LocalPosition => transform.localPosition.ToFloat3();
        public float3 WorldPosition => transform.position.ToFloat3();
        public float3 LocalRotation => transform.localEulerAngles.ToFloat3();
        public float3 WorldRotation => transform.eulerAngles.ToFloat3();
        public float4 LocalQuaternion => transform.localRotation.ToFloat4();
        public float4 WorldQuaternion => transform.rotation.ToFloat4();
        public float3 LocalScale => transform.localScale.ToFloat3();
        public float3 WorldScale => transform.lossyScale.ToFloat3();
        public float3 WorldForward => transform.forward.ToFloat3();
        public float3 WorldRight => transform.right.ToFloat3();
        public float3 WorldUp => transform.up.ToFloat3();

        private void AddToActiveCoroutines(UnityCoroutine coroutine) {
            coroutine.OnFinished += RemoveFromActiveCoroutines;

            bool isNotAdded = true;
            for (var i = 0; i < _activeCoroutines.Count; ++i) {
                if (_activeCoroutines[i] == null) {
                    _activeCoroutines[i] = coroutine;
                    isNotAdded = false;
                }
            }

            if (isNotAdded) {
                _activeCoroutines.Add(coroutine);
            }
        }

        private void RemoveFromActiveCoroutines(in IEnumerator enumerator) {
            if (enumerator is UnityCoroutine unityCoroutine) {
                for (var i = 0; i < _activeCoroutines.Count; ++i) {
                    if (_activeCoroutines[i] == unityCoroutine) {
                        _activeCoroutines[i] = null;
                    }
                }
            }
        }

        public void StartCoroutine(in IEnumerator<ICoWait> coroutine) {
            if (_mono != null) {
                var activeCoroutine = new UnityCoroutine(coroutine);
                AddToActiveCoroutines(activeCoroutine);
                _mono.StartCoroutine(activeCoroutine);
            }
        }

        public void StartCoroutine(in IEnumerable<ICoWait> coroutine) {
            if (_mono != null) {
                _mono.StartCoroutine(new UnityCoroutine(coroutine));
            }
        }

        public void StopCoroutine(in IEnumerator<ICoWait> coroutine) {
            if (_mono != null) {
                for (var i = 0; i < _activeCoroutines.Count; ++i) {
                    var activeCoroutine = _activeCoroutines[i];
                    if (activeCoroutine?.IsContaining(coroutine) ?? false) {
                        _mono.StopCoroutine(activeCoroutine);
                        _activeCoroutines[i] = null;
                    }
                }
            }
        }

        public void StopAllCoroutines() {
            if (_mono != null) {
                _mono.StopAllCoroutines();
            }
        }

        public event SceneObjectPreDestroyDelegate OnPreDestroy;

        public void Destroy() {
            OnPreDestroy?.Invoke(this);
            UnityEngine.Object.Destroy(transform.gameObject);
        }
    }
}
