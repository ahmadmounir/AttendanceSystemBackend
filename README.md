# Attendance System Backend

A RESTful API built with ASP.NET Core 10 for managing employee attendance, regions, countries, and user roles with JWT authentication and automatic audit logging.

## 🚀 Technologies

- **.NET 10** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **JWT Authentication** - Secure token-based authentication
- **Audit Logging** - Automatic request logging for compliance
- **Dapper** - Lightweight ORM for database operations
- **Microsoft SQL Server** - Database
- **OpenAPI/Swagger** - API documentation

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
- [Visual Studio 2025](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

## 🛠️ Getting Started

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

### 3. Configure JWT Settings

The JWT settings are already configured in `appsettings.json`. For production, change the secret key:

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256Algorithm",
    "Issuer": "AttendanceSystemBackend",
    "Audience": "AttendanceSystemFrontend",
    "AccessTokenExpirationMinutes": "60"
  }
}
```

⚠️ **Important**: Generate a strong, unique secret key for production!

### 4. Create Database Schema

Run the SQL scripts to create the required tables:

1. Execute `Database/AuthSchema.sql` - Creates authentication tables
2. Execute `Database/AuditLogsSchema.sql` - Creates audit logging table

#### AuditLogs Table
```sql
CREATE TABLE AuditLogs (
    id NVARCHAR(255) PRIMARY KEY,
    userId NVARCHAR(255) NOT NULL,
    action NVARCHAR(255) NULL,
    tableName NVARCHAR(255) NOT NULL,
    timestamp DATETIME NOT NULL
);
```

For other tables (Regions, Countries, UserRoles, Employees), refer to the SQL comments in the schema files.

### 5. Restore Dependencies

```bash
dotnet restore
```

### 6. Build the Project

```bash
dotnet build
```

### 7. Run the Application

```bash
dotnet run
```

The API will be available at: `http://localhost:5079`

### 8. Test the Authentication

Default test credentials (created by the AuthSchema.sql script):
- **Username**: `admin`
- **Password**: `password123`

## 🔐 Authentication

This API uses JWT (JSON Web Tokens) for authentication with refresh token support.

### Authentication Flow

1. **Login**: Send credentials to `/api/v1/auth/login`
2. **Receive Tokens**: Get access token (1 hour) and refresh token (7 days)
3. **Use Access Token**: Include in Authorization header for protected endpoints
4. **Refresh Token**: When access token expires, use refresh token to get new tokens
5. **Logout**: Revoke refresh token via `/api/v1/auth/revoke`

### Authentication Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/v1/auth/login` | Login with username/password | No |
| POST | `/api/v1/auth/refresh` | Refresh access token | No |
| POST | `/api/v1/auth/revoke` | Revoke refresh token (logout) | Yes |

### Login Example

**Request:**
```bash
curl -X POST http://localhost:5079/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "password123"
  }'
```

**Response:**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64-encoded-refresh-token",
    "username": "admin",
    "employeeId": "emp-123",
    "roleId": "role-1",
    "expiresAt": "2025-12-10T10:30:00Z"
  }
}
```

### Using Access Token

Include the access token in the Authorization header:

```bash
curl -X GET http://localhost:5079/api/v1/employees \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

### Refresh Token Example

**Request:**
```bash
curl -X POST http://localhost:5079/api/v1/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "your-refresh-token-here"
  }'
```

## 📝 Audit Logging

The system automatically logs all authenticated API requests for compliance and security monitoring.

### What Gets Logged

Every authenticated request to the API is automatically logged with:
- **User ID**: Who made the request
- **Action**: What type of operation (READ, CREATE, UPDATE, DELETE)
- **Table Name**: Which resource was accessed
- **Timestamp**: When the request was made (UTC)

### Example Audit Log Entry

```json
{
  "id": "guid-here",
  "userId": "user-1",
  "action": "READ",
  "tableName": "employees",
  "timestamp": "2025-12-10T09:30:00Z"
}
```

