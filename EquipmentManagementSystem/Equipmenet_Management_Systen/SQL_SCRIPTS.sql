-- Equipment Management System - MySQL Setup Scripts
-- Run these scripts to manually set up the database if needed

-- ========================================
-- 1. CREATE DATABASE
-- ========================================

CREATE DATABASE IF NOT EXISTS equipmentdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE equipmentdb;

-- ========================================
-- 2. ASSIGN ROLES TO USER (AFTER RUNNING MIGRATIONS)
-- ========================================

-- First, register your account through the web application
-- Then use these scripts to assign roles

-- Find your user ID
SELECT Id, UserName, Email FROM AspNetUsers;

-- Find role IDs
SELECT Id, Name FROM AspNetRoles;

-- Assign Head (Admin) role to your user
-- Replace 'your-user-id' and 'Head-role-id' with actual IDs
-- INSERT INTO AspNetUserRoles (UserId, RoleId) 
-- VALUES ('your-user-id', 'Head-role-id');

-- ========================================
-- 3. CREATE INITIAL DEPARTMENTS (OPTIONAL)
-- ========================================

INSERT INTO Departments (Name, CreatedDate) VALUES 
('Manufacturing', NOW()),
('Quality Assurance', NOW()),
('Maintenance', NOW()),
('Laboratory', NOW());

-- ========================================
-- 4. CREATE SAMPLE SERVICE PROVIDERS (OPTIONAL)
-- ========================================

INSERT INTO ServiceProviders 
(CompanyName, ContactPerson, MobileNumber, Email, Address, ScopeOfWork, ServiceType, ApprovalStatus, CreatedDate)
VALUES 
('Precision Calibration Services', 'John Smith', '+1-234-567-8900', 'john@precision-cal.com', '123 Tech Street, City', 'Calibration Services', 'Calibration', 'Approved', NOW()),
('Maintenance Plus', 'Sarah Johnson', '+1-234-567-8901', 'sarah@maintenance-plus.com', '456 Service Ave, City', 'Equipment Maintenance', 'Preventive Maintenance', 'Approved', NOW()),
('Complete Equipment Solutions', 'Michael Brown', '+1-234-567-8902', 'michael@equipment-sol.com', '789 Industrial Rd, City', 'All Services', 'Both Calibration & Maintenance', 'Pending', NOW());

-- ========================================
-- 5. CREATE SAMPLE EQUIPMENT (OPTIONAL)
-- ========================================

-- Note: Replace department IDs (1, 2, 3) with actual IDs from your Departments table

INSERT INTO Equipment 
(EquipmentName, SerialNumber, Manufacturer, Supplier, DepartmentId, InstallationDate, 
 CalibrationDate, NextCalibrationDate, CalibrationFrequency, ServiceType, Status, CreatedDate)
VALUES 
('Digital Pressure Gauge', 'DPG-2024-001', 'Calibration Labs Inc', 'Tech Supplies Ltd', 1, '2024-01-15', '2024-06-01', '2024-07-01', 'Monthly', 'Calibration', 'Active', NOW()),
('Temperature Controller', 'TC-2024-002', 'Control Systems Corp', 'Industrial Supply', 2, '2024-02-10', '2024-05-15', '2024-08-15', 'Quarterly', 'Both Calibration & Maintenance', 'Active', NOW()),
('Oscilloscope', 'OSC-2024-003', 'ElectroTech', 'Tech Supplies Ltd', 3, '2024-03-05', '2024-04-01', '2025-04-01', 'Yearly', 'Calibration', 'Calibration Due', NOW()),
('Flow Meter', 'FM-2024-004', 'Precision Instruments', 'Industrial Supply', 1, '2023-08-20', '2024-03-01', '2024-09-01', 'Half-Yearly', 'Preventive Maintenance', 'Active', NOW()),
('Multimeter', 'MM-2024-005', 'Fluke Corporation', 'Tech Supplies Ltd', 2, '2024-04-12', '2024-06-15', '2024-07-15', 'Monthly', 'Calibration', 'Active', NOW());

-- ========================================
-- 6. USEFUL QUERIES
-- ========================================

-- Get all equipment due for calibration (next 30 days)
SELECT 
    EquipmentName, 
    SerialNumber, 
    NextCalibrationDate,
    DATEDIFF(NextCalibrationDate, NOW()) as DaysUntilDue,
    Status
FROM Equipment
WHERE NextCalibrationDate <= DATE_ADD(NOW(), INTERVAL 30 DAY)
  AND NextCalibrationDate > NOW()
ORDER BY NextCalibrationDate ASC;

