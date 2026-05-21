# Role-Based System - Quick Reference Guide

## Role Access Matrix

| Feature | User | DeptHead | MaintIncharge | PurchaseMgr | PlatformHead | Head |
|---------|------|----------|---------------|-------------|--------------|-----------|
| View Own Equipment | ✅ | ✅ | ✅ | - | ✅ | ✅ |
| View Dept Equipment | - | ✅ | ✅ | - | ✅ | - |
| View All Equipment | - | - | - | - | ✅ | - |
| Manage Users | - | ✅ | ✅ | ✅ | ✅ | - |
| Recruit Users | - | ✅ | ✅ | - | ✅ | - |
| Change User Roles | - | - | ✅ | ✅ | ✅ | - |
| View All Users | - | - | ✅ | ✅ | ✅ | - |
| Create Equipment | - | ✅ | ✅ | - | ✅ | - |
| Manage Maintenance | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| View Reports | - | - | ✅ | ✅ | ✅ | - |
| System Settings | - | - | - | - | ✅ | - |

## Dashboard Features by Role

### 👤 User (Regular User)
- Card: Personal Equipment (Total)
- Card: Active Equipment
- Card: Recent Requests
- Table: Recent activities on their equipment
- Quick actions: View my equipment, Maintenance requests

### 🏢 Department Head
- Card: Department Equipment
- Card: Active Equipment  
- Card: Recruited Users
- Card: Recent Maintenance
- Table: Team members list with status
- Quick actions: Manage users, Add equipment, Department equipment

### 🔧 Maintenance Incharge
- Card: Total Recruited Users
- Card: System Users
- Card: Active Users
- Table: Team members and their status
- Table: Recent activities
- Quick actions: Manage users, View departments

### 💼 Purchase Manager
- Card: Total Users
- Card: Active Users
- Card: Pending Maintenance
- Card: Incharge users to monitor
- Table: Recent activities
- Quick actions: Manage users, View departments

### 👑 Platform Head
- Card: Total Users
- Card: Active Users
- Card: Total Equipment
- Card: Active Equipment
- Table: Recent activities across system
- Quick actions: Manage users, View all equipment, View departments

### 🤝 Client Head
- Limited access to client-specific data
- Can view assigned equipment
- Can submit maintenance requests

## How to Change User Roles

### Method 1: User Management Page
1. Go to Dashboard → Click "👥 Manage Users"
2. Find user in list
3. Click "Change Role" button
4. Select new role from dropdown
5. Click "Change Role" to confirm

### Method 2: User Registration
1. Go to `/Account/Register`
2. Fill in user details
3. Select role during registration
4. New user gets that role automatically

## Role Hierarchy (For Assignment)

```
Level 5: Platform Head ⭐ (Can assign to all)
   ↓
Level 4: Purchase Manager (Can assign to levels 1-3)
   ↓
Level 3: Maintenance Incharge (Can assign to levels 1-2)
   ↓
Level 2: Department Head (Can assign to level 1)
   ↓
Level 1: User/Client Head (Cannot assign roles)
```

**Rule**: A user can only assign roles to users below their level.

## Data Filtering Rules

### User Role
- Sees only their assigned equipment
- Sees only their maintenance requests

### Department Head
- Sees department equipment
- Sees users supervised by them
- Sees department maintenance requests

### Maintenance Incharge
- Sees users they supervise
- Sees all system users
- Sees all maintenance requests

### Purchase Manager
- Sees all users
- Sees all maintenance requests
- Sees all activities

### Platform Head
- Sees everything
- No data restrictions

## Dashboard URL Mapping

| Role | Primary Dashboard |
|------|------------------|
| Any | `/Dashboard/Index` |
| Admin/Manager | `/Admin/UserManagement` |

## Permission Hierarchy

```
Public Pages (No Auth Required)
├── Login Page
├── Register Page
└── Home Page

Authenticated Pages (Any User)
├── Dashboard
├── Equipment (filtered by role)
├── Maintenance Requests
└── Profile Management

Admin Pages (Specific Roles)
├── User Management (PlatformHead, PurchaseManager, MaintenanceIncharge)
├── System Settings (PlatformHead)
└── Reports (PlatformHead, PurchaseManager)
```

## Common User Flows

### 1. Register as Department Head
```
Register → Select "Head" → Confirm Registration → Department Dashboard
```

### 2. Recruit Users (as Department Head)
```
Dashboard → Manage Users → Search user → Assign to your department → 
User now appears in "Your Team" section
```

### 3. Manage Maintenance (as User)
```
Dashboard → Submit Request → Wait for approval → View status in Recent Activities
```

### 4. Monitor Activities (as Purchase Manager)
```
Dashboard → View all pending items → Monitor maintenance activities → 
View team performance
```

## Quick Commands in URLs

| URL | Accessible By | Purpose |
|-----|---------------|---------|
| `/Dashboard/Index` | All users | Main dashboard |
| `/Admin/UserManagement` | High-level roles | Manage users |
| `/Equipment/Index` | All users | View equipment |
| `/Account/Register` | Public | Register new user |
| `/Account/Login` | Public | Login |
| `/Account/Logout` | Authenticated | Logout |

## Testing Checklist

- [ ] Register with "User" role - verify limited dashboard
- [ ] Register with "Head" - verify department view + team section
- [ ] Login as PlatformHead - verify User Management access
- [ ] Try changing another user's role - verify hierarchy enforced
- [ ] Deactivate user - verify badge changes to "Inactive"
- [ ] Login as different roles - verify different dashboard stats
- [ ] Try accessing protected pages without authentication - verify redirect to login
- [ ] Try accessing admin pages with regular user - verify forbidden/redirect

## Common Customizations

### To Hide Features from Certain Roles:
```razor
@if (Model.UserRoles.Contains("PlatformHead"))
{
    <div>This only shows for Platform Heads</div>
}
```

### To Check Multiple Roles:
```csharp
if (userRoles.Contains("PlatformHead") || userRoles.Contains("PurchaseManager"))
{
    // Can access feature
}
```

### To Restrict Page Access:
```csharp
[Authorize(Roles = "PlatformHead,PurchaseManager")]
public class MyPageModel : PageModel { }
```

---

**Last Updated**: Implementation Complete
**Status**: Ready for Testing
**Support**: See ROLE_BASED_SYSTEM.md for detailed documentation
