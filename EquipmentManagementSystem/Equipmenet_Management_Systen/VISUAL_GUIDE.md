# 📊 Visual Guide - Role-Based System

## System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    Equipment Management System                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  Public Access          Authenticated Users      Admin Only       │
│  ┌──────────────┐       ┌──────────────────┐   ┌──────────────┐  │
│  │ Register     │──────▶│ Dashboard (Role) │   │ User Mgmt    │  │
│  │ (All Roles)  │       │ (Customized)     │   │ (Admin Only) │  │
│  └──────────────┘       └──────────────────┘   └──────────────┘  │
│       │                        │                      │            │
│  Login Page              Role Specific             Change          │
│                          Data View              Roles              │
│                          Buttons                Manage             │
│                          Stats                  Users              │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
```

---

## User Registration Flow

```
START
  │
  ▼
┌──────────────────┐
│ Registration     │
│ Page             │
└──────────────────┘
  │
  ▼ (Validate Form)
┌──────────────────┐      Role Description
│ Check Email      │      ┌──────────────────┐
│ Unique?          │◀─────│ Shows when user   │
└──────────────────┘      │ selects role      │
  │ Yes                    └──────────────────┘
  ▼
┌──────────────────┐
│ Create User      │
│ in Database      │
└──────────────────┘
  │
  ▼
┌──────────────────┐
│ Assign Role      │
│ (Identity)       │
└──────────────────┘
  │
  ▼
┌──────────────────┐
│ Set LastLoginAt  │
│ Update CreatedAt │
└──────────────────┘
  │
  ▼
┌──────────────────┐
│ Auto Sign In     │
│ User             │
└──────────────────┘
  │
  ▼
┌──────────────────┐
│ Redirect to      │
│ Dashboard        │
│ (Role Specific)  │
└──────────────────┘
  │
  ▼
 END
```

---

## Dashboard Views by Role

### 👤 User (Regular User)
```
┌─────────────────────────────────────────┐
│ Dashboard - User View                   │
├─────────────────────────────────────────┤
│                                         │
│ Welcome, John Doe!                      │
│ Role: User                              │
│                                         │
│ ┌─────────────────────────────────────┐ │
│ │ My Equipment                        │ │
│ │ Total: 5  |  Active: 4              │ │
│ └─────────────────────────────────────┘ │
│                                         │
│ ┌─────────────────────────────────────┐ │
│ │ Quick Actions                       │ │
│ │ [View My Equipment]                 │ │
│ │ [Submit Maintenance]                │ │
│ └─────────────────────────────────────┘ │
│                                         │
│ ┌─────────────────────────────────────┐ │
│ │ Recent Requests                     │ │
│ │ - Equipment A - Pending (12/23)     │ │
│ │ - Equipment B - In Progress (12/22) │ │
│ └─────────────────────────────────────┘ │
│                                         │
└─────────────────────────────────────────┘
```

### 🏢 Department Head
```
┌─────────────────────────────────────────┐
│ Dashboard - Department Head View        │
├─────────────────────────────────────────┤
│                                         │
│ Welcome, Manager Name!                  │
│ Role: Department Head                   │
│                                         │
│ ┌──────────┬──────────┬──────────┐     │
│ │ Dept     │ Active   │ Recruited│     │
│ │ Equip: 8 │ Equip: 7 │ Users: 3 │     │
│ └──────────┴──────────┴──────────┘     │
│                                         │
│ ┌─────────────────────────────────────┐ │
│ │ 👥 Your Team                        │ │
│ │ • John Doe (Active)                 │ │
│ │ • Jane Smith (Active)               │ │
│ │ • Mike Johnson (Active)             │ │
│ │ [View all team members →]           │ │
│ └─────────────────────────────────────┘ │
│                                         │
│ ┌─────────────────────────────────────┐ │
│ │ Quick Actions                       │ │
│ │ [Manage Users] [Add Equipment]      │ │
│ │ [Department Equipment] [Maintenance]│ │
│ └─────────────────────────────────────┘ │
│                                         │
└─────────────────────────────────────────┘
```

### 👑 Platform Head
```
┌─────────────────────────────────────────┐
│ Dashboard - Platform Head View          │
├─────────────────────────────────────────┤
│                                         │
│ Welcome, Administrator!                 │
│ Role: Platform Head                     │
│                                         │
│ ┌────────┬────────┬────────┬────────┐   │
│ │ Total  │ Active │ Total  │ Active │   │
│ │ Users  │ Users  │ Equip  │ Equip  │   │
│ │  145   │  132   │  892   │  856   │   │
│ └────────┴────────┴────────┴────────┘   │
│                                         │
│ ┌─────────────────────────────────────┐ │
│ │ Quick Actions                       │ │
│ │ [Manage Users] [View All Equipment] │ │
│ │ [Departments] [System Settings]     │ │
│ └─────────────────────────────────────┘ │
│                                         │
│ ┌─────────────────────────────────────┐ │
│ │ System Activities                   │ │
│ │ • New user: alice@example.com       │ │
│ │ • Role changed: john@example.com    │ │
│ │ • Equipment added: New Analyzer     │ │
│ └─────────────────────────────────────┘ │
│                                         │
└─────────────────────────────────────────┘
```

---

## User Management Page

```
┌───────────────────────────────────────────────────────────────┐
│ 👥 User Management                                  [Search...] │
├───────────────────────────────────────────────────────────────┤
│                                                               │
│ ┌─────────────────────────────────────────────────────────┐  │
│ │ Full Name     │ Email           │ Role        │ Status   │  │
│ ├─────────────────────────────────────────────────────────┤  │
│ │ John Doe      │ john@ex.com     │ User    │ Change │ -  │  │
│ │ Jane Smith    │ jane@ex.com     │ DeptHead│ Change │ ⊘  │  │
│ │ Mike Johnson  │ mike@ex.com     │ User    │ Change │ ⊕  │  │
│ │ Sarah Lee     │ sarah@ex.com    │ Maint   │ Change │ ⊘  │  │
│ └─────────────────────────────────────────────────────────┘  │
│                                                               │
│                    Modal Overlay (When changing role)        │
│                    ┌────────────────────┐                    │
│                    │ Change User Role   │                    │
│                    ├────────────────────┤                    │
│                    │ User: John Doe     │                    │
│                    │ Current: User      │                    │
│                    │                    │                    │
│                    │ New Role:          │                    │
│                    │ [Head ▼] │                    │
│                    │                    │                    │
│                    │ [Cancel] [Change]  │                    │
│                    └────────────────────┘                    │
└───────────────────────────────────────────────────────────────┘
```

---

## Data Access Levels

```
                    System Data
                        │
        ┌───────────────┼───────────────┐
        │               │               │
    All Data        Dept Data      Personal Data
        │               │               │
        ▼               ▼               ▼
    ┌───────┐      ┌───────┐      ┌───────┐
    │ Platform     │ Dept   │      │ User  │
    │ Head         │ Head   │      │       │
    │ Purchase Mgr │ Maint  │      │       │
    │ Maint In     │ Incharge      │       │
    └───────┘      └───────┘      └───────┘