-- Get overdue equipment
SELECT 
    EquipmentName,
    SerialNumber,
    NextCalibrationDate,
    DATEDIFF(NOW(), NextCalibrationDate) as DaysOverdue,
    Status
FROM Equipment
WHERE NextCalibrationDate < NOW()
ORDER BY NextCalibrationDate ASC;

-- Get equipment with service provider assigned
SELECT 
    e.EquipmentName,
    e.SerialNumber,
    sp.CompanyName as ServiceProvider,
    e.NextCalibrationDate
FROM Equipment e
LEFT JOIN ServiceProviders sp ON e.ServiceProviderId = sp.Id
WHERE e.ServiceProviderId IS NOT NULL;

-- Get approved service providers
SELECT 
    CompanyName,
    ContactPerson,
    Email,
    MobileNumber,
    ServiceType
FROM ServiceProviders
WHERE ApprovalStatus = 'Approved'
ORDER BY CompanyName;

-- Get pending service provider approvals
SELECT 
    CompanyName,
    ContactPerson,
    Email,
    CreatedDate
FROM ServiceProviders
WHERE ApprovalStatus = 'Pending'
ORDER BY CreatedDate DESC;

-- Count equipment by department
SELECT 
    d.Name as Department,
    COUNT(e.Id) as TotalEquipment,
    SUM(CASE WHEN e.Status = 'Active' THEN 1 ELSE 0 END) as ActiveCount,
    SUM(CASE WHEN e.NextCalibrationDate < NOW() THEN 1 ELSE 0 END) as OverdueCount
FROM Departments d
LEFT JOIN Equipment e ON d.Id = e.DepartmentId
GROUP BY d.Id, d.Name;

-- Count equipment by status
SELECT 
    Status,
    COUNT(Id) as Count
FROM Equipment
GROUP BY Status
ORDER BY Count DESC;

-- Get equipment by calibration frequency
SELECT 
    CalibrationFrequency,
    COUNT(Id) as Count,
    AVG(DATEDIFF(NextCalibrationDate, NOW())) as AvgDaysUntilDue
FROM Equipment
WHERE Status = 'Active'
GROUP BY CalibrationFrequency;

-- Get maintenance requests summary
SELECT 
    mr.Status,
    COUNT(mr.Id) as Count,
    e.EquipmentName,
    mr.RequestDate
FROM MaintenanceRequests mr
LEFT JOIN Equipment e ON mr.EquipmentId = e.Id
GROUP BY mr.Status, e.EquipmentName, mr.RequestDate
ORDER BY mr.RequestDate DESC;

-- Get users by role
SELECT 
    u.UserName,
    u.Email,
    u.FullName,
    d.Name as Department,
    r.Name as Role
FROM AspNetUsers u
LEFT JOIN Departments d ON u.DepartmentId = d.Id
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
ORDER BY u.UserName;

-- ========================================
-- 7. MAINTENANCE SCRIPTS
-- ========================================

-- Update next calibration dates (useful if you change frequency)
-- WARNING: Be careful with this script!
UPDATE Equipment
SET NextCalibrationDate = DATE_ADD(CalibrationDate, INTERVAL 1 MONTH)
WHERE CalibrationFrequency = 'Monthly' AND NextCalibrationDate < DATE_ADD(NOW(), INTERVAL 5 DAY);

-- Mark equipment as overdue if calibration date passed
-- Note: Actually marking as "Calibration Due" status if overdue
UPDATE Equipment
SET Status = 'Calibration Due'
WHERE NextCalibrationDate < NOW() AND Status = 'Active';

-- Archive old maintenance records (keep last 2 years)
-- DELETE FROM MaintenanceRequests 
-- WHERE CreatedDate < DATE_SUB(NOW(), INTERVAL 2 YEAR);

-- Cleanup old calibration history (keep last 3 years)
-- DELETE FROM CalibrationHistory
-- WHERE CreatedDate < DATE_SUB(NOW(), INTERVAL 3 YEAR);

-- ========================================
-- 8. DATABASE OPTIMIZATION
-- ========================================

-- Create indexes for better performance
CREATE INDEX idx_equipment_status ON Equipment(Status);
CREATE INDEX idx_equipment_nextcal ON Equipment(NextCalibrationDate);
CREATE INDEX idx_equipment_dept ON Equipment(DepartmentId);
CREATE INDEX idx_maintenance_status ON MaintenanceRequests(Status);
CREATE INDEX idx_maintenance_equip ON MaintenanceRequests(EquipmentId);
CREATE INDEX idx_provider_status ON ServiceProviders(ApprovalStatus);

