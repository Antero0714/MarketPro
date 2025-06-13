using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace MarketPro.Controllers
{
    public class ErrorsController : Controller
    {
        private readonly ILogger<ErrorsController> _logger;

        public ErrorsController(ILogger<ErrorsController> logger)
        {
            _logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            
            switch (statusCode)
            {
                case 404:
                    _logger.LogWarning($"404 error occurred. Path: {statusCodeResult?.OriginalPath}");
                    return View("~/Views/Shared/404error.cshtml");
                case 500:
                    _logger.LogError($"500 error occurred. Path: {statusCodeResult?.OriginalPath}");
                    return View("~/Views/Shared/500error.cshtml");
                default:
                    _logger.LogError($"{statusCode} error occurred. Path: {statusCodeResult?.OriginalPath}");
                    return View("~/Views/Shared/500error.cshtml");
            }
        }

        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            
            if (exceptionDetails != null)
            {
                _logger.LogError(exceptionDetails.Error, 
                    $"An unhandled exception occurred. Path: {exceptionDetails.Path}");
            }
            
            return View("~/Views/Shared/500error.cshtml");
        }
    }
} 