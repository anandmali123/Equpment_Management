using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Equipmenet_Management_Systen.Data;
using Equipmenet_Management_Systen.Models;
using EquipmentModel = Equipmenet_Management_Systen.Models.Equipment;

namespace Equipmenet_Management_Systen.Pages.Equipment
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EditModel> _logger;

        [BindProperty]
        public EquipmentModel Equipment { get; set; } = new();

        public SelectList? DepartmentList { get; set; }
        public SelectList? ServiceProviderList { get; set; }
        public SelectList? UserList { get; set; }
        public ApplicationUser? CurrentUser { get; set; }
        public IList<string>? UserRoles { get; set; }

        public EditModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<EditModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
                UserRoles = await _userManager.GetRolesAsync(CurrentUser);

                Equipment = await _context.Equipments.FindAsync(id);
                if (Equipment == null)
                {
                    return NotFound();
                }

                // Check permission
                if (!UserRoles.Contains("PlatformHead") && 
                    !(UserRoles.Contains("Head") && Equipment.DepartmentId == CurrentUser.DepartmentId))
                {
                    return Forbid();
                }

                // Load dropdowns
                List<Models.Department> departments;
                if (UserRoles.Contains("PlatformHead"))
                {
                    departments = await _context.Departments.ToListAsync();
                }
                else if (UserRoles.Contains("Head"))
                {
                    departments = await _context.Departments
                        .Where(d => d.Id == CurrentUser.DepartmentId)
                        .ToListAsync();
                }
                else
                {
                    departments = new List<Models.Department>();
                }

                DepartmentList = new SelectList(departments, "Id", "Name", Equipment.DepartmentId);
                ServiceProviderList = new SelectList(await _context.ServiceProviders.ToListAsync(), "Id", "Name", Equipment.ServiceProviderId);
                UserList = new SelectList(await _context.Users.Where(u => u.IsActive).ToListAsync(), "Id", "FullName", Equipment.AssignedUserId);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading equipment for edit");
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
                UserRoles = await _userManager.GetRolesAsync(CurrentUser);

                var existingEquipment = await _context.Equipments.FindAsync(Equipment.Id);
                if (existingEquipment == null)
                {
                    return NotFound();
                }

                // Check permission
                if (!UserRoles.Contains("PlatformHead") && 
                    !(UserRoles.Contains("Head") && existingEquipment.DepartmentId == CurrentUser.DepartmentId))
                {
                    return Forbid();
                }

                if (!ModelState.IsValid)
                {
                    await OnGetAsync(Equipment.Id);
                    return Page();
                }

                // Update properties
                existingEquipment.EquipmentName = Equipment.EquipmentName;
                existingEquipment.EquipmentCode = Equipment.EquipmentCode;
                existingEquipment.Description = Equipment.Description;
                existingEquipment.Location = Equipment.Location;
                existingEquipment.Value = Equipment.Value;
                existingEquipment.Status = Equipment.Status;
                existingEquipment.DepartmentId = Equipment.DepartmentId;
                existingEquipment.ServiceProviderId = Equipment.ServiceProviderId;
                existingEquipment.AssignedUserId = Equipment.AssignedUserId;
                existingEquipment.PurchaseDate = Equipment.PurchaseDate;
                existingEquipment.WarrantyExpiryDate = Equipment.WarrantyExpiryDate;
                existingEquipment.ManufacturerName = Equipment.ManufacturerName;
                existingEquipment.ManufacturerModel = Equipment.ManufacturerModel;
                existingEquipment.SerialNumber = Equipment.SerialNumber;
                existingEquipment.UpdatedAt = DateTime.UtcNow;
                existingEquipment.UpdatedBy = CurrentUser?.Id ?? string.Empty;

                _context.Equipments.Update(existingEquipment);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Equipment '{Equipment.EquipmentName}' updated by {CurrentUser?.Email}");
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating equipment");
                ModelState.AddModelError("", "Error updating equipment. Please try again.");
                await OnGetAsync(Equipment.Id);
                return Page();
            }
        }
    }
}
