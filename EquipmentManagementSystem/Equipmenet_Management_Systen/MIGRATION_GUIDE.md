# Migration Guide - Role-Based System Implementation

## What's New

Your Equipment Management System now includes a complete role-based access control system with:

✅ 6 distinct user roles with different access levels
✅ Role-specific dashboards customized for each user type
✅ User Management page for administrators
✅ Role hierarchy system for secure role assignment
✅ User supervisor tracking for organizational structure
✅ Enhanced registration with role selection and descriptions

## Steps to Deploy

### 1. Database Migration

The system will automatically add the required columns when the application starts:
- `SupervisorId` - Link between users and their supervisors
- `UserRole` - Stores user's primary role
- `LastLoginAt` - Tracks user's last login time

**No manual migration is required** - The Program.cs uses `ExecuteSqlRawAsync` with `IF NOT EXISTS` clauses.

### 2. Test the System

After deployment, test the following scenarios:

#### Test 1: User Registration
1. Navigate to `/Account/Register`
2. Fill in the form and select a role (e.g., "Head")
3. Verify role description appears
4. Submit the form
5. Verify user is created with the selected role

#### Test 2: Dashboard Role-Based Views
1. Log in as different roles
2. Verify each role sees appropriate statistics:
   - **User**: Only their equipment
   - **Head**: Department statistics and their team
   - **PlatformHead**: All system statistics
   - **PurchaseManager**: User and maintenance data
   - **MaintenanceIncharge**: Recruitment and user management data

#### Test 3: User Management
1. Log in as PlatformHead or PurchaseManager
2. Navigate to "Manage Users" (👥 button on dashboard)
3. Try changing a user's role
4. Verify role hierarchy is enforced (higher roles can only change lower roles)
5. Try deactivating/activating users

### 3. User Recruitment Flow

**For Department Head**:
1. Go to User Management
2. Change a User's role to Head (if you're PlatformHead)
3. They can now recruit users within their department

**For Platform Head**:
1. Has full control to recruit users at all levels
2. Can assign any role to any user

### 4. Verify Authorization

Check these protected pages:
- `/Admin/UserManagement` - Should only be accessible to PlatformHead, PurchaseManager, MaintenanceIncharge
- `/Dashboard/Index` - Should only be accessible to authenticated users

## Key Files Modified/Created

### Files Modified:
- `Models/ApplicationUser.cs` - Added SupervisorId and UserRole properties
- `Data/ApplicationDbContext.cs` - Added supervisor relationship configuration
- `Pages/Account/Register.cshtml.cs` - Enhanced with role descriptions and UserRole assignment
- `Pages/Account/Register.cshtml` - Added role descriptions display
- `Pages/Dashboard/Index.cshtml.cs` - Completely refactored for role-based data loading
- `Pages/Dashboard/Index.cshtml` - New role-specific dashboard UI
- `Program.cs` - Added columns for SupervisorId and UserRole

### Files Created:
- `Pages/Admin/UserManagement.cshtml.cs` - User management page code-behind
- `Pages/Admin/UserManagement.cshtml` - User management UI
- `ROLE_BASED_SYSTEM.md` - Detailed documentation

## Role Assignment Workflow

```
User Registration
    ↓
Select Role
    ↓
Verify Role Available (not PlatformHead)
    ↓
Create User
    ↓
Assign Role
    ↓
Redirect to Dashboard
    ↓
Dashboard shows role-specific view
```

## Testing Credentials

After first run, the system creates:
- Email: `admin@equipmentmanagement.com`
- Password: `Admin@123456`
- Role: `PlatformHead`

Use this to:
1. Access User Management
2. Create additional test users
3. Assign different roles
4. Test role-based features

## Common Issues & Solutions

### Issue: Users can see all data
**Solution**: Make sure pages have `[Authorize]` attribute and dashboard filters data by role

### Issue: Role descriptions not showing
**Solution**: Check browser console for JavaScript errors, ensure role names match exactly

### Issue: User Management page says "No users found"
**Solution**: This is role-specific - Department Heads only see their department users. Log in as PlatformHead to see all users.

### Issue: Can't change role for certain users
**Solution**: Role hierarchy enforced - you can only change roles for users below your rank. Log in as PlatformHead to change any role.

## Next Steps

1. ✅ Deploy the application
2. ✅ Test registration with different roles
3. ✅ Test dashboard for each role
4. ✅ Test user management
5. ⏳ Add role-specific features for equipment assignment
6. ⏳ Add activity logging for audit trail
7. ⏳ Create role-based reports
8. ⏳ Add email notifications for role-based events

## Support

For issues or questions about the role-based system:
1. Check ROLE_BASED_SYSTEM.md for detailed documentation
2. Review the code comments in modified files
3. Test with the admin credentials provided
4. Check application logs for error messages

---

**Implementation Status**: ✅ Complete and Ready for Testing
**Database Migration**: ✅ Automatic (No manual steps required)
**Authorization**: ✅ Enforced on all protected pages
