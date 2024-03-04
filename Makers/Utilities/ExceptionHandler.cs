using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Makers.Utilities;

public class ExceptionHandler : ExceptionFilterAttribute
{
    private readonly ILogger<ExceptionHandler> _logger;

    public ExceptionHandler(ILogger<ExceptionHandler> logger)
    {
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        var msg = context.Exception.Message;
        var exception = context.Exception;

        if (msg.StartsWith("ORA-20000: "))
        {
            _logger.LogWarning(msg);

            msg = msg.ExtractErrorMessage();
        }

        else
        {
            if (context.Exception.InnerException is not null)
            {
                if (exception.InnerException.Message.Contains("ORA-00001: unique constraint"))
                {
                    msg = "Item already exist";
                }

                else
                {
                    msg += $" Inner exception: {exception.InnerException.Message}";
                }
            }

            _logger.LogWarning($"Error: {exception}");
        }

        context.Result = new JsonResult(new { StatusCode = (int)StatusCodes.Error, StatusDescription = msg });
    }
}