```

---

## Role Assignment Hierarchy

```
                   Platform Head (5)
                          │
                          │ can assign to
                          ▼
                   Purchase Manager (4)
                          │
                          │ can assign to
                          ▼
                  Maintenance Incharge (3)
                          │
                          │ can assign to
                          ▼
                   Department Head (2)
                          │
                          │ can assign to
                          ▼
              User / Client Head (1)
```

**Key**: Higher level can assign to lower level only

---

## Database Schema Changes

```
AspNetUsers Table
┌─────────────────────────────────────┐
│ Existing Columns                    │
├─────────────────────────────────────┤
│ • Id (PK)                           │
│ • UserName                          │
│ • Email                             │
│ • PasswordHash                      │
│ • PhoneNumber                       │
│ • DepartmentId (FK)                 │
│ • IsActive                          │
│ • FullName                          │
├─────────────────────────────────────┤
│ New Columns (Added)                 │
├─────────────────────────────────────┤
│ • CreatedAt (DateTime)              │
│ • LastLoginAt (DateTime?)           │
│ • SupervisorId (VARCHAR) (FK)       │
│ • UserRole (VARCHAR)                │
│ • City (VARCHAR)                    │
└─────────────────────────────────────┘
        │
        │ Foreign Key
        ▼
    ┌─────────────┐
    │ AspNetRoles │
    │ (Identity)  │
    └─────────────┘
```

---

## Authorization Flow

```
User Requests Page
        │
        ▼
┌──────────────────┐
│ Is User          │
│ Authenticated?   │
└──────────────────┘
   No│        │Yes
     │        ▼
     │   ┌──────────────┐
     │   │ Load User    │
     │   │ Roles        │
     │   └──────────────┘
     │        │
     │        ▼
     │   ┌──────────────┐
     │   │ Check        │
     │   │ [Authorize]  │
     │   │ Attribute    │
     │   └──────────────┘
     │   Pass│    │Fail
     │        │    │
     │        ▼    ▼
     │   Page    403
     │   Load    Forbidden
     │    │        │
     ▼    ▼        ▼
   Redirect   Show      Return
   to Login   Page      Error
