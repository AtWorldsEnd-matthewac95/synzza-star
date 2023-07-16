using System;

namespace AWE.Synzza {
    public abstract class ByteSizedRegistry<TRegisterable> where TRegisterable : struct {
        protected struct Pair {
            public bool IsRegistered;
            public TRegisterable Registerable;
            public Pair(bool isRegistered, in TRegisterable faction) { IsRegistered = isRegistered; Registerable = faction; }
        }

        protected readonly Pair[] _registry;

        protected abstract byte GetRegisterableID(in TRegisterable registerable);
        protected virtual string GetRegisterableDisplayName(in TRegisterable registerable) => string.Empty;
        protected virtual bool TryGetInvalidID(out byte invalidID) { invalidID = default; return false; }

        public ByteSizedRegistry() {
            _registry = new Pair[byte.MaxValue + 1];
        }

        public bool IsRegistered(byte id) => _registry[id].IsRegistered;
        public ref TRegisterable this[byte id] => ref _registry[id].Registerable;

        public bool TryRegister(in TRegisterable registerable, bool isOverwritingAllowed = false) => TryRegister(registerable, out _, isOverwritingAllowed);
        public bool TryRegister(in TRegisterable registerable, out TRegisterable registered) => TryRegister(registerable, out registered, false);
        protected bool TryRegister(in TRegisterable registerable, out TRegisterable registered, bool isOverwritingAllowed) {
            var id = GetRegisterableID(registerable);
            var isAbleToRegister = (!TryGetInvalidID(out byte invalidID) || id != invalidID) && (isOverwritingAllowed || !IsRegistered(id));

            if (isAbleToRegister) {
                UncheckedRegister(registerable);
            }

            registered = _registry[id].Registerable;
            return isAbleToRegister;
        }

        public void Register(in TRegisterable registerable, bool isOverwritingAllowed = false) {
            var id = GetRegisterableID(registerable);

            if (TryGetInvalidID(out byte invalidID) && id == invalidID) {
                throw new InvalidOperationException($"ID {invalidID} is invalid for this {typeof(TRegisterable).Name} registry.");
            }

            if (!isOverwritingAllowed && IsRegistered(id)) {
                ref var existing = ref _registry[id].Registerable;
                var existingDisplayName = GetRegisterableDisplayName(existing);
                var displayNameString = string.IsNullOrWhiteSpace(existingDisplayName) ? string.Empty : $" \"{GetRegisterableDisplayName(existing)}\"";
                throw new InvalidOperationException($"Overwriting existing registered element{displayNameString} with ID {id} is disallowed.");
            }

            UncheckedRegister(registerable);
        }

        protected void UncheckedRegister(in TRegisterable registerable) {
            _registry[GetRegisterableID(registerable)] = new(true, registerable);
        }

        public bool Unregister(in TRegisterable registerable) {
            var id = GetRegisterableID(registerable);

            if (_registry[id].IsRegistered) {
                _registry[id] = new(false, default);
                return true;
            }

            return false;
        }
    }
}
