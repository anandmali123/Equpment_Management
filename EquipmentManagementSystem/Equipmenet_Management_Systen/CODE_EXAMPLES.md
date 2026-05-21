# Code Examples - Role-Based System

## 1. How to Check User Role in Code

### In Razor Pages (C#)
```csharp
// Get current user
var currentUser = await _userManager.GetUserAsync(HttpContext.User);

// Get user roles
var userRoles = await _userManager.GetRolesAsync(currentUser);

// Check if user has specific role
if (userRoles.Contains("PlatformHead"))
{
    // Do something for Platform Head
}
```

### In Razor Pages (.cshtml)
```razor
@if (Model.UserRoles.Contains("PlatformHead"))
{
    <div class="alert alert-info">Only Platform Head can see this</div>
}
```

## 2. How to Restrict Page Access

### Using Authorize Attribute
```csharp
[Authorize]  // Any authenticated user
public class DashboardModel : PageModel { }

[Authorize(Roles = "PlatformHead")]  // Only Platform Head
public class AdminModel : PageModel { }

[Authorize(Roles = "PlatformHead,PurchaseManager")]  // Multiple roles
public class ReportModel : PageModel { }
```

### Using Policy-Based Authorization
```csharp
[Authorize(Policy = "AdminOnly")]
public class SettingsModel : PageModel { }
```

## 3. How to Filter Data by Role

### Example: Get Equipment Based on User Role
```csharp
public async Task LoadEquipmentByRole()
{
    var userRoles = await _userManager.GetRolesAsync(CurrentUser);

    if (userRoles.Contains("PlatformHead"))
    {
        // Get all equipment
        Equipment = await _context.Equipments.ToListAsync();
    }
    else if (userRoles.Contains("Head"))
    {
        // Get department equipment
        Equipment = await _context.Equipments
            .Where(e => e.DepartmentId == CurrentUser.DepartmentId)
            .ToListAsync();
    }
    else
    {
        // Get assigned equipment
        Equipment = await _context.Equipments
            .Where(e => e.AssignedUserId == CurrentUser.Id)
            .ToListAsync();
    }
}
```

## 4. How to Change User Role

### Example: Change User Role (with validation)
```csharp
public async Task<IActionResult> ChangeUserRoleAsync(string userId, string newRole)
{
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
        return NotFound();

    // Get current user's roles
    var currentRoles = await _userManager.GetRolesAsync(CurrentUser);

    // Check if current user can assign this role
    if (!CanAssignRole(currentRoles, newRole))
    {
        ModelState.AddModelError("", "You cannot assign this role");
        return Page();
    }

    // Remove existing roles
    var oldRoles = await _userManager.GetRolesAsync(user);
    await _userManager.RemoveFromRolesAsync(user, oldRoles);

    // Assign new role
    await _userManager.AddToRoleAsync(user, newRole);

    // Update UserRole property
    user.UserRole = newRole;
    await _userManager.UpdateAsync(user);

    return Page();
}

private bool CanAssignRole(IList<string> currentRoles, string targetRole)
{
    var hierarchy = new Dictionary<string, int>
    {
        { "User", 1 },
        { "Head", 2 },
        { "MaintenanceIncharge", 3 },
        { "PurchaseManager", 4 },
        { "PlatformHead", 5 }
    };

    var targetLevel = hierarchy[targetRole];
    var maxCurrentLevel = currentRoles
        .Where(r => hierarchy.ContainsKey(r))
        .Max(r => hierarchy[r]);

    return maxCurrentLevel > targetLevel;
}
```

## 5. How to Create Conditional UI

