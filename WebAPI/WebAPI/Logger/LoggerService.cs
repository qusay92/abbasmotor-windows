using Newtonsoft.Json;

namespace WebAPI.Logger
{
    public class LoggerService : ILoggerService
    {
        private readonly ILogger<LoggerService> _logger;

        public LoggerService(ILogger<LoggerService> logger)
        {
            _logger = logger;
        }

        public void LogError(Exception ex, object Parameter, string ControllerName, string ActionName)
        {
            string exResult = "";
            if (ex != null)
            {
                exResult = $" ## Exception.Message = {ex.Message}";

                if (ex.InnerException != null)
                {
                    exResult += $" @@ InnerException.Message = {ex.InnerException.Message} ";
                }
            }
            Log.Error($"Controller : {ControllerName} - ActionName : {ActionName} - Parameter : {JsonConvert.SerializeObject(Parameter)} - Error : {exResult}");
        }
    }
}
