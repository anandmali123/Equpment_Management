# Equipment Calibration & Maintenance Management System

A comprehensive ASP.NET Core web application for managing equipment calibration, maintenance schedules, and service providers with role-based access control.

## System Overview

This system is designed to help companies manage their equipment lifecycles, track calibration schedules, manage maintenance requests, and coordinate with service providers.

## Technology Stack

- **Framework**: ASP.NET Core 9.0
- **Language**: C#
- **Database**: MySQL 8.0+
- **ORM**: Entity Framework Core 9.0
- **Frontend**: Bootstrap 5, HTML5, JavaScript
- **Authentication**: ASP.NET Identity
- **Email**: SMTP (Gmail or any SMTP server)

## Features

### 1. Dashboard
- Real-time KPI cards showing:
  - Total Equipment Count
  - Due Calibration (Next 30 days)
  - Overdue Equipment
  - Active Service Providers
  - Pending Maintenance Requests
- Recent activity feed
- Quick action links

### 2. Equipment Management
- Add, Edit, Delete equipment records
- Track calibration dates and next due dates
- Auto-calculate next calibration dates based on frequency
- Equipment status tracking (Active, Under Maintenance, Calibration Due, Inactive, Scrap)
- Link equipment to service providers and departments
- Assign equipment to users

### 3. Service Provider Management
- Maintain service provider database
- Track AMC (Annual Maintenance Contract) dates
- Manage approval workflow:
  - Purchase Manager → Adds Provider
  - Maintenance Head → Reviews
  - Client Head → Approves
- Support for multiple service types

### 4. Calibration Management
- Automatic calibration frequency calculation
- Monthly, Quarterly, Half-Yearly, Yearly, and Custom frequencies
- Overdue alerts and reminders
- Calibration history tracking
- Certificate upload support (future enhancement)

### 5. User Management
- Create and manage users
- Assign users to departments
- Role-based access control

### 6. Department Management
- Create and manage departments
- Track equipment per department
- Department-wise user assignments

## User Roles & Permissions

| Role | Permissions |
|------|-------------|
| **User** | View assigned equipment, update status, raise maintenance requests, view calibration schedule |
| **Department Head** | All User permissions + Add/Update/Delete users, view department equipment, approve maintenance |
| **Maintenance Incharge** | Add/Update service providers, assign providers to equipment, schedule maintenance, monitor calibration |
| **Platform Head** | Add/Update/Delete equipment, view all departments, generate reports, full equipment management |
| **Purchase Manager** | Approve service providers, manage AMC/calibration agreements |
| **Client Head / Super Admin** | Full system access, permission management, user role assignment, audit logs |

## Prerequisites

- .NET 9 SDK or later
- MySQL Server 8.0 or later
- Visual Studio 2022 or Visual Studio Code

## Installation & Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd Equipmenet_Management_Systen
```

### 2. Database Setup

#### Option A: Using MySQL
```bash
# Install MySQL Server from https://dev.mysql.com/downloads/mysql/

# Create database
mysql -u root -p
mysql> CREATE DATABASE equipmentdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
mysql> EXIT;
```

#### Option B: Docker MySQL (Recommended)
```bash
docker run --name mysql-eqms -e MYSQL_ROOT_PASSWORD=123456 -e MYSQL_DATABASE=equipmentdb -p 3306:3306 -d mysql:8.0
```

### 3. Update Connection String
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=equipmentdb;user=root;password=123456"
  }
}
```

### 4. Email Configuration
Update `appsettings.json` with your SMTP settings:
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "FromEmail": "your-email@gmail.com",
    "FromPassword": "your-app-password"
  }
}
```

**For Gmail**: 
- Enable 2-Factor Authentication
- Generate an App Password: https://myaccount.google.com/apppasswords
- Use the 16-character password in `FromPassword`

### 5. Install Dependencies
```bash
dotnet restore
```

### 6. Run Database Migrations
```bash
# Create initial migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

Roles will be automatically created on first run.

### 7. Run the Application
```bash
dotnet run
```

The application will be available at: `https://localhost:5001` or `http://localhost:5000`

## First Time Setup

### 1. Create Admin User
1. Navigate to the application
2. Click **Register**
3. Create your account
4. Exit and log back in as the registered user

### 2. Manually Assign Super Admin Role (Via Database)
```sql
-- Using MySQL Command Line
USE equipmentdb;

-- Find your user's ID
SELECT Id, UserName FROM AspNetUsers WHERE UserName = 'your-username';

-- Get Head role ID
SELECT Id FROM AspNetRoles WHERE Name = 'Head';

-- Insert the role assignment
INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES ('<your-user-id>', '<Head-role-id>');
```

Or use a tool like MySQL Workbench or Navicat to perform the same operation.

### 3. Create Test Data
- Create Departments: Dashboard → User Management → Departments
- Create Users: Dashboard → Users
- Add Equipment: Dashboard → Equipment
- Add Service Providers: Dashboard → Service Providers

## Project Structure

```
EquipmentManagementSystem/
├── Controllers/           # MVC Controllers
├── Models/               # Data Models
├── Views/                # Razor Views
├── Services/             # Business Logic
├── Data/                 # Database Context & Migrations
├── wwwroot/              # Static Files
├── appsettings.json      # Configuration
└── Program.cs            # Application Entry Point
```