### Hide/Show Buttons Based on Role
```razor
<div class="btn-group">
    @if (Model.UserRoles.Contains("Head"))
    {
        <a asp-page="/Equipment/Create" class="btn btn-success">Add Equipment</a>
    }

    @if (Model.UserRoles.Contains("PlatformHead"))
    {
        <a asp-page="/Admin/UserManagement" class="btn btn-primary">Manage Users</a>
    }

    @if (Model.UserRoles.Contains("User"))
    {
        <a asp-page="/Maintenance/Submit" class="btn btn-warning">Submit Request</a>
    }
</div>
```

### Conditional Dashboard Sections
```razor
@if (Model.UserRoles.Contains("Head"))
{
    <div class="card">
        <h5>Your Team Members</h5>
        <ul>
        @foreach (var user in Model.AssignedUsers)
        {
            <li>@user.FullName (@user.Email)</li>
        }
        </ul>
    </div>
}
```

## 6. How to Log User Actions

### Track Role Changes
```csharp
public async Task LogUserActionAsync(string action, string userId)
{
    var log = new ActivityLog
    {
        PerformedBy = CurrentUser.Id,
        PerformedOn = userId,
        Action = action,
        Timestamp = DateTime.UtcNow,
        Details = $"User {CurrentUser.Email} performed {action} on {userId}"
    };

    _context.ActivityLogs.Add(log);
    await _context.SaveChangesAsync();
}

// Usage:
await LogUserActionAsync("RoleChanged", userId);
await LogUserActionAsync("UserDeactivated", userId);
```

## 7. How to Validate Role Hierarchy

### Prevent Unauthorized Actions
```csharp
public bool IsUserAuthorized(ApplicationUser actor, ApplicationUser target, string action)
{
    var actorRoles = _userManager.GetRolesAsync(actor).Result;
    var targetRoles = _userManager.GetRolesAsync(target).Result;

    var hierarchy = new Dictionary<string, int>
    {
        { "User", 1 },
        { "Head", 2 },
        { "MaintenanceIncharge", 3 },
        { "PurchaseManager", 4 },
        { "PlatformHead", 5 }
    };

    var actorLevel = actorRoles
        .Where(r => hierarchy.ContainsKey(r))
        .Max(r => hierarchy[r]);

    var targetLevel = targetRoles
        .Where(r => hierarchy.ContainsKey(r))
        .Max(r => hierarchy[r]);

    // Action allowed if actor is higher level
    return actorLevel > targetLevel;
}
```

## 8. How to Implement Custom Authorization Policy

### Create a Custom Policy
```csharp
// In Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("PlatformHead", "PurchaseManager"));

    options.AddPolicy("ManagerOrAbove", policy =>
        policy.RequireRole("Head", "MaintenanceIncharge", 
                          "PurchaseManager", "PlatformHead"));
});

// Use in code
[Authorize(Policy = "AdminOnly")]
public class AdminPageModel : PageModel { }
```

## 9. How to Handle Role-Based Redirects

### Redirect After Login Based on Role
```csharp
public async Task<IActionResult> OnPostLoginAsync(string returnUrl)
{
    var result = await _signInManager.PasswordSignInAsync(
        Input.Email, Input.Password, Input.RememberMe, false);

    if (result.Succeeded)
    {
        var user = await _userManager.FindByEmailAsync(Input.Email);
        var roles = await _userManager.GetRolesAsync(user);

        // Redirect based on role
        if (roles.Contains("PlatformHead"))
            return RedirectToPage("/Admin/Dashboard");
        else if (roles.Contains("Head"))
            return RedirectToPage("/Department/Dashboard");
        else
            return RedirectToPage("/Dashboard/Index");
    }

    ModelState.AddModelError("", "Invalid login attempt");
    return Page();
}
```

## 10. How to Track Supervisor Relationships

### Assign Supervisor to User
```csharp
public async Task AssignSupervisorAsync(string userId, string supervisorId)
{
    var user = await _context.Users.FindAsync(userId);
    var supervisor = await _context.Users.FindAsync(supervisorId);

    if (user == null || supervisor == null)
        throw new InvalidOperationException("User or supervisor not found");

    user.SupervisorId = supervisorId;
    await _context.SaveChangesAsync();
}

// Usage:
await AssignSupervisorAsync(newUserId, HeadId);
```

