using Microsoft.AspNetCore.Mvc;
using MVC.Services;
using MVC.ViewModels;

namespace MVC.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task<IActionResult> Index()
        {
            var sessions = await _sessionService.GetAllAsync();
            return View(sessions);
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _sessionService.GetDetailsAsync(id);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Value);
        }

        public async Task<IActionResult> Create()
        {
            return View(await _sessionService.BuildCreateViewModelAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSessionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(await _sessionService.BuildCreateViewModelAsync(model));
            }

            var result = await _sessionService.CreateAsync(model);

            if (result.IsFailure)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Could not create session.");
                return View(await _sessionService.BuildCreateViewModelAsync(model));
            }

            TempData["SuccessMessage"] = "Session created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _sessionService.BuildUpdateViewModelAsync(id);

            if (model is null)
            {
                TempData["ErrorMessage"] = "Session does not exist.";
                return RedirectToAction(nameof(Index));
            }

            if (model.StartDate <= DateTime.Now)
            {
                TempData["ErrorMessage"] = "Only upcoming sessions can be edited.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateSessionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(await _sessionService.PopulateUpdateLookupsAsync(model));
            }

            var result = await _sessionService.UpdateAsync(model);

            if (result.IsFailure)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Could not update session.");
                return View(await _sessionService.PopulateUpdateLookupsAsync(model));
            }

            TempData["SuccessMessage"] = "Session updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _sessionService.GetDetailsAsync(id);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _sessionService.DeleteAsync(id);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Session deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
