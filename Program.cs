using AttendanceSystemBackend.Repositories.Employees;
using AttendanceSystemBackend.Repositories.Regions;
using AttendanceSystemBackend.Repositories.Countries;
using AttendanceSystemBackend.Repositories.UserRoles;
using AttendanceSystemBackend.Repositories.Departments;
using AttendanceSystemBackend.Repositories.Auth;
using AttendanceSystemBackend.Repositories.AuditLogs;
using AttendanceSystemBackend.Repositories.Notifications;
using AttendanceSystemBackend.Repositories.LeaveBalances;
using AttendanceSystemBackend.Repositories.LeaveRequests;
using AttendanceSystemBackend.Repositories.LeaveTypes;
using AttendanceSystemBackend.Repositories.Shifts;
using AttendanceSystemBackend.Repositories.EmployeeShifts;
using AttendanceSystemBackend.Repositories.ViolationTypes;
using AttendanceSystemBackend.Repositories.Violations;
using AttendanceSystemBackend.Repositories.OvertimeRequests;
using AttendanceSystemBackend.Services.Auth;
using AttendanceSystemBackend.Services.LeaveRequests;
using AttendanceSystemBackend.Services.Authorization;
using AttendanceSystemBackend.Middleware;
using AttendanceSystemBackend.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;
using System.Text;
using AttendanceSystemBackend.Repositories.JobTitles;
using Dapper;
using AttendanceSystemBackend.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register Dapper handlers to map SQL `time` (TimeSpan) to .NET `TimeOnly` / `TimeOnly?`
SqlMapper.AddTypeHandler(new DapperTimeOnlyTypeHandler());
SqlMapper.AddTypeHandler(typeof(TimeOnly?), new DapperNullableTimeOnlyTypeHandler());

// Configure to always use port 5079
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenLocalhost(5079);
});

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero
    };
});

// Add services to the container.
builder.Services.AddScoped<IEmployeesRepo, EmployeesRepo>();
builder.Services.AddScoped<IRegionsRepo, RegionsRepo>();
builder.Services.AddScoped<ICountriesRepo, CountriesRepo>();
builder.Services.AddScoped<IUserRolesRepo, UserRolesRepo>();
builder.Services.AddScoped<IDepartmentsRepo, DepartmentsRepo>();
builder.Services.AddScoped<IJobTitlesRepo, JobTitlesRepo>();
builder.Services.AddScoped<IAuthRepo, AuthRepo>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
builder.Services.AddScoped<IAuditLogsRepo, AuditLogsRepo>();
builder.Services.AddScoped<INotificationsRepo, NotificationsRepo>();
builder.Services.AddScoped<ILeaveBalancesRepo, LeaveBalancesRepo>();
builder.Services.AddScoped<ILeaveRequestsRepo, LeaveRequestsRepo>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<ILeaveTypesRepo, LeaveTypesRepo>();
builder.Services.AddScoped<IShiftsRepo, ShiftsRepo>();
builder.Services.AddScoped<IEmployeeShiftsRepo, EmployeeShiftsRepo>();
builder.Services.AddScoped<IViolationTypesRepo, ViolationTypesRepo>();
builder.Services.AddScoped<IViolationsRepo, ViolationsRepo>();
builder.Services.AddScoped<IOvertimeRequestsRepo, OvertimeRequestsRepo>();