### Get Supervised Users
```csharp
public async Task<List<ApplicationUser>> GetSupervisedUsersAsync(string supervisorId)
{
    return await _context.Users
        .Where(u => u.SupervisorId == supervisorId && u.IsActive)
        .ToListAsync();
}
```

## 11. How to Create Role-Based Reports

### Count Users by Role
```csharp
public async Task<Dictionary<string, int>> GetUserCountByRoleAsync()
{
    var counts = new Dictionary<string, int>();
    var roles = await _roleManager.Roles.ToListAsync();

    foreach (var role in roles)
    {
        var userCount = (await _userManager.GetUsersInRoleAsync(role.Name)).Count;
        counts[role.Name] = userCount;
    }

    return counts;
}
```

### Get Active Users by Department
```csharp
public async Task<List<ApplicationUser>> GetActiveUsersByDepartmentAsync(int deptId)
{
    return await _context.Users
        .Where(u => u.DepartmentId == deptId && u.IsActive)
        .OrderBy(u => u.FullName)
        .ToListAsync();
}
```

## 12. How to Implement Role-Based Soft Delete

### Deactivate Instead of Delete
```csharp
public async Task DeactivateUserAsync(string userId)
{
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
        return;

    user.IsActive = false;
    await _userManager.UpdateAsync(user);

    // Log the action
    await LogUserActionAsync("UserDeactivated", userId);
}

// Reactivate user
public async Task ActivateUserAsync(string userId)
{
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
        return;

    user.IsActive = true;
    await _userManager.UpdateAsync(user);

    await LogUserActionAsync("UserActivated", userId);
}
```

## 13. How to Get User's Last Login

```csharp
// Set last login on successful authentication
public async Task OnPostLoginAsync()
{
    if (result.Succeeded)
    {
        var user = await _userManager.FindByEmailAsync(Input.Email);
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
    }
}

// Get dashboard with login info
public async Task OnGetAsync()
{
    CurrentUser = await _userManager.GetUserAsync(HttpContext.User);

    if (CurrentUser.LastLoginAt.HasValue)
    {
        var daysSinceLogin = (DateTime.UtcNow - CurrentUser.LastLoginAt.Value).Days;
        ViewData["DaysSinceLogin"] = daysSinceLogin;
    }
}
```

---

## Common Patterns

### Pattern 1: Load and Filter Data
```csharp
public async Task LoadDataAsync()
{
    var userRoles = await _userManager.GetRolesAsync(CurrentUser);

    Data = userRoles.Contains("PlatformHead") 
        ? await GetAllData() 
        : await GetRestrictedData();
}
```

### Pattern 2: Check Authorization
```csharp
if (!userRoles.Contains("PlatformHead") && 
    !userRoles.Contains("PurchaseManager"))
{
    return Unauthorized();
}
```

### Pattern 3: Conditional Rendering
```razor
@if (condition)
{
    <div>Show this</div>
}
else
{
    <div>Show that</div>
}
```

---

## Best Practices

1. **Always check roles on both client and server**
   - Don't rely only on hiding UI elements
   - Always validate on server-side

2. **Use Authorize attributes**
   - Prevents unauthorized page access
   - More secure than UI hiding

3. **Log role changes**
   - Track who changed what and when
   - Important for audit trails

4. **Validate hierarchy before changes**
   - Prevent unauthorized role assignments
   - Enforce business rules

5. **Filter data appropriately**
   - Don't load data users shouldn't see
   - Reduces security risks

6. **Cache roles appropriately**
   - Load roles once, cache them
   - Improves performance

7. **Handle role changes gracefully**
   - Log activities
   - Notify affected users
   - Update dashboards

---

**Remember**: Security must be enforced on the server side, not just the UI!