### Actions Logged

| HTTP Method | Logged Action |
|-------------|---------------|
| GET | READ |
| POST | CREATE |
| PUT | UPDATE |
| DELETE | DELETE |
| PATCH | UPDATE |

### What's NOT Logged

- Authentication endpoints (`/api/v1/auth/*`)
- Unauthenticated requests
- Non-API requests

### Audit Log Benefits

✅ **Compliance**: Meet regulatory requirements (GDPR, HIPAA, etc.)  
✅ **Security**: Track suspicious activities and unauthorized access attempts  
✅ **Debugging**: Trace user actions for troubleshooting  
✅ **Analytics**: Understand API usage patterns  
✅ **Accountability**: Know who did what and when

### Querying Audit Logs

You can query the `AuditLogs` table directly:

```sql
-- Get all actions by a specific user
SELECT * FROM AuditLogs 
WHERE userId = 'user-1' 
ORDER BY timestamp DESC;

-- Get all actions on a specific table
SELECT * FROM AuditLogs 
WHERE tableName = 'employees' 
ORDER BY timestamp DESC;

-- Get recent activity (last 24 hours)
SELECT * FROM AuditLogs 
WHERE timestamp >= DATEADD(hour, -24, GETUTCDATE())
ORDER BY timestamp DESC;

-- Count actions by type
SELECT action, COUNT(*) as count
FROM AuditLogs
GROUP BY action;
```

## 📡 API Endpoints

All endpoints are prefixed with `/api/v1`

🔓 = No authentication required  
🔒 = Authentication required (logged in audit)

### Authentication

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/v1/auth/login` | User login | 🔓 |
| POST | `/api/v1/auth/refresh` | Refresh access token | 🔓 |
| POST | `/api/v1/auth/revoke` | Revoke refresh token | 🔒 |

### Regions

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/v1/regions` | Get all regions | 🔒 |
| GET | `/api/v1/regions/{id}` | Get region by ID | 🔒 |

### Countries

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/v1/countries` | Get all countries | 🔒 |
| GET | `/api/v1/countries/{id}` | Get country by ID | 🔒 |

### User Roles

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/v1/userroles` | Get all user roles | 🔒 |
| GET | `/api/v1/userroles/{id}` | Get user role by ID | 🔒 |

### Employees

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/v1/employees` | Get all employees | 🔒 |
| POST | `/api/v1/employees` | Create a new employee | 🔒 |
| PUT | `/api/v1/employees/{id}` | Update an employee | 🔒 |
| DELETE | `/api/v1/employees/{id}` | Delete an employee | 🔒 |

## 📦 API Response Format

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

### Authentication Error (401)
```json
{
  "success": false,
  "statusCode": 401,
  "message": "Invalid username or password",
  "data": null
}
```

## 🔧 Configuration

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

### JWT Configuration

Token lifetimes can be configured in `appsettings.json`:
- **Access Token**: 60 minutes (configurable)
- **Refresh Token**: 7 days (hardcoded in AuthService.cs)

## 🏗️ Project Structure

```
AttendanceSystemBackend/
├── Controllers/              # API Controllers
│   ├── AuthController.cs
│   ├── EmployeesController.cs
│   ├── RegionsController.cs
│   ├── CountriesController.cs
│   └── UserRolesController.cs
├── Middleware/               # Custom middleware
│   └── AuditLoggingMiddleware.cs
├── Models/                   # Data models
│   ├── ApiResponse.cs
│   ├── UserAccount.cs
│   ├── RefreshToken.cs
│   ├── AuditLog.cs
│   ├── Employees.cs
│   ├── Region.cs
│   ├── Country.cs
│   ├── UserRole.cs
│   └── DTOs/                # Data Transfer Objects
│       ├── LoginRequest.cs
│       ├── LoginResponse.cs
│       └── RefreshTokenRequest.cs
├── Repositories/             # Data access layer
│   ├── Auth/
│   ├── AuditLogs/
│   ├── Employees/
│   ├── Regions/
│   ├── Countries/
│   └── UserRoles/
├── Services/                 # Business logic
│   └── Auth/
│       ├── IAuthService.cs
│       └── AuthService.cs
├── Extensions/               # Extension methods
│   └── MvcOptionsExtensions.cs
├── Database/                 # SQL scripts
│   ├── AuthSchema.sql
│   └── AuditLogsSchema.sql
├── appsettings.json         # Configuration
└── Program.cs               # Application entry point
```

## 🧪 Testing

You can test the API using:

1. **Swagger UI** (Development mode only)
   - Navigate to `http://localhost:5079/openapi/v1.json`

