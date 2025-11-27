namespace WebAPI.Logger
{
    public interface ILoggerService
    {
        void LogError(Exception ex, object Parameter, string ControllerName, string ActionName);
    }
}
