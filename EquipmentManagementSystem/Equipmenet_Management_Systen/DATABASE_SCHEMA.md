    # Equipment Management System - Database Schema

    ## Database: equipmentdb

    ### Overview
    MySQL database with 7 main tables plus ASP.NET Identity tables for authentication and authorization.

    ---

    ## Tables

    ### 1. AspNetUsers (ASP.NET Identity)
    **Purpose**: Store user accounts and credentials

    **Columns**:
    | Column | Type | Notes |
    |--------|------|-------|
    | Id | nvarchar(450) | Primary Key, Unique User ID |
    | UserName | nvarchar(256) | Login Username |
    | NormalizedUserName | nvarchar(256) | Indexed for searches |
    | Email | nvarchar(256) | User Email |
    | NormalizedEmail | nvarchar(256) | Indexed for searches |
    | EmailConfirmed | bit | Is email verified |
    | PasswordHash | nvarchar(max) | Bcrypt hashed password |
    | SecurityStamp | nvarchar(max) | Security token |
    | ConcurrencyStamp | nvarchar(max) | Concurrency control |
    | PhoneNumber | nvarchar(max) | User phone |
    | PhoneNumberConfirmed | bit | Is phone verified |
    | TwoFactorEnabled | bit | 2FA enabled |
    | LockoutEnd | datetimeoffset | Account lockout expiry |
    | LockoutEnabled | bit | Allow lockouts |
    | AccessFailedCount | int | Failed login attempts |
    | **FullName** | longtext | Extended: User's full name |
    | **DepartmentId** | int | Extended: Foreign key to Department |
    | **CreatedDate** | datetime(6) | Extended: Account creation date |
    | **IsActive** | bit | Extended: Account status |

    **Indexes**: 
    - Primary Key: Id
    - Unique: NormalizedUserName, NormalizedEmail
    - Foreign Key: DepartmentId → Departments

    **Extended Properties**: FullName, DepartmentId, CreatedDate, IsActive

    ---

    ### 2. AspNetRoles (ASP.NET Identity)
    **Purpose**: Define available roles in the system

    **Columns**:
    | Column | Type | Notes |
    |--------|------|-------|
    | Id | nvarchar(450) | Primary Key |
    | Name | nvarchar(256) | Role name (e.g., "PlatformHead") |
    | NormalizedName | nvarchar(256) | Uppercase version for searching |
    | ConcurrencyStamp | nvarchar(max) | Version control |

    **Predefined Roles**:
    1. User
    2. Head
    3. MaintenanceIncharge
    4. PlatformHead
    5. PurchaseManager
    6. Head

    ---

    ### 3. AspNetUserRoles (ASP.NET Identity)
    **Purpose**: Many-to-Many relationship between Users and Roles

    **Columns**:
    | Column | Type | Notes |
    |--------|------|-------|
    | UserId | nvarchar(450) | Primary Key + Foreign Key |
    | RoleId | nvarchar(450) | Primary Key + Foreign Key |

    **Indexes**: 
    - Primary Key: (UserId, RoleId)
    - Foreign Keys: UserId → AspNetUsers, RoleId → AspNetRoles

    **Sample Data**:
    ```sql
    -- User john@example.com has roles User and Head
    INSERT INTO AspNetUserRoles (UserId, RoleId) 
    VALUES ('user-id-123', 'user-role-id');
    INSERT INTO AspNetUserRoles (UserId, RoleId) 
    VALUES ('user-id-123', 'depthead-role-id');
    ```

    ---

    ### 4. Departments
    **Purpose**: Organize equipment and users by department

    **Columns**:
    | Column | Type | Constraints | Notes |
    |--------|------|-----------|-------|
    | Id | int | PRIMARY KEY, AUTO_INCREMENT | Unique department ID |
    | Name | longtext | NOT NULL | Department name |
    | CreatedDate | datetime(6) | NOT NULL | Creation timestamp |

    **Relationships**:
    - Has Many: Users (AspNetUsers.DepartmentId)
    - Has Many: Equipment (Equipment.DepartmentId)

    **Sample Data**:
    ```sql
    INSERT INTO Departments (Name, CreatedDate) 
    VALUES ('Manufacturing', NOW());
    INSERT INTO Departments (Name, CreatedDate) 
    VALUES ('Quality Assurance', NOW());
    INSERT INTO Departments (Name, CreatedDate) 
    VALUES ('Maintenance', NOW());
    ```

    ---

    ### 5. ServiceProviders
    **Purpose**: Maintain service provider/vendor information

    **Columns**:
    | Column | Type | Constraints | Notes |
    |--------|------|-----------|-------|
    | Id | int | PRIMARY KEY, AUTO_INCREMENT | Unique provider ID |
    | CompanyName | longtext | NOT NULL | Vendor company name |
    | ContactPerson | longtext | | Main contact person |
    | MobileNumber | longtext | | Contact phone |
    | Email | longtext | | Contact email |
    | Address | longtext | | Company address |
    | ScopeOfWork | longtext | | Services offered |
    | SupportedMachines | longtext | | Equipment types supported |
    | QCProductCertification | longtext | | Certifications held |
    | AmcStartDate | datetime(6) | NULLABLE | AMC contract start |
    | AmcEndDate | datetime(6) | NULLABLE | AMC contract expiry |
    | ServiceType | longtext | | Service category |
    | ApprovalStatus | longtext | | Status: Pending/Approved/Rejected |
    | ApprovedBy | longtext | | User who approved |
    | CreatedDate | datetime(6) | | Registration date |

    **Relationships**:
    - Has Many: Equipment (Equipment.ServiceProviderId)
    - Has Many: MaintenanceRequests (MaintenanceRequest.AssignedProviderId)

    **Approval Workflow**:
    1. Purchase Manager creates provider (ApprovalStatus = 'Pending')
    2. Maintenance Incharge reviews details
    3. Client Head approves (ApprovalStatus = 'Approved')
    4. Can now be assigned to equipment

    **Sample Data**:
    ```sql
    INSERT INTO ServiceProviders 
    (CompanyName, ContactPerson, MobileNumber, Email, ServiceType, ApprovalStatus, CreatedDate)
    VALUES 
    ('Calibration Services Ltd', 'John Smith', '+1234567890', 'john@cal.com', 'Calibration', 'Approved', NOW());
    ```

    ---

    ### 6. Equipment
    **Purpose**: Main equipment master data

    **Columns**:
    | Column | Type | Constraints | Notes |
    |--------|------|-----------|-------|
    | Id | int | PRIMARY KEY, AUTO_INCREMENT | Unique equipment ID |
    | EquipmentName | longtext | NOT NULL | Equipment description |
    | SerialNumber | longtext | | Manufacturer serial number |
    | Manufacturer | longtext | | Equipment manufacturer |
    | Supplier | longtext | | Purchase supplier |
    | DepartmentId | int | FOREIGN KEY | Department this equipment belongs to |
    | InstallationDate | datetime(6) | | When installed |
    | CalibrationDate | datetime(6) | | Last calibration date |
    | NextCalibrationDate | datetime(6) | | When next calibration is due |
    | CalibrationFrequency | longtext | | Monthly/Quarterly/Yearly/etc |
    | ServiceType | longtext | | Calibration/Maintenance/Both |
    | AssignedUserId | nvarchar(450) | NULLABLE FOREIGN KEY | User responsible |
    | ServiceProviderId | int | NULLABLE FOREIGN KEY | Current service provider |
    | Status | longtext | | Active/Under Maintenance/Inactive/Scrap |
    | MachineLocation | longtext | | Physical location in factory |
    | Remarks | longtext | | Additional notes |
    | CreatedDate | datetime(6) | | Record creation date |

    **Relationships**:
    - Many-to-One: Department (DepartmentId)
    - Many-to-One: User (AssignedUserId)
    - Many-to-One: ServiceProvider (ServiceProviderId)
    - Has Many: CalibrationHistory (CalibrationHistory.EquipmentId)
    - Has Many: MaintenanceRequests (MaintenanceRequest.EquipmentId)

    **Statuses**:
    - Active
    - Under Maintenance
    - Calibration Due
    - Inactive
    - Scrap

    **Calibration Frequencies**:
    - Monthly (30 days)
    - Quarterly (90 days)
    - Half-Yearly (180 days)
    - Yearly (365 days)
    - Custom

    **Indexes**:
    - Primary Key: Id
    - Foreign Keys: DepartmentId, AssignedUserId, ServiceProviderId
    - Optional Index: Status, NextCalibrationDate (for quick filtering)

    **Sample Data**:
    ```sql
    INSERT INTO Equipment 
    (EquipmentName, SerialNumber, Manufacturer, DepartmentId, InstallationDate, 
     CalibrationDate, NextCalibrationDate, CalibrationFrequency, Status, CreatedDate)
    VALUES 
    ('Digital Pressure Gauge', 'SN-2024-001', 'Calibration Labs Inc', 1, 
     '2024-01-15', '2024-06-01', '2024-07-01', 'Monthly', 'Active', NOW());
    ```

    ---

    ### 7. CalibrationHistory
    **Purpose**: Track all calibration activities and history

    **Columns**:
    | Column | Type | Constraints | Notes |
    |--------|------|-----------|-------|
    | Id | int | PRIMARY KEY, AUTO_INCREMENT | Unique record ID |
    | EquipmentId | int | FOREIGN KEY, NOT NULL | Reference to Equipment |
    | CalibrationDate | datetime(6) | | When calibration was performed |
    | NextDueDate | datetime(6) | | When next calibration is due |
    | PerformedBy | nvarchar(450) | NULLABLE FOREIGN KEY | User who performed |
    | CertificateFile | longtext | | Path to calibration certificate |
    | Remarks | longtext | | Notes from calibration |
    | CreatedDate | datetime(6) | | Record creation timestamp |

    **Relationships**:
    - Many-to-One: Equipment (EquipmentId)
    - Many-to-One: User (PerformedBy)

    **Usage**:
    - Store historical calibration records
    - Track calibration certificates
    - Generate compliance reports
    - Audit trail

    **Sample Data**:
    ```sql
    INSERT INTO CalibrationHistory 
    (EquipmentId, CalibrationDate, NextDueDate, PerformedBy, Remarks, CreatedDate)
    VALUES 
    (1, '2024-06-01 10:00:00', '2024-07-01', 'user-id-123', 'Within tolerance', NOW());
    ```

    ---

    ### 8. MaintenanceRequests
    **Purpose**: Track maintenance and repair requests

    **Columns**:
    | Column | Type | Constraints | Notes |
    |--------|------|-----------|-------|
    | Id | int | PRIMARY KEY, AUTO_INCREMENT | Unique request ID |
    | EquipmentId | int | FOREIGN KEY, NOT NULL | Equipment needing maintenance |
    | RequestedBy | nvarchar(450) | FOREIGN KEY | User who raised request |
    | AssignedProviderId | int | NULLABLE FOREIGN KEY | Service provider assigned |
    | RequestDate | datetime(6) | | When request was created |
    | Status | longtext | | Pending/In Progress/Completed |
    | EstimationAmount | decimal(18,2) | NULLABLE | Estimated cost |
    | CompletionDate | datetime(6) | NULLABLE | When maintenance completed |
    | RequestType | longtext | | Breakdown/Preventive/Calibration |
    | Description | longtext | | Maintenance description |
    | CreatedDate | datetime(6) | | Record creation date |

    **Relationships**:
    - Many-to-One: Equipment (EquipmentId)
    - Many-to-One: User (RequestedBy)
    - Many-to-One: ServiceProvider (AssignedProviderId)

    **Status Workflow**:
    1. Pending (Just created)
    2. In Progress (Assigned to provider, work started)
    3. Completed (Work finished, equipment returned)

    **Request Types**:
    - Breakdown Maintenance (Equipment failed)
    - Preventive Maintenance (Scheduled maintenance)
    - Calibration (Recalibration needed)

    **Sample Data**:
    ```sql
    INSERT INTO MaintenanceRequests 
    (EquipmentId, RequestedBy, RequestType, Status, Description, RequestDate, CreatedDate)
    VALUES 
    (1, 'user-id-456', 'Breakdown Maintenance', 'Pending', 'Gauge not reading correctly', NOW(), NOW());
    ```

    ---

    ## Relationships Diagram

    ```
    AspNetUsers (1) ──→ (Many) AspNetUserRoles
                        ↓
                    AspNetRoles

    AspNetUsers
        ├─ (1) → (1) Departments (via DepartmentId)
        └─ (1) → (Many) MaintenanceRequests (as RequestedBy)
        └─ (1) → (Many) CalibrationHistory (as PerformedBy)

    Departments
        └─ (1) → (Many) Equipment

    Equipment
        ├─ (1) → (1) Department
        ├─ (1) → (1) ServiceProvider
        ├─ (1) → (1) User (AssignedUserId)
        ├─ (1) → (Many) CalibrationHistory
        └─ (1) → (Many) MaintenanceRequests

    ServiceProviders
        ├─ (1) → (Many) Equipment
        └─ (1) → (Many) MaintenanceRequests
    ```

    ---

    ## Key Queries

    ### Get All Equipment Due for Calibration (Next 30 Days)
    ```sql
    SELECT e.* FROM Equipment e
    WHERE e.NextCalibrationDate <= DATE_ADD(NOW(), INTERVAL 30 DAY)
      AND e.NextCalibrationDate > NOW()
    ORDER BY e.NextCalibrationDate ASC;
    ```

    ### Get Overdue Equipment
    ```sql
    SELECT e.* FROM Equipment e
    WHERE e.NextCalibrationDate < NOW()
    ORDER BY e.NextCalibrationDate ASC;
    ```

    ### Get Equipment with Service Provider Assigned
    ```sql
    SELECT e.EquipmentName, sp.CompanyName 
    FROM Equipment e
    LEFT JOIN ServiceProviders sp ON e.ServiceProviderId = sp.Id;
    ```

    ### Get User's Assigned Equipment
    ```sql
    SELECT e.* FROM Equipment e
    WHERE e.AssignedUserId = 'user-id-123'
    ORDER BY e.EquipmentName;
    ```

    ### Get Department Equipment Summary
    ```sql
    SELECT d.Name,
           COUNT(e.Id) as TotalEquipment,
           SUM(CASE WHEN e.Status = 'Active' THEN 1 ELSE 0 END) as ActiveCount,
           SUM(CASE WHEN e.NextCalibrationDate < NOW() THEN 1 ELSE 0 END) as OverdueCount
    FROM Departments d
    LEFT JOIN Equipment e ON d.Id = e.DepartmentId
    GROUP BY d.Id, d.Name;
    ```

    ### Get Pending Maintenance Requests
    ```sql
    SELECT mr.*, e.EquipmentName, u.UserName, sp.CompanyName
    FROM MaintenanceRequests mr
    JOIN Equipment e ON mr.EquipmentId = e.Id
    JOIN AspNetUsers u ON mr.RequestedBy = u.Id
    LEFT JOIN ServiceProviders sp ON mr.AssignedProviderId = sp.Id
    WHERE mr.Status = 'Pending'
    ORDER BY mr.RequestDate DESC;
    ```

    ### Get Pending Service Provider Approvals
    ```sql
    SELECT sp.* FROM ServiceProviders sp
    WHERE sp.ApprovalStatus = 'Pending'
    ORDER BY sp.CreatedDate DESC;
    ```

    ---

    ## Backup & Restore

    ### Backup Database
    ```bash
    # Full database backup
    mysqldump -u root -p equipmentdb > backup_equipmentdb.sql

    # Backup with timestamp
    mysqldump -u root -p equipmentdb > backup_equipmentdb_$(date +%Y%m%d_%H%M%S).sql
    ```

    ### Restore Database
    ```bash
    # Restore from backup
    mysql -u root -p equipmentdb < backup_equipmentdb.sql

    # Restore to different database
    mysql -u root -p newdatabase < backup_equipmentdb.sql
    ```

    ### Scheduled Backups (Linux/Mac)
    ```bash
    # Add to crontab (daily at 2 AM)
    0 2 * * * mysqldump -u root -p'password' equipmentdb > /backups/eq_$(date +\%Y\%m\%d).sql
    ```

    ---

    ## Performance Considerations

    ### Recommended Indexes
    ```sql
    -- Equipment filters
    CREATE INDEX idx_equipment_status ON Equipment(Status);
    CREATE INDEX idx_equipment_nextcal ON Equipment(NextCalibrationDate);
    CREATE INDEX idx_equipment_dept ON Equipment(DepartmentId);
    CREATE INDEX idx_equipment_assigned ON Equipment(AssignedUserId);

    -- Maintenance queries
    CREATE INDEX idx_maintenance_status ON MaintenanceRequests(Status);
    CREATE INDEX idx_maintenance_equip ON MaintenanceRequests(EquipmentId);

    -- Service Provider
    CREATE INDEX idx_provider_status ON ServiceProviders(ApprovalStatus);

    -- Calibration History
    CREATE INDEX idx_calibration_equip ON CalibrationHistory(EquipmentId);
    CREATE INDEX idx_calibration_date ON CalibrationHistory(CalibrationDate);
    ```

    ### Query Optimization Tips
    1. Always filter by Status and NextCalibrationDate for equipment queries
    2. Use LIMIT for large result sets (pagination)
    3. Use JOINs instead of multiple queries
    4. Create indexes on frequently filtered columns

    ---

    ## Data Validation Rules

    ### Equipment
    - EquipmentName: Required, max 255 chars
    - SerialNumber: Optional, unique recommended
    - CalibrationDate: Required, must be before NextCalibrationDate
    - Status: Must be one of predefined values
    - CalibrationFrequency: Must match calculation logic

    ### ServiceProvider
    - CompanyName: Required, max 255 chars
    - Email: Optional, valid email format
    - MobileNumber: Optional, valid format
    - ApprovalStatus: Must be Pending/Approved/Rejected

    ### MaintenanceRequest
    - EquipmentId: Required, must exist
    - RequestedBy: Required, must exist user
    - Status: Must be Pending/In Progress/Completed
    - RequestType: Must match category

    ---

    ## Database Size Estimate

    With 1000 equipment records:
    - AspNetUsers: ~1 MB (50-100 users)
    - Equipment: ~2-3 MB
    - CalibrationHistory: ~5-10 MB (5-10 records per equipment)
    - MaintenanceRequests: ~3-5 MB (3-5 requests per equipment)
    - **Total**: ~15-25 MB

    Scalable to millions of records with proper indexing.

    ---

    ## Version History

    - **v1.0** (Initial Release)
      - Core tables implemented             
      - ASP.NET Identity integration
      - Basic relationships established
      - Calibration tracking
      - Maintenance request workflow

    ---

    This schema supports the complete equipment management lifecycle from acquisition to maintenance and eventual decommissioning.
