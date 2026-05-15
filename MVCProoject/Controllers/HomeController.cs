using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("Home/NotFoundPage")]
        public IActionResult NotFoundPage()
        {
            var statusCodeFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            ViewData["OriginalPath"] = statusCodeFeature?.OriginalPath ?? HttpContext.Request.Path.Value ?? string.Empty;
            Response.StatusCode = StatusCodes.Status404NotFound;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