## Key Models

### Equipment
- Equipment metadata (Name, Serial Number, Manufacturer, etc.)
- Calibration tracking (Dates, Frequency, Next Due)
- Service provider assignment
- Status management

### ServiceProvider
- Company information
- Contact details
- AMC dates
- Approval workflow status

### CalibrationHistory
- Historical calibration records
- Certificate tracking
- Performance metrics

### MaintenanceRequest
- Request tracking
- Status management
- Service provider assignment
- Completion dates

## API Endpoints

The application uses MVC Controllers. Key endpoints:

```
Dashboard:
  GET  / → Home
  GET  /dashboard → Main Dashboard
  GET  /dashboard/due-soon → Due Calibrations
  GET  /dashboard/overdue → Overdue Equipment

Equipment:
  GET    /equipment → List all equipment
  GET    /equipment/create → Create form
  POST   /equipment/create → Save equipment
  GET    /equipment/edit/{id} → Edit form
  POST   /equipment/edit → Update equipment
  POST   /equipment/delete/{id} → Delete equipment

Service Providers:
  GET    /serviceprovider → List providers
  GET    /serviceprovider/create → Create form
  POST   /serviceprovider/create → Save provider
  GET    /serviceprovider/edit/{id} → Edit form
  POST   /serviceprovider/approve/{id} → Approve provider

Users:
  GET    /usermanagement → List users
  GET    /usermanagement/edit/{id} → Edit form
  POST   /usermanagement/edit → Update user

Departments:
  GET    /department → List departments
  GET    /department/create → Create form
  POST   /department/create → Save department
```

## Authentication Flow

1. User registers/logs in via ASP.NET Identity
2. User is assigned roles (User, Head, etc.)
3. Controllers check `[Authorize]` and `[Authorize(Roles = "...")]` attributes
4. Sidebar and menus dynamically show based on user roles

## Development Notes

### Automatic Calibration Date Calculation
Next calibration dates are automatically calculated based on frequency:
- Monthly: +1 month
- Quarterly: +3 months
- Half-Yearly: +6 months
- Yearly: +1 year

### Database Backup
```bash
# Backup MySQL Database
mysqldump -u root -p equipmentdb > backup.sql

# Restore MySQL Database
mysql -u root -p equipmentdb < backup.sql
```

### Troubleshooting

#### Connection String Issues
```
Error: Unable to connect to MySQL
Solution: Check your MySQL server is running
- Windows: Services → MySQL80 → Start
- Docker: docker start mysql-eqms
```

#### Migration Issues
```bash
# Drop all tables and recreate
dotnet ef database drop --force
dotnet ef database update
```

#### Role-Based Access Denied
```
Error: Access Denied for this page
Solution: Ensure user has correct role assigned in AspNetUserRoles table
```

## Security Considerations

1. **Password Storage**: Uses ASP.NET Identity's bcrypt hashing
2. **CSRF Protection**: All forms include anti-forgery tokens
3. **SQL Injection**: Uses parameterized queries via Entity Framework
4. **Authentication**: Requires login for all protected pages
5. **Authorization**: Role-based access control implemented

## Performance Optimization

1. **Database Queries**: Uses `AsNoTracking()` for read-only operations
2. **Pagination**: Can be added for large data sets
3. **Caching**: Can be implemented for frequently accessed data
4. **Indexes**: MySQL tables have primary keys and foreign key indexes

## Future Enhancements

- [ ] Calibration Calendar (Interactive UI)
- [ ] Reports Export (PDF/Excel)
- [ ] QR Code Equipment Tracking
- [ ] Audit Logs
- [ ] WhatsApp Notifications
- [ ] AMC Tracking & Renewal Alerts
- [ ] Mobile App (React Native/Flutter)
- [ ] Approval Workflow (Multi-step approvals)
- [ ] REST API
- [ ] Real-time Notifications (SignalR)
- [ ] Dashboard Analytics (Charts.js)
- [ ] File Upload for Certificates

## Deployment

### Local IIS Deployment
```bash
# Publish application
dotnet publish -c Release -o ./publish

# Copy files to IIS physical path
Copy-Item -Path ./publish/* -Destination "C:\inetpub\wwwroot\EquipmentManagementSystem" -Recurse -Force
```

### Cloud Deployment (Azure)
```bash
# Create Azure App Service
az appservice plan create -n eqms-plan -g MyResourceGroup --sku B1

# Deploy from GitHub
az webapp up -n eqms-app -g MyResourceGroup --plan eqms-plan
```

### Docker Deployment
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 as runtime
WORKDIR /app
COPY ./publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "Equipmenet_Management_Systen.dll"]
```

```bash
docker build -t eqms:latest .
docker run -p 8080:80 --name eqms-container eqms:latest
```

## Support & Documentation

- **Official Docs**: https://learn.microsoft.com/aspnet/core/
- **Entity Framework**: https://learn.microsoft.com/ef/core/
- **Bootstrap 5**: https://getbootstrap.com/docs/5.0/
- **MySQL Docs**: https://dev.mysql.com/doc/

## License

This project is provided as-is for educational and business purposes.

## Contact

For support or questions, please contact the development team.

---

**Version**: 1.0.0  
**Last Updated**: 2026  
**Status**: Production Ready
