using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AWE.Synzza.UnityLayer {
    public interface IUnityWorldObject : IWorldObject {
        Transform transform { get; }
    }

    public class UnityWorldObject : WorldObject.Impl, IUnityWorldObject {
        public Transform transform { get; }

        private bool _isMobile;

        public override bool IsMobile => _isMobile;
        public void SetIsMobile(bool value) => _isMobile = value;

        protected MonoBehaviour _mono;
        public MonoBehaviour Mono => _mono;

        private readonly List<UnityCoroutine> _activeCoroutines = new();

        public UnityWorldObject(Transform T, bool isMobile = true) {
            transform = T;
            _isMobile = isMobile;

            if (!transform.gameObject.TryGetComponent(out _mono)) {
                _mono = null;
            }
        }
        public UnityWorldObject(MonoBehaviour mono, bool isMobile = true) {
            _mono = mono;
            transform = _mono.transform;
            _isMobile = isMobile;
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

        public override float3 LocalPosition => transform.localPosition.ToFloat3();
        public override float3 WorldPosition => transform.position.ToFloat3();
        public override float3 LocalRotation => transform.localEulerAngles.ToFloat3();
        public override float3 WorldRotation => transform.eulerAngles.ToFloat3();
        public override float4 LocalQuaternion => transform.localRotation.ToFloat4();
        public override float4 WorldQuaternion => transform.rotation.ToFloat4();
        public override float3 LocalScale => transform.localScale.ToFloat3();
        public override float3 WorldScale => transform.lossyScale.ToFloat3();
        public override float3 WorldForward => transform.forward.ToFloat3();
        public override float3 WorldRight => transform.right.ToFloat3();
        public override float3 WorldUp => transform.up.ToFloat3();

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

        public override void StartCoroutine(in IEnumerator<ICoWait> coroutine) {
            if (_mono != null) {
                var activeCoroutine = new UnityCoroutine(coroutine);
                AddToActiveCoroutines(activeCoroutine);
                _mono.StartCoroutine(activeCoroutine);
            }
        }

        public override void StartCoroutine(in IEnumerable<ICoWait> coroutine) {
            if (_mono != null) {
                _mono.StartCoroutine(new UnityCoroutine(coroutine));
            }
        }

        public override void StopCoroutine(in IEnumerator<ICoWait> coroutine) {
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

        public override void StopAllCoroutines() {
            if (_mono != null) {
                _mono.StopAllCoroutines();

                for (int i = 0; i < _activeCoroutines.Count; ++i) {
                    _activeCoroutines[i] = null;
                }
            }
        }

        public override void Destroy() {
            InvokeOnPreDestroy();
            UnityEngine.Object.Destroy(transform.gameObject);
        }
    }
}
