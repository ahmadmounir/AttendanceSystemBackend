CREATE TABLE [Departments] (
  [id] nvarchar(255) PRIMARY KEY,
  [departmentName] nvarchar(255) UNIQUE NOT NULL
)
GO

CREATE TABLE [JobTitles] (
  [id] nvarchar(255) PRIMARY KEY,
  [titleName] nvarchar(255) UNIQUE NOT NULL,
  [minSalary] decimal,
  [maxSalary] decimal
)
GO

CREATE TABLE [Regions] (
  [id] nvarchar(255) PRIMARY KEY,
  [regionName] nvarchar(255) UNIQUE NOT NULL
)
GO

INSERT INTO [Regions] ([id], [regionName]) VALUES 
('11111111-1111-1111-1111-111111111111', 'Middle East & North Africa (MENA)')
GO

INSERT INTO [Regions] ([id], [regionName]) VALUES 
('22222222-2222-2222-2222-222222222222', 'Europe')
GO

INSERT INTO [Regions] ([id], [regionName]) VALUES 
('33333333-3333-3333-3333-333333333333', 'North America')
GO

INSERT INTO [Regions] ([id], [regionName]) VALUES 
('44444444-4444-4444-4444-444444444444', 'Asia Pacific')
GO

CREATE TABLE [Countries] (
  [id] nvarchar(255) PRIMARY KEY,
  [countryName] nvarchar(255) UNIQUE NOT NULL,
  [regionId] nvarchar(255) NOT NULL
)
GO

INSERT INTO [Countries] ([id], [countryName], [regionId]) VALUES 
('a001-ksa-001', 'Saudi Arabia', '11111111-1111-1111-1111-111111111111'),
('a002-uae-002', 'United Arab Emirates', '11111111-1111-1111-1111-111111111111'),
('a003-egy-003', 'Egypt', '11111111-1111-1111-1111-111111111111'),
('a004-jor-004', 'Jordan', '11111111-1111-1111-1111-111111111111'),
('a005-tur-005', 'Turkey', '11111111-1111-1111-1111-111111111111'),
('a006-qat-006', 'Qatar', '11111111-1111-1111-1111-111111111111'),
('a007-kuw-007', 'Kuwait', '11111111-1111-1111-1111-111111111111'),
('b001-uk-001', 'United Kingdom', '22222222-2222-2222-2222-222222222222'),
('b002-ger-002', 'Germany', '22222222-2222-2222-2222-222222222222'),
('b003-fra-003', 'France', '22222222-2222-2222-2222-222222222222'),
('b004-esp-004', 'Spain', '22222222-2222-2222-2222-222222222222'),
('b005-ita-005', 'Italy', '22222222-2222-2222-2222-222222222222'),
('c001-usa-001', 'United States', '33333333-3333-3333-3333-333333333333'),
('c002-can-002', 'Canada', '33333333-3333-3333-3333-333333333333'),
('d001-jpn-001', 'Japan', '44444444-4444-4444-4444-444444444444'),
('d002-chn-002', 'China', '44444444-4444-4444-4444-444444444444'),
('d003-ind-003', 'India', '44444444-4444-4444-4444-444444444444'),
('d004-sgp-004', 'Singapore', '44444444-4444-4444-4444-444444444444')
GO

CREATE TABLE [Shifts] (
  [id] nvarchar(255) PRIMARY KEY,
  [shiftName] nvarchar(255) NOT NULL,
  [startTime] time NOT NULL,
  [endTime] time NOT NULL,
  [gracePeriodMinutes] int DEFAULT (15)
)
GO

INSERT INTO [Shifts] ([id], [shiftName], [startTime], [endTime], [gracePeriodMinutes])
VALUES 
('d290f1ee-6c54-4b01-90e6-d701748f0851', 'Morning Shift', '08:00:00', '16:00:00', 15)
GO

INSERT INTO [Shifts] ([id], [shiftName], [startTime], [endTime], [gracePeriodMinutes])
VALUES 
('a1b2c3d4-e5f6-7890-1234-56789abcdef0', 'Evening Shift', '16:00:00', '00:00:00', 15)
GO

INSERT INTO [Shifts] ([id], [shiftName], [startTime], [endTime], [gracePeriodMinutes])
VALUES 
('f47ac10b-58cc-4372-a567-0e02b2c3d479', 'Admin Shift', '09:00:00', '17:00:00', 30)
GO

CREATE TABLE [Employees] (
  [id] nvarchar(255) PRIMARY KEY,
  [firstName] nvarchar(255) NOT NULL,
  [lastName] nvarchar(255) NOT NULL,
  [email] nvarchar(255) UNIQUE NOT NULL,
  [phone] nvarchar(255),
  [hireDate] datetime NOT NULL,
  [startDate] date NOT NULL,
  [shiftId] nvarchar(255) NOT NULL,
  [endDate] date,
  [departmentId] nvarchar(255) NOT NULL,
  [jobId] nvarchar(255) NOT NULL,
  [countryId] nvarchar(255),
  [isSystemActive] bit DEFAULT (1)
)
GO

