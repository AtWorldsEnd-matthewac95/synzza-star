namespace AWE.Synzza {
    public interface INpcMovement {
        IWorldObject PickNewMovementTarget(IWorldObject previousTarget);
    }
}
