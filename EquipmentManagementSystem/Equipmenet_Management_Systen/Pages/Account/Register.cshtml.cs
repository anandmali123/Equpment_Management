using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Equipmenet_Management_Systen.Models;
using System.ComponentModel.DataAnnotations;

namespace Equipmenet_Management_Systen.Pages.Account
{
	[Authorize(Roles = "PurchaseManager,MaintenanceIncharge,PlatformHead,Head")]
	public class RegisterModel : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ILogger<RegisterModel> _logger;

		[BindProperty]
		public InputModel Input { get; set; } = new();

		public SelectList? Roles { get; set; }

		public class InputModel
		{
			[Required]
			public string FullName { get; set; } = string.Empty;

			[Required, EmailAddress]
			public string Email { get; set; } = string.Empty;

			public string? PhoneNumber { get; set; }
			public string? City { get; set; }

			[Required, DataType(DataType.Password)]
			public string Password { get; set; } = string.Empty;

			[DataType(DataType.Password)]
			[Compare("Password")]
			public string? ConfirmPassword { get; set; }

			[Required]
			public string SelectedRole { get; set; } = string.Empty;
		}

		public RegisterModel(
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			ILogger<RegisterModel> logger)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			await LoadRolesAsync();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var allowedRoles = GetAllowedRoles();

			if (!allowedRoles.Contains(Input.SelectedRole))
			{
				ModelState.AddModelError("", "You are not allowed to create this role.");
			}

			if (!ModelState.IsValid)
			{
				await LoadRolesAsync();
				return Page();
			}

			var existingUser = await _userManager.FindByEmailAsync(Input.Email);
			if (existingUser != null)
			{
				ModelState.AddModelError("", "This email is already registered.");
				await LoadRolesAsync();
				return Page();
			}

			var currentUser = await _userManager.GetUserAsync(User);

			var user = new ApplicationUser
			{
				UserName = Input.Email,
				Email = Input.Email,
				EmailConfirmed = true,
				FullName = Input.FullName.Trim(),
				PhoneNumber = Input.PhoneNumber?.Trim(),
				City = Input.City?.Trim(),
				IsActive = true,
				CreatedAt = DateTime.UtcNow,
				UserRole = Input.SelectedRole,
				SupervisorId = currentUser?.Id
			};

			var result = await _userManager.CreateAsync(user, Input.Password);

			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(user, Input.SelectedRole);
				return RedirectToPage("/Dashboard/Index");
			}

			foreach (var error in result.Errors)
				ModelState.AddModelError("", error.Description);

			await LoadRolesAsync();
			return Page();
		}

		private async Task LoadRolesAsync()
		{
			var allowedRoles = GetAllowedRoles();

			foreach (var role in allowedRoles)
			{
				if (!await _roleManager.RoleExistsAsync(role))
					await _roleManager.CreateAsync(new IdentityRole(role));
			}

			Roles = new SelectList(
				allowedRoles.Select(r => new SelectListItem
				{
					Value = r,
					Text = FormatRoleName(r)
				}),
				"Value",
				"Text"
			);
		}

		private List<string> GetAllowedRoles()
		{
			if (User.IsInRole("PurchaseManager"))
				return new() { "MaintenanceIncharge", "PlatformHead", "Head", "User" };

			if (User.IsInRole("MaintenanceIncharge"))
				return new() { "PlatformHead", "Head", "User" };

			if (User.IsInRole("PlatformHead"))
				return new() { "Head", "User" };

			if (User.IsInRole("Head"))
				return new() { "User" };

			return new();
		}

		private string FormatRoleName(string role)
		{
			return System.Text.RegularExpressions.Regex
				.Replace(role, "([A-Z])", " $1")
				.Trim();
		}
	}
}