CREATE TABLE [UserRoles] (
  [id] nvarchar(255) PRIMARY KEY,
  [roleName] nvarchar(50) UNIQUE NOT NULL
)
GO

CREATE TABLE [UserAccounts] (
  [id] nvarchar(255) PRIMARY KEY,
  [employeeId] nvarchar(255) UNIQUE NOT NULL,
  [username] nvarchar(255) UNIQUE NOT NULL,
  [password] nvarchar(255),
  [roleId] nvarchar(255)
)
GO

CREATE TABLE [RefreshTokens] (
  [id] nvarchar(255) PRIMARY KEY,
  [userId] nvarchar(255) NOT NULL,
  [token] nvarchar(255) UNIQUE NOT NULL,
  [isActive] bit NOT NULL DEFAULT (1),
  [createdAt] datetime NOT NULL,
  [expiresAt] datetime NOT NULL
)
GO


CREATE TABLE [EmployeeShifts] (
  [employeeId] nvarchar(255) NOT NULL,
  [startDate] date NOT NULL,
  [shiftId] nvarchar(255) NOT NULL,
  [endDate] date,
  PRIMARY KEY ([employeeId], [startDate])
)
GO

CREATE TABLE [AttendanceLogs] (
  [id] nvarchar(255) PRIMARY KEY,
  [employeeId] nvarchar(255) NOT NULL,
  [clockInTime] datetime NOT NULL,
  [clockOutTime] datetime,
  [totalHours] decimal
)
GO

CREATE TABLE [OvertimeRequests] (
  [id] nvarchar(255) PRIMARY KEY,
  [employeeId] nvarchar(255) NOT NULL,
  [requestDate] date,
  [hours] decimal,
  [reason] nvarchar(255),
  [isApproved] nvarchar(255)
)
GO

CREATE TABLE [LeaveTypes] (
  [id] nvarchar(255) PRIMARY KEY,
  [typeName] nvarchar(255) UNIQUE NOT NULL,
  [isPaid] int DEFAULT (1),
  [maxDaysPerYear] int
)
GO

CREATE TABLE [LeaveRequests] (
  [id] nvarchar(255) PRIMARY KEY,
  [employeeId] nvarchar(255) NOT NULL,
  [leaveTypeId] nvarchar(255) NOT NULL,
  [startDate] date NOT NULL,
  [endDate] date NOT NULL,
  [reason] nvarchar(255),
  [status] nvarchar(255) DEFAULT 'Pending'
)
GO

CREATE TABLE [LeaveBalances] (
  [id] nvarchar(255) PRIMARY KEY,
  [employeeId] nvarchar(255) NOT NULL,
  [leaveTypeId] nvarchar(255) NOT NULL,
  [remainingDays] decimal DEFAULT (0),
  [year] int
)
GO


CREATE TABLE [AuditLogs] (
  [id] nvarchar(255) PRIMARY KEY,
  [userId] nvarchar(255),
  [action] nvarchar(255),
  [tableName] nvarchar(255) NOT NULL,
  [timestamp] datetime NOT NULL
)
GO



---  2. Foreign Key Constraints

ALTER TABLE [Employees] ADD FOREIGN KEY ([departmentId]) REFERENCES [Departments] ([id])
GO

ALTER TABLE [Employees] ADD FOREIGN KEY ([jobId]) REFERENCES [JobTitles] ([id])
GO

ALTER TABLE [Employees] ADD FOREIGN KEY ([countryId]) REFERENCES [Countries] ([id])
GO

ALTER TABLE [Countries] ADD FOREIGN KEY ([regionId]) REFERENCES [Regions] ([id])
GO

ALTER TABLE [UserAccounts] ADD FOREIGN KEY ([employeeId]) REFERENCES [Employees] ([id])
GO

ALTER TABLE [RefreshTokens] ADD FOREIGN KEY ([userId]) REFERENCES [UserAccounts] ([id])
GO

ALTER TABLE [EmployeeShifts] ADD FOREIGN KEY ([employeeId]) REFERENCES [Employees] ([id]) ON DELETE CASCADE
GO

ALTER TABLE [EmployeeShifts] ADD FOREIGN KEY ([shiftId]) REFERENCES [Shifts] ([id])
GO

ALTER TABLE [AttendanceLogs] ADD FOREIGN KEY ([employeeId]) REFERENCES [Employees] ([id])
GO

ALTER TABLE [OvertimeRequests] ADD FOREIGN KEY ([employeeId]) REFERENCES [Employees] ([id])
GO

ALTER TABLE [LeaveRequests] ADD FOREIGN KEY ([employeeId]) REFERENCES [Employees] ([id])
GO

ALTER TABLE [LeaveRequests] ADD FOREIGN KEY ([leaveTypeId]) REFERENCES [LeaveTypes] ([id])
GO

ALTER TABLE [LeaveBalances] ADD FOREIGN KEY ([employeeId]) REFERENCES [Employees] ([id])
GO

