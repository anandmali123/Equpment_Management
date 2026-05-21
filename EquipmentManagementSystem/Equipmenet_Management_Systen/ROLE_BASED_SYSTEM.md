# Role-Based Access Control System - Documentation

## Overview
The Equipment Management System now has a comprehensive role-based access control system with 6 distinct user roles, each with specific permissions and dashboard views.

## User Roles & Responsibilities

### 1. **User** (Regular Equipment User)
- **Access Level**: Limited (Personal Equipment Only)
- **Permissions**:
  - View assigned equipment
  - Submit maintenance requests
  - View their own maintenance requests
  - Access personal dashboard with limited data

- **Dashboard Shows**:
  - Total assigned equipment count
  - Active equipment count
  - Recent maintenance requests

---

### 2. **Department Head**
- **Access Level**: Department Level
- **Permissions**:
  - Recruit and manage users within department
  - View all department equipment
  - Add/edit/delete equipment
  - Manage maintenance requests for department
  - Assign equipment to users

- **Dashboard Shows**:
  - Department equipment statistics
  - Active equipment count
  - Recruited users team list
  - Recent maintenance activities

- **User Management**:
  - Can manage users within their department
  - Can recruit new users and assign them to the department

---

### 3. **Platform Head** (Administrator)
- **Access Level**: Full System Access
- **Permissions**:
  - View all system users and equipment
  - Manage all user roles and permissions
  - Access all departments
  - Full control over all equipment and maintenance
  - Create new departments
  - Access complete analytics

- **Dashboard Shows**:
  - Total users count
  - Active users count
  - Total equipment count
  - Active equipment count
  - Full system statistics

---

### 4. **Maintenance Incharge**
- **Access Level**: Recruitment & Oversight
- **Permissions**:
  - Recruit users for the platform
  - Monitor Platform Head activities
  - Oversee all platform operations
  - Access all maintenance request data

- **Dashboard Shows**:
  - Total recruited users
  - Total system users
  - Active users count
  - Team members list
  - All maintenance activities

---

### 5. **Purchase Manager**
- **Access Level**: Monitoring & Control
- **Permissions**:
  - Monitor all platform head activities
  - Monitor maintenance incharge activities
  - Control maintenance incharge operations
  - Access complete activity logs
  - Oversee all business processes

- **Dashboard Shows**:
  - Total users count
  - Active users count
  - Pending maintenance count
  - Incharge users list
  - All recent activities

---

### 6. **Client Head**
- **Access Level**: Limited (Client Specific)
- **Permissions**:
  - View client-specific equipment and data
  - Limited to client operations

---

## Registration Process

When a user registers:
1. They select a user role from the available options (excluding Platform Head)
2. A role description is displayed to help them choose
3. Upon successful registration, they're automatically:
   - Assigned the selected role
   - Logged in
   - Redirected to their role-specific dashboard

## Database Changes

The following columns were added to the `AspNetUsers` table:
- `CreatedAt` (DateTime) - User creation timestamp
- `SupervisorId` (VARCHAR) - Foreign key to supervisor/manager user
- `UserRole` (VARCHAR) - Stores the primary role as string
- `City` (VARCHAR) - User's city
- `LastLoginAt` (DateTime) - Last login timestamp

## User Management Page

Located at: `/Admin/UserManagement`

**Accessible to**: Platform Head, Purchase Manager, Maintenance Incharge

**Features**:
- View all users (filtered based on role hierarchy)
- Change user roles
- Activate/Deactivate users
- Search users
- View user details and creation date

**Role Hierarchy** (for role assignment):
```
Platform Head (Level 5)
  ↓
Purchase Manager (Level 4)
  ↓
Maintenance Incharge (Level 3)
  ↓
Department Head (Level 2)
  ↓
User/Client Head (Level 1)
```

Users can only assign roles to users below their level in the hierarchy.

## Dashboard Customization

Each role sees a customized dashboard with:
- Role-appropriate statistics
- Relevant quick action buttons
- Team member information (for managers)
- Role-specific navigation options

## Authorization Attributes

All protected pages use `[Authorize(Roles = "...")]` attribute to restrict access.

Example:
```csharp
[Authorize(Roles = "PlatformHead,PurchaseManager,MaintenanceIncharge")]
public class UserManagementModel : PageModel { ... }
```

## API Responses & Data Filtering

The system filters data based on the current user's role:
- **Platform Head**: Sees all data
- **Purchase Manager**: Sees users and maintenance data
- **Maintenance Incharge**: Sees department-level data
- **Department Head**: Sees only department data
- **User**: Sees only their own equipment

## Security Features

1. **Role-based access control** - Every page checks user roles
2. **Supervisor tracking** - Users can be assigned to supervisors
3. **Activity logging** - User actions are logged
4. **Soft deletion** - Users can be deactivated instead of deleted
5. **Role hierarchy** - Higher roles can manage lower roles only

## Future Enhancements

Consider implementing:
- Role-based API endpoints
- Advanced permission management
- Role delegation
- Activity audit logs with detailed tracking
- Role-specific report generation
