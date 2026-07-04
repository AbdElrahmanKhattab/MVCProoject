using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Services;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAnalyticsService _analyticsService;

        public HomeController(ILogger<HomeController> logger, IAnalyticsService analyticsService)
        {
            _logger = logger;
            _analyticsService = analyticsService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _analyticsService.GetDashboardAsync());
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
