namespace Controllers
{
    public interface ILateExecute : IController
    {
        void LateExecute();
    }
}