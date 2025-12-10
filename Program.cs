using AttendanceSystemBackend.Repositories.Employees;
using AttendanceSystemBackend.Repositories.Regions;
using AttendanceSystemBackend.Repositories.Countries;
using AttendanceSystemBackend.Repositories.UserRoles;
using AttendanceSystemBackend.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure to always use port 5079
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenLocalhost(5079);
});

// Add services to the container.
builder.Services.AddScoped<IEmployeesRepo, EmployeesRepo>();
builder.Services.AddScoped<IRegionsRepo, RegionsRepo>();
builder.Services.AddScoped<ICountriesRepo, CountriesRepo>();
builder.Services.AddScoped<IUserRolesRepo, UserRolesRepo>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