ALTER TABLE [LeaveBalances] ADD FOREIGN KEY ([leaveTypeId]) REFERENCES [LeaveTypes] ([id])
GO

ALTER TABLE [AuditLogs] ADD FOREIGN KEY ([userId]) REFERENCES [UserAccounts] ([id])
GO

ALTER TABLE [UserAccounts] ADD FOREIGN KEY ([roleId]) REFERENCES [UserRoles] ([id])
GO



-- =============================================
-- 1. (Departments)
-- =============================================
INSERT INTO [Departments] ([id], [departmentName]) VALUES 
('3794CA53-1A31-4D4F-9A73-3FC4865C2CDD', 'Human Resources (HR)'),
('E9E4F16F-9347-4A7E-A0A3-2E4AFF371780', 'Finance & Accounting'),
('C55F0103-24B9-4182-AEE3-BD9534268A50', 'Operations'),
('509C71DD-AA6A-4453-B6B5-4770E15F2829', 'Marketing'),
('687BD2F9-16DB-4479-B9FD-02D758769AFF', 'Information Technology (IT)');

-- =============================================
-- 2. (JobTitles)
-- =============================================
INSERT INTO [JobTitles] ([id], [titleName], [minSalary], [maxSalary]) VALUES 
('6FF6FADF-27C8-4EC4-B086-F57BD99D3221', 'Software Engineer', 12000.00, 22000.00),
('3970BD96-E3E4-4950-AFC6-8ED675D72941', 'HR Manager', 14000.00, 20000.00),
('07F81CE2-81EC-4EA7-BB2D-60E2733C151E', 'Accountant', 9000.00, 16000.00),
('176BE9D5-47C8-407F-A564-C574928BEE8D', 'Sales Representative', 6000.00, 12000.00),
('DDBA1D72-E4DA-4870-9B71-F89464B062C6', 'System Administrator', 15000.00, 25000.00);

-- =============================================
-- 3. (Admin - Employee)
-- =============================================
INSERT INTO [Employees] (
    [id], 
    [firstName], 
    [lastName], 
    [email], 
    [phone], 
    [hireDate], 
    [startDate], 
    [shiftId],
    [endDate], 
    [departmentId], 
    [jobId], 
    [countryId],
    [isSystemActive]
) VALUES (
    '6E94885D-7F4D-4F6D-971E-DFC670A60C1E',
    'admin', 
    '', 
    'admin@company.com', 
    '+905550000000', 
    GETDATE(), 
    GETDATE(), 
    'f47ac10b-58cc-4372-a567-0e02b2c3d479',
    NULL, 
    '687BD2F9-16DB-4479-B9FD-02D758769AFF',
    'DDBA1D72-E4DA-4870-9B71-F89464B062C6',
    'a005-tur-005',
    1
);

-- =============================================
-- 4. (User Account)
-- =============================================
INSERT INTO [UserAccounts] (
    [id], 
    [employeeId], 
    [username], 
    [password], 
    [roleId]
) VALUES (
    '6A785FBF-80DB-4D84-B5C2-0C73158A353D',
    '6E94885D-7F4D-4F6D-971E-DFC670A60C1E',
    'admin@company.com', 
    '123456', 
    'amjifdf2vdfpgm23in2'
);

INSERT INTO [LeaveTypes] ([id], [typeName], [isPaid], [maxDaysPerYear]) VALUES 
(NEWID(), 'Annual Leave', 1, 21),       -- إجازة سنوية (مدفوعة - 21 يوم)
(NEWID(), 'Sick Leave', 1, 14),         -- إجازة مرضية (مدفوعة - 14 يوم)
(NEWID(), 'Unpaid Leave', 0, 30),       -- إجازة بدون راتب (غير مدفوعة - 30 يوم)
(NEWID(), 'Maternity Leave', 1, 90),    -- إجازة أمومة (مدفوعة - 90 يوم)
(NEWID(), 'Paternity Leave', 1, 3),     -- إجازة أبوة (مدفوعة - 3 أيام)
(NEWID(), 'Marriage Leave', 1, 5),      -- إجازة زواج (مدفوعة - 5 أيام)
(NEWID(), 'Bereavement Leave', 1, 3);   -- إجازة وفاة (مدفوعة - 3 أيام)

INSERT INTO [ViolationTypes] ([id], [typeName], [description]) VALUES 
(NEWID(), 'Late Arrival', 'Arriving after the scheduled start time plus the grace period.'),
(NEWID(), 'Early Departure', 'Leaving the workplace before the official shift end time without permission.'),
(NEWID(), 'Unexcused Absence', 'Absence from work without prior approval or a valid medical report.'),
(NEWID(), 'Missed Clock-In/Out', 'Failure to record attendance (check-in or check-out) on the system.'),
(NEWID(), 'Extended Break', 'Exceeding the allowed duration for lunch or rest breaks.'),
(NEWID(), 'Policy Violation', 'General violation of company internal policies or code of conduct.');