-- Check table sizes
SELECT 
    table_name,
    ROUND(((data_length + index_length) / 1024 / 1024), 2) as size_in_mb
FROM information_schema.tables 
WHERE table_schema = 'equipmentdb'
ORDER BY (data_length + index_length) DESC;

-- ========================================
-- 9. BACKUP SCRIPT (Run via mysqldump command line)
-- ========================================

-- Command line backup:
-- mysqldump -u root -p equipmentdb > backup_equipmentdb.sql

-- Command line restore:
-- mysql -u root -p equipmentdb < backup_equipmentdb.sql

-- ========================================
-- 10. VERIFICATION QUERIES
-- ========================================

-- Verify database is set up correctly
SELECT 'Database' as Type, COUNT(*) as Count FROM information_schema.TABLES WHERE table_schema = 'equipmentdb';

-- Count records in each table
SELECT 
    'Users' as Table_Name, COUNT(*) as Record_Count FROM AspNetUsers
UNION ALL
SELECT 'Roles', COUNT(*) FROM AspNetRoles
UNION ALL
SELECT 'Departments', COUNT(*) FROM Departments
UNION ALL
SELECT 'Equipment', COUNT(*) FROM Equipment
UNION ALL
SELECT 'ServiceProviders', COUNT(*) FROM ServiceProviders
UNION ALL
SELECT 'MaintenanceRequests', COUNT(*) FROM MaintenanceRequests
UNION ALL
SELECT 'CalibrationHistory', COUNT(*) FROM CalibrationHistory;

-- ========================================
-- 11. RESET DATABASE (CAREFUL!)
-- ========================================

-- Only run this if you want to completely reset the database
-- WARNING: This will delete all data!

-- DROP DATABASE equipmentdb;
-- CREATE DATABASE equipmentdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
-- USE equipmentdb;
-- Then run 'dotnet ef database update' from command line

-- ========================================
-- 12. AUDIT QUERIES
-- ========================================

-- Find equipment modified in last 7 days (if audit logging is implemented)
-- SELECT * FROM Equipment WHERE CreatedDate >= DATE_SUB(NOW(), INTERVAL 7 DAY);

-- Find users created in last 30 days
SELECT UserName, Email, CreatedDate FROM AspNetUsers WHERE CreatedDate >= DATE_SUB(NOW(), INTERVAL 30 DAY);

-- Find pending approvals
SELECT 'Service Providers' as Type, COUNT(*) as Count FROM ServiceProviders WHERE ApprovalStatus = 'Pending'
UNION ALL
SELECT 'Maintenance Requests', COUNT(*) FROM MaintenanceRequests WHERE Status = 'Pending';

-- ========================================
-- 13. REPORTING QUERIES
-- ========================================

-- Monthly calibration summary
SELECT 
    YEAR(NextCalibrationDate) as Year,
    MONTH(NextCalibrationDate) as Month,
    COUNT(*) as DueForCalibration,
    d.Name as Department
FROM Equipment e
JOIN Departments d ON e.DepartmentId = d.Id
WHERE NextCalibrationDate > NOW() AND NextCalibrationDate <= DATE_ADD(NOW(), INTERVAL 90 DAY)
GROUP BY YEAR(NextCalibrationDate), MONTH(NextCalibrationDate), d.Name
ORDER BY Year, Month;

-- Equipment age analysis
SELECT 
    d.Name as Department,
    COUNT(*) as TotalEquipment,
    AVG(DATEDIFF(NOW(), InstallationDate)) as AvgAgeInDays,
    MIN(InstallationDate) as OldestInstallation,
    MAX(InstallationDate) as NewestInstallation
FROM Equipment e
JOIN Departments d ON e.DepartmentId = d.Id
GROUP BY d.Id, d.Name;

-- Service provider performance
SELECT 
    sp.CompanyName,
    COUNT(e.Id) as EquipmentAssigned,
    COUNT(mr.Id) as MaintenanceRequests,
    sp.ApprovalStatus
FROM ServiceProviders sp
LEFT JOIN Equipment e ON sp.Id = e.ServiceProviderId
LEFT JOIN MaintenanceRequests mr ON sp.Id = mr.AssignedProviderId
GROUP BY sp.Id, sp.CompanyName, sp.ApprovalStatus
ORDER BY EquipmentAssigned DESC;

-- ========================================
-- Notes:
-- 1. Run migrations first: dotnet ef database update
-- 2. These scripts are optional enhancements
-- 3. Customize department and equipment names for your company
-- 4. Always backup before running DELETE or UPDATE queries
-- 5. Test queries on a copy before running on production
-- ========================================