```

---

## Feature Access Matrix (Visual)

```
┌──────────────────────────────────────────────────┐
│              FEATURE ACCESS                     │
├──────────────────────────────────────────────────┤
│                  U  DH  MI  PM  PH               │
├──────────────────────────────────────────────────┤
│ View Own Data      ✓  ✓   ✓   ✓   ✓             │
│ View Dept Data     -  ✓   ✓   ✓   ✓             │
│ View All Data      -  -   -   -   ✓             │
│ Manage Users       -  ✓   ✓   ✓   ✓             │
│ Recruit Users      -  ✓   ✓   -   ✓             │
│ Change Roles       -  -   ✓   ✓   ✓             │
│ Add Equipment      -  ✓   ✓   -   ✓             │
│ System Settings    -  -   -   -   ✓             │
│                                                  │
│ Legend:                                         │
│ U = User          DH = Dept Head                │
│ MI = Maint Incharge  PM = Purchase Manager      │
│ PH = Platform Head   ✓ = Can Access             │
└──────────────────────────────────────────────────┘
```

---

## Login & Session Flow

```
┌──────────────────┐
│ Login Form       │
│ (Email/Password) │
└──────────────────┘
        │
        ▼
┌──────────────────┐
│ Validate         │
│ Credentials      │
└──────────────────┘
        │
        ├─ Failed ──▶ Show Error
        │
        ├─ Success ──▶ ┌──────────────────┐
        │              │ Load User Roles  │
        │              └──────────────────┘
        │                     │
        │                     ▼
        │              ┌──────────────────┐
        │              │ Update LastLogin │
        │              │ At = Now         │
        │              └──────────────────┘
        │                     │
        │                     ▼
        │              ┌──────────────────┐
        │              │ Create Session/  │
        │              │ Cookie           │
        │              └──────────────────┘
        │                     │
        └─────────────┬───────┘
                      ▼
             ┌──────────────────┐
             │ Redirect to      │
             │ Dashboard        │
             └──────────────────┘
                     │
                     ▼
             ┌──────────────────┐
             │ Load Role-Specific│
             │ Data & Display   │
             └──────────────────┘
                     │
                     ▼
              USER LOGGED IN ✓
```

---

## Data Filtering Examples

```
[User Role]
   │
   ├─ Equipments: WHERE AssignedUserId = UserId
   ├─ Requests: WHERE Equipment.AssignedUserId = UserId
   └─ Activities: WHERE CreatedBy = UserId
        │
        └─ Limited Dashboard View

[Department Head Role]
   │
   ├─ Equipments: WHERE DepartmentId = UserDepartmentId
   ├─ Users: WHERE SupervisorId = UserId OR DepartmentId = UserDepartmentId
   ├─ Requests: WHERE Equipment.DepartmentId = UserDepartmentId
   └─ Activities: WHERE Department = UserDepartment
        │
        └─ Department Dashboard View

[Platform Head Role]
   │
   ├─ Equipments: (No Filter - All)
   ├─ Users: (No Filter - All)
   ├─ Requests: (No Filter - All)
   └─ Activities: (No Filter - All)
        │
        └─ System Dashboard View
```

---

## Typical User Journey

```
DAY 1: Registration
  ├─ New user visits /Account/Register
  ├─ Selects role "Head"
  ├─ System creates user with that role
  └─ Auto-logged in → Department Dashboard

DAY 2: Create Team
  ├─ Department Head → Manage Users
  ├─ Finds existing "User" accounts
  ├─ Changes their SupervisorId
  └─ They now appear in "Your Team"

DAY 3: Manage Equipment
  ├─ Department Head → Equipment List
  ├─ Adds new equipment
  ├─ Assigns to team members
  └─ Updates dashboard stats

DAY 4: Monitor Activity
  ├─ Dashboard shows team activities
  ├─ Maintenance requests visible
  ├─ Can approve/assign
  └─ Activity history tracked
```

---

## File Structure (Role-Based)

```
Pages/
├── Account/
│   ├── Login.cshtml (.cs updated for LastLoginAt)
│   ├── Register.cshtml (.cs & .cshtml updated for roles)
│   └── Logout.cshtml
├── Dashboard/
│   ├── Index.cshtml.cs (REFACTORED for roles)
│   └── Index.cshtml (REDESIGNED for role views)
├── Admin/ (NEW)
│   ├── UserManagement.cshtml.cs (NEW)
│   └── UserManagement.cshtml (NEW)
├── Equipment/
├── Maintenance/
└── Department/

Models/
├── ApplicationUser.cs (UPDATED with roles)
└── ... other models

Data/
└── ApplicationDbContext.cs (UPDATED with role config)
```

---

This visual guide complements the detailed documentation. Use this for quick understanding of flows and hierarchies!
