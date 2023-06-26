namespace AWE.Synzza {
    public interface IScene {
        ISceneObject FindRandomPlayer();
        bool IsSceneObjectPlayer(ISceneObject obj);
    }
}