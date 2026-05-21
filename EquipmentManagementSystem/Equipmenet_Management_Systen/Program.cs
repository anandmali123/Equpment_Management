using Equipmenet_Management_Systen.Data;
using Equipmenet_Management_Systen.Models;
using Equipmenet_Management_Systen.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceProviderModel = Equipmenet_Management_Systen.Models.ServiceProvider;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IServiceProviderService, ServiceProviderService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await dbContext.Database.MigrateAsync();

        string[] roles =
        {
            "User",
            "Head",
            "MaintenanceIncharge",
            "PlatformHead",
            "PurchaseManager"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var departmentNames = new[] { "Electrical", "Mechanical", "IT", "Production" };

        foreach (var deptName in departmentNames)
        {
            if (!await dbContext.Departments.AnyAsync(d => d.Name == deptName))
            {
                dbContext.Departments.Add(new Department
                {
                    Name = deptName,
                    CreatedDate = DateTime.UtcNow
                });
            }
        }

        if (dbContext.ChangeTracker.HasChanges())
        {
            await dbContext.SaveChangesAsync();
        }

        var departments = await dbContext.Departments.ToDictionaryAsync(d => d.Name, d => d.Id);

        async Task<ApplicationUser> EnsureUserAsync(
            string email,
            string userName,
            string fullName,
            string role,
            int? departmentId,
            string password,
            string? supervisorId = null)
        {
            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser == null)
            {
                existingUser = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = fullName,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    DepartmentId = departmentId,
                    SupervisorId = supervisorId,
                    UserRole = role
                };

                var createResult = await userManager.CreateAsync(existingUser, password);

                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(existingUser, role);
                }
            }
            else
            {
                existingUser.FullName = fullName;
                existingUser.DepartmentId = departmentId;
                existingUser.SupervisorId = supervisorId;
                existingUser.UserRole = role;
                existingUser.IsActive = true;

                await userManager.UpdateAsync(existingUser);

                if (!await userManager.IsInRoleAsync(existingUser, role))
                {
                    await userManager.AddToRoleAsync(existingUser, role);
                }
            }

            return existingUser;
        }

        var platformHead = await EnsureUserAsync(
            "platform@equipmentmanagement.com",
            "platform.head",
            "Platform Head",
            "PlatformHead",
            departments["IT"],
            "Platform@123456");

        var purchaseManager = await EnsureUserAsync(
            "purchase@equipmentmanagement.com",
            "purchase.manager",
            "Purchase Manager",
            "PurchaseManager",
            departments["Mechanical"],
            "Purchase@123456",
            platformHead.Id);

        var maintenanceIncharge = await EnsureUserAsync(
            "maintenance@equipmentmanagement.com",
            "maintenance.incharge",
            "Maintenance Incharge",
            "MaintenanceIncharge",
            departments["Electrical"],
            "Maintenance@123456",
            purchaseManager.Id);

        var headUser = await EnsureUserAsync(
            "head@equipmentmanagement.com",
            "head",
            "Head",
            "Head",
            departments["Production"],
            "Head@123456",
            maintenanceIncharge.Id);

        var normalUser = await EnsureUserAsync(
            "user@equipmentmanagement.com",
            "equipment.user",
            "Equipment User",
            "User",
            departments["Production"],
            "User@123456",
            headUser.Id);

        var itSupport = await EnsureUserAsync(
            "it.support@equipmentmanagement.com",
            "it.support",
            "IT Support",
            "User",
            departments["IT"],
            "Support@123456",
            platformHead.Id);

        var mechanicalUser = await EnsureUserAsync(
            "mechanical.user@equipmentmanagement.com",
            "mechanical.user",
            "Mechanical User",
            "User",
            departments["Mechanical"],
            "Mechanical@123456",
            purchaseManager.Id);

        var electricalUser = await EnsureUserAsync(
            "electrical.user@equipmentmanagement.com",
            "electrical.user",
            "Electrical User",
            "User",
            departments["Electrical"],
            "Electrical@123456",
            maintenanceIncharge.Id);

        if (!await dbContext.ServiceProviders.AnyAsync())
        {
            dbContext.ServiceProviders.AddRange(
                new ServiceProviderModel
                {
                    CompanyName = "Alpha Equipment Services",
                    ContactPerson = "Ravi Sharma",
                    Email = "ravi@alphaes.com",
                    MobileNumber = "+91-9876543210",
                    Address = "Noida, UP",
                    ScopeOfWork = "Installation and maintenance of production machines",
                    SupportedMachines = "Laser Cutter, Mixer, Conveyor",
                    QCProductCertification = "ISO 9001",
                    AmcStartDate = DateTime.UtcNow.AddMonths(-3),
                    AmcEndDate = DateTime.UtcNow.AddYears(1),
                    ServiceType = "AMC",
                    ApprovalStatus = "Approved",
                    ApprovedBy = platformHead.Email,
                    CreatedDate = DateTime.UtcNow
                },
                new ServiceProviderModel
                {
                    CompanyName = "Prime Calibration Labs",
                    ContactPerson = "Simran Kaur",
                    Email = "simran@primelabs.com",
                    MobileNumber = "+91-9812345678",
                    Address = "Gurgaon, HR",
                    ScopeOfWork = "Calibration and preventive maintenance",
                    SupportedMachines = "Printers, CNC Machines, Power Tools",
                    QCProductCertification = "ISO 17025",
                    AmcStartDate = DateTime.UtcNow.AddMonths(-1),
                    AmcEndDate = DateTime.UtcNow.AddMonths(11),
                    ServiceType = "Calibration",
                    ApprovalStatus = "Approved",
                    ApprovedBy = platformHead.Email,
                    CreatedDate = DateTime.UtcNow
                },
                new ServiceProviderModel
                {
                    CompanyName = "QualityPro Services",
                    ContactPerson = "Neha Patel",
                    Email = "info@qualityproservices.com",
                    MobileNumber = "+91-9223344556",
                    Address = "Faridabad, HR",
                    ScopeOfWork = "Quality inspections and preventive maintenance",
                    SupportedMachines = "Compressors, Generators, Pumps",
                    QCProductCertification = "ISO 9001",
                    AmcStartDate = DateTime.UtcNow.AddMonths(-4),
                    AmcEndDate = DateTime.UtcNow.AddYears(1),
                    ServiceType = "Inspection",
                    ApprovalStatus = "Approved",
                    ApprovedBy = platformHead.Email,
                    CreatedDate = DateTime.UtcNow
                },
                new ServiceProviderModel
                {
                    CompanyName = "TechEquip Solutions",
                    ContactPerson = "Amit Desai",
                    Email = "amit@techequip.com",
                    MobileNumber = "+91-9901122334",
                    Address = "Noida, UP",
                    ScopeOfWork = "Equipment installation and spare part support",
                    SupportedMachines = "Forklifts, Conveyors, CNC Machines",
                    QCProductCertification = "ISO 14001",
                    AmcStartDate = DateTime.UtcNow.AddMonths(-2),
                    AmcEndDate = DateTime.UtcNow.AddMonths(10),
                    ServiceType = "Installation",
                    ApprovalStatus = "Pending",
                    CreatedDate = DateTime.UtcNow
                });

            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.Equipments.AnyAsync())
        {
            var alphaProvider = await dbContext.ServiceProviders.FirstOrDefaultAsync(sp => sp.CompanyName == "Alpha Equipment Services");
            var primeProvider = await dbContext.ServiceProviders.FirstOrDefaultAsync(sp => sp.CompanyName == "Prime Calibration Labs");
            var qualityProvider = await dbContext.ServiceProviders.FirstOrDefaultAsync(sp => sp.CompanyName == "QualityPro Services");
            var techProvider = await dbContext.ServiceProviders.FirstOrDefaultAsync(sp => sp.CompanyName == "TechEquip Solutions");

            dbContext.Equipments.AddRange(
                new Equipment
                {
                    EquipmentName = "Laser Cutter",
                    EquipmentCode = "LC-100",
                    Description = "High precision laser cutting machine",
                    SerialNumber = "LC100-01",
                    ManufacturerName = "LaserTech",
                    ManufacturerModel = "LTX-1000",
                    Supplier = "Alpha Equipment Services",
                    DepartmentId = departments["Production"],
                    PurchaseDate = DateTime.UtcNow.AddYears(-1),
                    InstallationDate = DateTime.UtcNow.AddYears(-1).AddMonths(1),
                    CalibrationDate = DateTime.UtcNow.AddMonths(-2),
                    NextCalibrationDate = DateTime.UtcNow.AddMonths(10),
                    CalibrationFrequency = "Annual",
                    ServiceType = "AMC",
                    AssignedUserId = normalUser.Id,
                    ServiceProviderId = alphaProvider?.Id,
                    Status = "Active",
                    Location = "Production Floor",
                    Remarks = "Requires quarterly checks",
                    Value = 125000.00m,
                    WarrantyExpiryDate = DateTime.UtcNow.AddYears(2),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = purchaseManager.Id
                },
                new Equipment
                {
                    EquipmentName = "3D Printer",
                    EquipmentCode = "3DP-210",
                    Description = "Industrial grade 3D printer",
                    SerialNumber = "3DP210-09",
                    ManufacturerName = "PrintWorks",
                    ManufacturerModel = "PW-210",
                    Supplier = "Prime Calibration Labs",
                    DepartmentId = departments["IT"],
                    PurchaseDate = DateTime.UtcNow.AddYears(-2),
                    InstallationDate = DateTime.UtcNow.AddYears(-2).AddMonths(2),
                    CalibrationDate = DateTime.UtcNow.AddMonths(-1),
                    NextCalibrationDate = DateTime.UtcNow.AddMonths(11),
                    CalibrationFrequency = "Annual",
                    ServiceType = "Calibration",
                    AssignedUserId = itSupport.Id,
                    ServiceProviderId = primeProvider?.Id,
                    Status = "Active",
                    Location = "Design Lab",
                    Remarks = "Routine calibration pending",
                    Value = 85000.00m,
                    WarrantyExpiryDate = DateTime.UtcNow.AddYears(1),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = platformHead.Id
                },
                new Equipment
                {
                    EquipmentName = "Forklift",
                    EquipmentCode = "FLK-721",
                    Description = "Heavy-duty warehouse forklift",
                    SerialNumber = "FLK721-34",
                    ManufacturerName = "MoveMax",
                    ManufacturerModel = "MMX-72",
                    Supplier = "Alpha Equipment Services",
                    DepartmentId = departments["Electrical"],
                    PurchaseDate = DateTime.UtcNow.AddYears(-3),
                    InstallationDate = DateTime.UtcNow.AddYears(-3).AddMonths(1),
                    CalibrationDate = DateTime.UtcNow.AddMonths(-5),
                    NextCalibrationDate = DateTime.UtcNow.AddMonths(7),
                    CalibrationFrequency = "Semi-Annual",
                    ServiceType = "Maintenance",
                    AssignedUserId = electricalUser.Id,
                    ServiceProviderId = alphaProvider?.Id,
                    Status = "Active",
                    Location = "Warehouse",
                    Remarks = "Battery replaced recently",
                    Value = 55000.00m,
                    WarrantyExpiryDate = DateTime.UtcNow.AddYears(1),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = maintenanceIncharge.Id
                },
                new Equipment
                {
                    EquipmentName = "Air Compressor",
                    EquipmentCode = "AC-450",
                    Description = "Industrial air compressor for shop operations",
                    SerialNumber = "AC450-18",
                    ManufacturerName = "CompressAir",
                    ManufacturerModel = "CA-450",
                    Supplier = "QualityPro Services",
                    DepartmentId = departments["Mechanical"],
                    PurchaseDate = DateTime.UtcNow.AddYears(-2),
                    InstallationDate = DateTime.UtcNow.AddYears(-2).AddMonths(1),
                    CalibrationDate = DateTime.UtcNow.AddMonths(-3),
                    NextCalibrationDate = DateTime.UtcNow.AddMonths(9),
                    CalibrationFrequency = "Annual",
                    ServiceType = "Inspection",
                    AssignedUserId = mechanicalUser.Id,
                    ServiceProviderId = qualityProvider?.Id,
                    Status = "Active",
                    Location = "Mechanical Workshop",
                    Remarks = "Check regulator valves every quarter",
                    Value = 48000.00m,
                    WarrantyExpiryDate = DateTime.UtcNow.AddYears(1),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = purchaseManager.Id
                },
                new Equipment
                {
                    EquipmentName = "UPS System",
                    EquipmentCode = "UPS-315",
                    Description = "Uninterruptible power supply for server room",
                    SerialNumber = "UPS315-22",
                    ManufacturerName = "PowerSafe",
                    ManufacturerModel = "PS-315",
                    Supplier = "TechEquip Solutions",
                    DepartmentId = departments["IT"],
                    PurchaseDate = DateTime.UtcNow.AddYears(-1),
                    InstallationDate = DateTime.UtcNow.AddYears(-1).AddMonths(2),
                    CalibrationDate = DateTime.UtcNow.AddMonths(-6),
                    NextCalibrationDate = DateTime.UtcNow.AddMonths(6),
                    CalibrationFrequency = "Semi-Annual",
                    ServiceType = "Installation",
                    AssignedUserId = itSupport.Id,
                    ServiceProviderId = techProvider?.Id,
                    Status = "Active",
                    Location = "Server Room",
                    Remarks = "Battery checks due next month",
                    Value = 62000.00m,
                    WarrantyExpiryDate = DateTime.UtcNow.AddYears(2),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = platformHead.Id
                });

            await dbContext.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration or seeding");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();