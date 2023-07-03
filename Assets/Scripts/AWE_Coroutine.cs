using System;
using System.Collections.Generic;

namespace AWE.Synzza {
    public enum CoWaitType {
        None,
        WaitForSeconds,
        WaitUntil,
        WaitWhile
    }

    public interface ICoWait {
        CoWaitType WaitType { get; }
    }

    public class CoWaitForSeconds : ICoWait {
        public CoWaitType WaitType => CoWaitType.WaitForSeconds;

        public float Duration { get; }

        public CoWaitForSeconds(float duration) => Duration = duration;
    }

    public class CoWaitUntil : ICoWait {
        public CoWaitType WaitType => CoWaitType.WaitUntil;

        public Func<bool> Until { get; }

        public CoWaitUntil(in Func<bool> until) => Until = until;
    }

    public class CoWaitWhile : ICoWait {
        public CoWaitType WaitType => CoWaitType.WaitWhile;

        public Func<bool> While { get; }

        public CoWaitWhile(in Func<bool> whilefunc) => While = whilefunc;
    }
}
