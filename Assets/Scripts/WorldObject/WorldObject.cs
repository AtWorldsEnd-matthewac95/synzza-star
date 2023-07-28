using System;
using System.Collections.Generic;

namespace AWE.Synzza {
    public interface IWorldObject {
        float3 LocalPosition { get; }
        float3 WorldPosition { get; }
        float3 LocalRotation { get; }
        float3 WorldRotation { get; }
        float4 LocalQuaternion { get; }
        float4 WorldQuaternion { get; }
        float3 LocalScale { get; }
        float3 WorldScale { get; }
        float3 WorldForward { get; }
        float3 WorldRight { get; }
        float3 WorldUp { get; }
    }

    public interface IMutableWorldObject : IWorldObject {
        bool IsMobile { get; }

        event WorldObjectPreDestroyDelegate OnPreDestroy;

        void StartCoroutine(in IEnumerator<ICoWait> coroutine);
        void StartCoroutine(in IEnumerable<ICoWait> coroutine);
        void StopCoroutine(in IEnumerator<ICoWait> coroutine);
        void StopAllCoroutines();
        void Destroy();
    }

    public class WorldObject : IMutableWorldObject {
        public abstract class Impl {
            public abstract float3 LocalPosition { get; }
            public abstract float3 WorldPosition { get; }
            public abstract float3 LocalRotation { get; }
            public abstract float3 WorldRotation { get; }
            public abstract float4 LocalQuaternion { get; }
            public abstract float4 WorldQuaternion { get; }
            public abstract float3 LocalScale { get; }
            public abstract float3 WorldScale { get; }
            public abstract bool IsMobile { get; }
            public abstract float3 WorldForward { get; }
            public abstract float3 WorldRight { get; }
            public abstract float3 WorldUp { get; }

            public event Action OnPreDestroy;
            protected void InvokeOnPreDestroy() {
                OnPreDestroy?.Invoke();
            }

            public abstract void StartCoroutine(in IEnumerator<ICoWait> coroutine);
            public abstract void StartCoroutine(in IEnumerable<ICoWait> coroutine);
            public abstract void StopCoroutine(in IEnumerator<ICoWait> coroutine);
            public abstract void StopAllCoroutines();
            public abstract void Destroy();
        }

        private readonly Impl _impl;

        public float3 LocalPosition => _impl.LocalPosition;
        public float3 WorldPosition => _impl.WorldPosition;
        public float3 LocalRotation => _impl.LocalRotation;
        public float3 WorldRotation => _impl.WorldRotation;
        public float4 LocalQuaternion => _impl.LocalQuaternion;
        public float4 WorldQuaternion => _impl.WorldQuaternion;
        public float3 LocalScale => _impl.LocalScale;
        public float3 WorldScale => _impl.WorldScale;
        public bool IsMobile => _impl.IsMobile;
        public float3 WorldForward => _impl.WorldForward;
        public float3 WorldRight => _impl.WorldRight;
        public float3 WorldUp => _impl.WorldUp;

        public event WorldObjectPreDestroyDelegate OnPreDestroy;

        public WorldObject(Impl impl) {
            _impl = impl;
            _impl.OnPreDestroy += OnImplPreDestroy;
        }

        private bool _isImplToBeDestroyed = false;

        private void OnImplPreDestroy() {
            if (!_isImplToBeDestroyed) {
                _isImplToBeDestroyed = true;
                Destroy();
            }
        }

        public void StartCoroutine(in IEnumerator<ICoWait> coroutine) => _impl.StartCoroutine(coroutine);
        public void StartCoroutine(in IEnumerable<ICoWait> coroutine) => _impl.StartCoroutine(coroutine);
        public void StopCoroutine(in IEnumerator<ICoWait> coroutine) => _impl.StopCoroutine(coroutine);
        public void StopAllCoroutines() => _impl.StopAllCoroutines();

        public void Destroy() {
            OnPreDestroy?.Invoke(this);

            if (!_isImplToBeDestroyed) {
                _isImplToBeDestroyed = true;
                _impl.Destroy();
            }
        }

        public TImpl GetImpl<TImpl>() where TImpl : Impl {
            if (_impl is not TImpl validImpl) {
                throw new ArgumentException($"This {GetType().Name} instance is implemented with {_impl.GetType().Name}, not {typeof(TImpl).Name}!");
            }

            return validImpl;
        }

        public bool TryGetImpl<TImpl>(out TImpl impl) where TImpl : Impl {
            if (_impl is not TImpl validImpl) {
                impl = default;
                return false;
            }

            impl = validImpl;
            return true;
        }
    }
}