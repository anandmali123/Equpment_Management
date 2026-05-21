using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Equipmenet_Management_Systen.Data;
using Equipmenet_Management_Systen.Models;

namespace Equipmenet_Management_Systen.Pages.Admin
{
    [Authorize(Roles = "PurchaseManager,MaintenanceIncharge,PlatformHead,Head")]
    public class UserManagementModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserManagementModel> _logger;

        public List<ApplicationUser> Users { get; set; } = new();
        public List<IdentityRole> Roles { get; set; } = new();
        public ApplicationUser CurrentUser { get; set; } = null!;
        public IList<string> CurrentUserRoles { get; set; } = new List<string>();
        public SelectList? CreateRoles { get; set; }
        public SelectList? DepartmentSelectList { get; set; }
       
        public List<Department> Departments { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        [BindProperty]
        public CreateUserInput NewUser { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        public class CreateUserInput
        {
            [Required]
            public string FullName { get; set; } = string.Empty;

            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string SelectedRole { get; set; } = string.Empty;

            [Required]
            public int? DepartmentId { get; set; }

            [Required, DataType(DataType.Password)]
            [StringLength(100, MinimumLength = 6)]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Compare("Password")]
            public string? ConfirmPassword { get; set; }
        }

        public UserManagementModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserManagementModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            CurrentUser = await _userManager.GetUserAsync(User);
            if (CurrentUser == null) return;

            CurrentUserRoles = await _userManager.GetRolesAsync(CurrentUser);

            Roles = await _roleManager.Roles.ToListAsync();

            Departments = await _context.Departments
                .OrderBy(d => d.Name)
                .ToListAsync();

            var allowedRoles = GetAllowedRolesForCurrentUser();

            foreach (var role in allowedRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));
            }

            CreateRoles = new SelectList(
                allowedRoles.Select(r => new SelectListItem
                {
                    Value = r,
                    Text = FormatRoleName(r)
                }),
                "Value",
                "Text"
            );

            DepartmentSelectList = new SelectList(Departments, "Id", "Name");

            Users = await LoadUsersBasedOnRole();
        }

        public async Task<IActionResult> OnPostCreateUserAsync()
        {
            CurrentUser = await _userManager.GetUserAsync(User);
            if (CurrentUser == null) return RedirectToPage("/Account/Login");

            CurrentUserRoles = await _userManager.GetRolesAsync(CurrentUser);

            if (!CanCreateUserRole(NewUser.SelectedRole))
            {
                ModelState.AddModelError("", "You do not have permission to create this role.");
            }

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var existingUser = await _userManager.FindByEmailAsync(NewUser.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "This email is already registered.");
                await OnGetAsync();
                return Page();
            }

            var user = new ApplicationUser
            {
                UserName = NewUser.Email,
                Email = NewUser.Email,
                EmailConfirmed = true,
                FullName = NewUser.FullName.Trim(),
                DepartmentId = NewUser.DepartmentId,
                UserRole = NewUser.SelectedRole,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                SupervisorId = CurrentUser.Id
            };

            var createResult = await _userManager.CreateAsync(user, NewUser.Password);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError("", error.Description);

                await OnGetAsync();
                return Page();
            }

            var roleResult = await _userManager.AddToRoleAsync(user, NewUser.SelectedRole);

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                    ModelState.AddModelError("", error.Description);

                await OnGetAsync();
                return Page();
            }

            StatusMessage = "User created successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeactivateAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.IsActive = false;
            await _userManager.UpdateAsync(user);

            StatusMessage = "User deactivated successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostActivateAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            StatusMessage = "User activated successfully.";
            return RedirectToPage();
        }

        private async Task<List<ApplicationUser>> LoadUsersBasedOnRole()
        {
            IQueryable<ApplicationUser> query = _context.Users
                .Include(u => u.Department);

            if (CurrentUserRoles.Contains("PurchaseManager"))
            {
                query = query.Where(u =>
                    u.Id != CurrentUser.Id &&
                    u.UserRole != "PurchaseManager");
            }
            else if (CurrentUserRoles.Contains("MaintenanceIncharge"))
            {
                query = query.Where(u =>
                    u.SupervisorId == CurrentUser.Id ||
                    u.UserRole == "PlatformHead" ||
                    u.UserRole == "Head" ||
                    u.UserRole == "User");
            }
            else if (CurrentUserRoles.Contains("PlatformHead"))
            {
                query = query.Where(u =>
                    u.SupervisorId == CurrentUser.Id ||
                    u.UserRole == "Head" ||
                    u.UserRole == "User");
            }
            else if (CurrentUserRoles.Contains("Head"))
            {
                query = query.Where(u =>
                    u.SupervisorId == CurrentUser.Id ||
                    u.UserRole == "User");
            }

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var search = SearchTerm.Trim();
                query = query.Where(u =>
                    EF.Functions.Like(u.FullName, $"%{search}%") ||
                    EF.Functions.Like(u.Email, $"%{search}%"));
            }

            return await query
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        private List<string> GetAllowedRolesForCurrentUser()
        {
            if (CurrentUserRoles.Contains("PurchaseManager"))
                return new() { "MaintenanceIncharge", "PlatformHead", "Head", "User" };

            if (CurrentUserRoles.Contains("MaintenanceIncharge"))
                return new() { "PlatformHead", "Head", "User" };

            if (CurrentUserRoles.Contains("PlatformHead"))
                return new() { "Head", "User" };

            if (CurrentUserRoles.Contains("Head"))
                return new() { "User" };

            return new();
        }

        private bool CanCreateUserRole(string targetRole)
        {
            return GetAllowedRolesForCurrentUser().Contains(targetRole);
        }

        private string FormatRoleName(string role)
        {
            return System.Text.RegularExpressions.Regex
                .Replace(role, "([A-Z])", " $1")
                .Trim();
        }
    }
}