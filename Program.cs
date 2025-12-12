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
using AttendanceSystemBackend.Services.Auth;
using AttendanceSystemBackend.Services.LeaveRequests;
using AttendanceSystemBackend.Middleware;

using AttendanceSystemBackend.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IAuthRepo, AuthRepo>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuditLogsRepo, AuditLogsRepo>();
builder.Services.AddScoped<INotificationsRepo, NotificationsRepo>();
builder.Services.AddScoped<ILeaveBalancesRepo, LeaveBalancesRepo>();
builder.Services.AddScoped<ILeaveRequestsRepo, LeaveRequestsRepo>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<ILeaveTypesRepo, LeaveTypesRepo>();

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
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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
