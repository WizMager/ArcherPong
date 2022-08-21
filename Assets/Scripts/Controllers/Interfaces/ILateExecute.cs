namespace Controllers.Interfaces
{
    public interface ILateExecute : IController
    {
        void LateExecute(float deltaTime);
    }
}