2. **Postman** or **Thunder Client**
   - Import the API endpoints and start testing
   - Remember to set the Authorization header with Bearer token

3. **cURL Examples**
   ```bash
   # Login
   curl -X POST http://localhost:5079/api/v1/auth/login \
     -H "Content-Type: application/json" \
     -d '{"username":"admin","password":"password123"}'
   
   # Get regions (with token) - This will be logged in AuditLogs
   curl -X GET http://localhost:5079/api/v1/regions \
     -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
   ```

## 🔐 Security Features

- **JWT Token Authentication**: Stateless authentication using JWT
- **Refresh Token Rotation**: New refresh token issued on each refresh
- **Token Expiration**: Access tokens expire after 60 minutes
- **Token Revocation**: Ability to revoke refresh tokens
- **Audit Logging**: Automatic logging of all authenticated requests
- **CORS Protection**: Only configured origins can access the API
- **HTTPS Redirection**: Forces HTTPS in production

⚠️ **Security Notes**:
- Passwords are currently stored in plain text (for simplicity)
- In production, implement proper password hashing (BCrypt, Argon2, PBKDF2)
- Use strong, unique JWT secret keys
- Consider implementing rate limiting for authentication endpoints
- Use HTTPS in production environments
- Regularly review audit logs for suspicious activities

## 🐛 Troubleshooting

### Authentication Issues

**401 Unauthorized Error**:
1. Verify your access token hasn't expired (check `expiresAt` from login response)
2. Ensure token is included in Authorization header: `Bearer YOUR_TOKEN`
3. Check if the token format is correct (should start with `eyJ...`)

**Invalid Token Error**:
1. Token may have expired - use refresh token endpoint
2. JWT secret key might have changed - re-login to get new tokens
3. Token format is incorrect - ensure "Bearer " prefix

### Connection Issues

If you encounter database connection errors:
1. Verify SQL Server is running
2. Check the connection string in `appsettings.json`
3. Ensure the database and tables exist
4. Verify Windows Authentication or provide SQL credentials

### Audit Logging Issues

**Logs not appearing**:
1. Ensure user is authenticated (audit logs only for authenticated requests)
2. Check that AuditLogs table exists in database
3. Verify the middleware is registered in `Program.cs`
4. Check application logs for any errors

### CORS Errors

If you get CORS errors from your frontend:
1. Verify your frontend is running on ports 3000-3003
2. Check the CORS configuration in `Program.cs`
3. Add additional origins if needed
4. Ensure credentials are enabled: `credentials: 'include'`

## 📚 Dependencies

- **Dapper** (v2.1.66) - Micro-ORM
- **Microsoft.Data.SqlClient** (v6.1.3) - SQL Server data provider
- **Microsoft.AspNetCore.OpenApi** (v10.0.0) - OpenAPI support
- **Microsoft.AspNetCore.Authentication.JwtBearer** (v10.0.0) - JWT authentication

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is part of a Database course project.

## 👥 Authors

- Ahmad Mounir - [GitHub](https://github.com/ahmadmounir)

## 📞 Support

For support and questions, please open an issue in the GitHub repository.

---

**Happy Coding! 🚀**
