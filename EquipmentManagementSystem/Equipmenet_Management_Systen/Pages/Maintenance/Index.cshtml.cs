using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Equipmenet_Management_Systen.Data;
using Equipmenet_Management_Systen.Models;

namespace Equipmenet_Management_Systen.Pages.Maintenance
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IndexModel> _logger;

        public IList<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
        public ApplicationUser? CurrentUser { get; set; }
        public IList<string>? UserRoles { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<IndexModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            try
            {
                CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
                UserRoles = await _userManager.GetRolesAsync(CurrentUser);

                IQueryable<MaintenanceRequest> query = _context.MaintenanceRequests
                    .Include(m => m.Equipment)
                    .Include(m => m.AssignedProvider);

                // Filter based on role
                if (UserRoles.Contains("User"))
                {
                    // Users see only their submitted requests
                    query = query.Where(m => m.Equipment.AssignedUserId == CurrentUser.Id);
                }
                else if (UserRoles.Contains("Head"))
                {
                    // Department heads see their department's requests
                    query = query.Where(m => m.Equipment.DepartmentId == CurrentUser.DepartmentId);
                }
                // Platform Head sees all

                // Apply status filter
                if (!string.IsNullOrWhiteSpace(StatusFilter))
                {
                    query = query.Where(m => m.Status == StatusFilter);
                }

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    query = query.Where(m => m.Equipment.EquipmentName.Contains(SearchTerm));
                }

                MaintenanceRequests = await query
                    .OrderByDescending(m => m.RequestDate)
                    .ToListAsync();

                _logger.LogInformation($"Maintenance requests loaded for {CurrentUser?.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading maintenance requests");
            }
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            try
            {
                CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
                UserRoles = await _userManager.GetRolesAsync(CurrentUser);

                var request = await _context.MaintenanceRequests.FindAsync(id);
                if (request == null)
                {
                    return NotFound();
                }

                // Only admin can approve
                if (!UserRoles.Contains("PlatformHead") && !UserRoles.Contains("Head"))
                {
                    return Forbid();
                }

                request.Status = "Approved";
                request.ApprovedAt = DateTime.UtcNow;
                request.ApprovedBy = CurrentUser?.Id;

                _context.MaintenanceRequests.Update(request);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Maintenance request {id} approved by {CurrentUser?.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving maintenance request");
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            try
            {
                CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
                UserRoles = await _userManager.GetRolesAsync(CurrentUser);

                var request = await _context.MaintenanceRequests.FindAsync(id);
                if (request == null)
                {
                    return NotFound();
                }

                // Only admin can reject
                if (!UserRoles.Contains("PlatformHead") && !UserRoles.Contains("Head"))
                {
                    return Forbid();
                }

                request.Status = "Rejected";
                _context.MaintenanceRequests.Update(request);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Maintenance request {id} rejected by {CurrentUser?.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting maintenance request");
            }

            return RedirectToPage();
        }
    }
}
