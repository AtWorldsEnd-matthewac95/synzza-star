namespace AWE.Synzza {
    public interface INpcMovement {
        ISceneObject PickNewMovementTarget(ISceneObject previousTarget);
    }
}
