using Equipmenet_Management_Systen.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceProviderModel = Equipmenet_Management_Systen.Models.ServiceProvider;

namespace Equipmenet_Management_Systen.Controllers
{
    [Authorize(Roles = "MaintenanceIncharge,PurchaseManager,PlatformHead,Head")]
    public class ServiceProviderController : Controller
    {
        private readonly IServiceProviderService _serviceProviderService;
        private readonly ILogger<ServiceProviderController> _logger;

        public ServiceProviderController(
            IServiceProviderService serviceProviderService,
            ILogger<ServiceProviderController> logger)
        {
            _serviceProviderService = serviceProviderService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var providers = await _serviceProviderService.GetAllProvidersAsync();
            return View(providers);
        }

        public IActionResult Create()
        {
            SetSelectLists();
            return View(new ServiceProviderModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceProviderModel provider)
        {
            if (!ModelState.IsValid)
            {
                SetSelectLists();
                return View(provider);
            }

            provider.CreatedDate = DateTime.UtcNow;
            if (!User.IsInRole("Head"))
            {
                provider.ApprovalStatus = "Pending";
            }

            var success = await _serviceProviderService.CreateProviderAsync(provider);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Unable to save the service provider. Please try again.");
                SetSelectLists();
                return View(provider);
            }

            TempData["Success"] = "Service provider added successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var provider = await _serviceProviderService.GetProviderByIdAsync(id);
            if (provider == null)
            {
                return NotFound();
            }

            SetSelectLists();
            return View("Create", provider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceProviderModel provider)
        {
            if (!ModelState.IsValid)
            {
                SetSelectLists();
                return View("Create", provider);
            }

            var success = await _serviceProviderService.UpdateProviderAsync(provider);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Unable to update the service provider. Please try again.");
                SetSelectLists();
                return View("Create", provider);
            }

            TempData["Success"] = "Service provider updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _serviceProviderService.DeleteProviderAsync(id);
            if (!success)
            {
                TempData["Success"] = "Unable to delete the service provider.";
            }
            else
            {
                TempData["Success"] = "Service provider deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            if (!User.IsInRole("Head") && !User.IsInRole("PlatformHead"))
            {
                return Forbid();
            }

            var approvedBy = User.Identity?.Name ?? "System";
            var success = await _serviceProviderService.ApproveProviderAsync(id, approvedBy);

            if (!success)
            {
                TempData["Success"] = "Unable to approve the service provider.";
            }
            else
            {
                TempData["Success"] = "Service provider approved successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private void SetSelectLists()
        {
            ViewBag.ServiceTypes = new SelectList(new[]
            {
                "AMC",
                "Calibration",
                "Installation",
                "Preventive Maintenance",
                "Repair"
            });

            ViewBag.ApprovalStatuses = new SelectList(new[]
            {
                "Pending",
                "Approved",
                "Rejected"
            });
        }
    }
}
