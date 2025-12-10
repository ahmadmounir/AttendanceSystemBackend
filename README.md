# Attendance System Backend

A RESTful API built with ASP.NET Core 10 for managing employee attendance, regions, countries, and user roles.

## ?? Technologies

- **.NET 10** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **Dapper** - Lightweight ORM for database operations
- **Microsoft SQL Server** - Database
- **OpenAPI/Swagger** - API documentation

## ?? Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
- [Visual Studio 2025](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

## ??? Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/ahmadmounir/AttendanceSystemBackend.git
cd AttendanceSystemBackend
```

### 2. Configure Database Connection

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=EmployeesAttendanceLeavingSystem;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Replace `YOUR_SERVER_NAME` with your SQL Server instance name.

### 3. Create Database Schema

Ensure your SQL Server database has the following tables:

#### Regions Table
```sql
CREATE TABLE Regions (
    id NVARCHAR(450) PRIMARY KEY,
    regionName NVARCHAR(255) NOT NULL
);
```

#### Countries Table
```sql
CREATE TABLE Countries (
    id NVARCHAR(450) PRIMARY KEY,
    countryName NVARCHAR(255) NOT NULL,
    regionId NVARCHAR(450),
    FOREIGN KEY (regionId) REFERENCES Regions(id)
);
```

#### UserRoles Table
```sql
CREATE TABLE UserRoles (
    id NVARCHAR(450) PRIMARY KEY,
    roleName NVARCHAR(255) NOT NULL
);
```

#### Employees Table
```sql
CREATE TABLE Employees (
    id UNIQUEIDENTIFIER PRIMARY KEY,
    firstName NVARCHAR(255) NOT NULL,
    lastName NVARCHAR(255) NOT NULL,
    email NVARCHAR(255) NOT NULL,
    phone NVARCHAR(50),
    hireDate DATETIME NOT NULL,
    startDate DATETIME NOT NULL,
    endDate DATETIME,
    shiftId NVARCHAR(450),
    departmentId NVARCHAR(450),
    jobId NVARCHAR(450),
    countryId NVARCHAR(450),
    isSystemActive BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (countryId) REFERENCES Countries(id)
);
```

### 4. Restore Dependencies

```bash
dotnet restore
```

### 5. Build the Project

```bash
dotnet build
```

### 6. Run the Application

```bash
dotnet run
```

The API will be available at: `http://localhost:5079`

## ?? API Endpoints

All endpoints are prefixed with `/api/v1`

### Regions

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/regions` | Get all regions |
| GET | `/api/v1/regions/{id}` | Get region by ID |

### Countries

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/countries` | Get all countries |
| GET | `/api/v1/countries/{id}` | Get country by ID |

### User Roles

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/userroles` | Get all user roles |
| GET | `/api/v1/userroles/{id}` | Get user role by ID |

### Employees

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/employees` | Get all employees |
| POST | `/api/v1/employees` | Create a new employee |
| PUT | `/api/v1/employees/{id}` | Update an employee |
| DELETE | `/api/v1/employees/{id}` | Delete an employee |

## ?? API Response Format

All API responses follow this standard format:

### Success Response
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Data retrieved successfully",
  "data": { ... }
}
```

### Error Response
```json
{
  "success": false,
  "statusCode": 400,
  "message": "Error message",
  "data": null
}
```

## ?? Configuration

### CORS Configuration

The API is configured to accept requests from frontend applications running on ports 3000-3003:
- `http://localhost:3000`
- `http://localhost:3001`
- `http://localhost:3002`
- `http://localhost:3003`

To modify CORS settings, update the `Program.cs` file.

### Port Configuration

The application is configured to always run on port **5079**. To change this, modify the Kestrel configuration in `Program.cs`:

```csharp
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenLocalhost(YOUR_PORT);
});
```

## ??? Project Structure

```
AttendanceSystemBackend/
??? Controllers/              # API Controllers
?   ??? EmployeesController.cs
?   ??? RegionsController.cs
?   ??? CountriesController.cs
?   ??? UserRolesController.cs
??? Models/                   # Data models
?   ??? ApiResponse.cs
?   ??? Employees.cs
?   ??? Region.cs
?   ??? Country.cs
?   ??? UserRole.cs
??? Repositories/             # Data access layer
?   ??? Employees/
?   ??? Regions/
?   ??? Countries/
?   ??? UserRoles/
??? Extensions/               # Extension methods
?   ??? MvcOptionsExtensions.cs
??? appsettings.json         # Configuration
??? Program.cs               # Application entry point
```

## ?? Testing

You can test the API using:

1. **Swagger UI** (Development mode only)
   - Navigate to `http://localhost:5079/openapi/v1.json`

2. **Postman** or **Thunder Client**
   - Import the API endpoints and start testing

3. **cURL Example**
   ```bash
   curl -X GET http://localhost:5079/api/v1/regions
   ```

## ?? Database Connection

The application uses **Dapper** as a micro-ORM for database operations. Connection management follows these principles:

- Connections are created on-demand
- Connections are automatically disposed using `using` statements
- Connection pooling is handled by SQL Server

## ?? Development Guidelines

### Adding a New Module

1. Create the model in `Models/`
2. Create interface and repository in `Repositories/{ModuleName}/`
3. Create controller in `Controllers/`
4. Register repository in `Program.cs` dependency injection
5. Implement GET/POST/PUT/DELETE operations as needed

### Code Style

- Follow C# naming conventions
- Use async/await for database operations
- Implement proper error handling with try-catch blocks
- Return appropriate HTTP status codes
- Use the `ApiResponse<T>` wrapper for all responses

## ?? Troubleshooting

### Connection Issues

If you encounter database connection errors:
1. Verify SQL Server is running
2. Check the connection string in `appsettings.json`
3. Ensure the database exists
4. Verify Windows Authentication or provide SQL credentials

### Port Already in Use

If port 5079 is already in use:
1. Stop the application using that port
2. Or modify the port in `Program.cs`

### CORS Errors

If you get CORS errors from your frontend:
1. Verify your frontend is running on ports 3000-3003
2. Check the CORS configuration in `Program.cs`
3. Add additional origins if needed

## ?? Dependencies

- **Dapper** (v2.1.66) - Micro-ORM
- **Microsoft.Data.SqlClient** (v6.1.3) - SQL Server data provider
- **Microsoft.AspNetCore.OpenApi** (v10.0.0) - OpenAPI support

## ?? Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## ?? License

This project is part of a Database course project.

## ?? Authors

- Ahmad Mounir - [GitHub](https://github.com/ahmadmounir)

## ?? Support

For support and questions, please open an issue in the GitHub repository.

---

**Happy Coding! ??**