// Configure CORS to allow requests from frontend ports 3000-3003
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "http://localhost:3001",
            "http://localhost:3002",
            "http://localhost:3003"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.AddControllers(options =>
{
    // Configure global route prefix for all API controllers
    options.UseGeneralRoutePrefix("api/v1");
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Attendance System API";
        document.Info.Version = "v1";
        document.Info.Description = @"
# Attendance System - Complete API Documentation

## ?? Authentication

All endpoints (except login, refresh, and logout) require JWT authentication.

### How to Authenticate:
1. Call `/api/v1/auth/login` with your credentials
2. Copy the `accessToken` from the response
3. Click the **Authorize** button (??) at the top right
4. Paste the token (don't add 'Bearer' prefix)
5. Click **Authorize**

## ?? User Roles

### Admin Role
**Full Access** - Can view and modify all system data:
- ? View all employees, attendance logs, violations, and leave requests
- ? Create, update, and delete employees
- ? Manage attendance logs and violations
- ? Approve/reject leave requests
- ? Manage employee shifts and departments

### Member Role
**Limited Access** - Can only view and manage own data:
- ? View all employees (read-only)
- ? View own attendance logs
- ? View own violations
- ? Submit and view own leave requests
- ? Cannot modify others' data
- ? Cannot approve leave requests
- ? Cannot manage system configuration

## ?? API Endpoints Overview

### Authentication
- `POST /api/v1/auth/login` - Login
- `POST /api/v1/auth/refresh` - Refresh token
- `POST /api/v1/auth/logout` - Logout

### Employees
- `GET /api/v1/employees` - All users can view
- `POST /api/v1/employees` - **Admin only**
- `PUT /api/v1/employees/{id}` - **Admin only**
- `DELETE /api/v1/employees/{id}` - **Admin only**

### Attendance Logs
- `GET /api/v1/attendancelogs` - Admin sees all, members see own
- `GET /api/v1/attendancelogs/{id}` - Get specific log
- `POST /api/v1/attendancelogs` - **Admin only**
- `PUT /api/v1/attendancelogs/{id}` - **Admin only**

### Violations
- `GET /api/v1/violations` - Admin sees all, members see own
- `GET /api/v1/violations/employee/{employeeId}` - Get by employee
- `POST /api/v1/violations` - **Admin only**
- `PUT /api/v1/violations/{id}` - **Admin only**
- `DELETE /api/v1/violations/{id}` - **Admin only**

### Leave Requests
- `GET /api/v1/leaverequests` - **Admin only** (all requests)
- `GET /api/v1/leaverequests/my` - Get own requests
- `POST /api/v1/leaverequests` - Submit leave request
- `PUT /api/v1/leaverequests/{id}/review` - **Admin only** (approve/reject)
- `DELETE /api/v1/leaverequests/{id}` - Delete own pending request

### Employee Shifts
- `GET /api/v1/employeeshifts` - All users can view
- `POST /api/v1/employeeshifts` - **Admin only**
- `PUT /api/v1/employeeshifts/{employeeId}/{startDate}` - **Admin only**
- `DELETE /api/v1/employeeshifts/{employeeId}/{startDate}` - **Admin only**

### Departments
- `GET /api/v1/departments` - All users can view
- `POST /api/v1/departments` - **Admin only**
- `DELETE /api/v1/departments/{id}` - **Admin only**

### Read-Only Resources (All Users)
- Shifts: `/api/v1/shifts`
- Leave Types: `/api/v1/leavetypes`
- Violation Types: `/api/v1/violationtypes`
- Regions: `/api/v1/regions`
- Countries: `/api/v1/countries`
- User Roles: `/api/v1/userroles`

## ?? Base URL
```
http://localhost:5079/api/v1
```

## ?? Response Format

All responses follow this standard format:

**Success Response:**
```json
{
  ""success"": true,
  ""statusCode"": 200,
  ""message"": ""Operation successful"",
  ""data"": { ... }
}
```

**Error Response:**
```json
{
  ""success"": false,
  ""statusCode"": 400,
  ""message"": ""Error message"",
  ""data"": null
}
```

## ?? Error Codes

- **401 Unauthorized** - Missing or invalid token
- **403 Forbidden** - Valid token but insufficient permissions
- **404 Not Found** - Resource not found
- **500 Internal Server Error** - Server error

## ?? Support

- **Developer**: Ahmad Mounir
- **Email**: support@attendancesystem.com
- **GitHub**: [github.com/ahmadmounir/AttendanceSystemBackend](https://github.com/ahmadmounir/AttendanceSystemBackend)

## ?? Tips for Frontend Developers

1. **Store the access token** securely (in memory or secure storage)
2. **Refresh token is in HTTP-only cookie** - no need to handle it manually
3. **Check response status** codes for proper error handling
4. **Use TypeScript types** based on the schemas below
5. **All dates are in UTC** and ISO 8601 format
6. **GUIDs are used for IDs** in string format

Happy coding! ??
";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Attendance System API")
            .WithTheme(ScalarTheme.Moon)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors();

// Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Enable Audit Logging Middleware (after authentication)
app.UseMiddleware<AuditLoggingMiddleware>();

app.MapControllers();

app.Run();
