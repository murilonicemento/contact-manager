using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.Controllers;

public class HomeController : Controller
{
    [Route("/Error")]
    public IActionResult Error()
    {
        IExceptionHandlerPathFeature? exceptionHandlerPathFeature =
            HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature is not null)
        {
            ViewBag.ErrorMessage = exceptionHandlerPathFeature.Error.Message;
        }

        return View();
    }
}