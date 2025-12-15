using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Models.DTOs;
using AttendanceSystemBackend.Repositories.Employees;
using AttendanceSystemBackend.Repositories.Auth;
using AttendanceSystemBackend.Repositories.EmployeeShifts;
using AttendanceSystemBackend.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("employees")]
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IEmployeesRepo _employeesRepo;
        private readonly IAuthRepo _authRepo;
        private readonly IEmployeeShiftsRepo _employeeShiftsRepo;
        private readonly IUserAuthorizationService _authorizationService;

        public EmployeesController(
            IEmployeesRepo employeesRepo, 
            IAuthRepo authRepo, 
            IEmployeeShiftsRepo employeeShiftsRepo,
            IUserAuthorizationService authorizationService)
        {
            _employeesRepo = employeesRepo;
            _authRepo = authRepo;
            _employeeShiftsRepo = employeeShiftsRepo;
            _authorizationService = authorizationService;
        }

        // GET /api/v1/employees (All users can view)
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var c = await _employeesRepo.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<Models.Employees>>.SuccessResponse(
                    c, "Employees retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<Models.Employees>>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        /// <summary>
        /// Add new employee with user account - Admin only
        /// </summary>
        /// <param name="dto">Employee and account data. Email will be used as username.</param>
        /// <returns>Created employee ID</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/employees
        ///     {
        ///         "firstName": "John",
        ///         "lastName": "Doe",
        ///         "email": "john.doe@company.com",
        ///         "phone": "+1234567890",
        ///         "shiftId": "d290f1ee-6c54-4b01-90e6-d701748f0851",
        ///         "departmentId": "3794CA53-1A31-4D4F-9A73-3FC4865C2CDD",
        ///         "jobId": "6FF6FADF-27C8-4EC4-B086-F57BD99D3221",
        ///         "countryId": "a005-tur-005",
        ///         "isSystemActive": true,
        ///         "password": "initialPassword123",
        ///         "roleId": "role-id-here"
        ///     }
        ///     
        /// Note: 
        /// - Email will automatically be used as the username for login.
        /// - HireDate and StartDate are automatically set to the current date/time.
        /// - Employee shift schedule is automatically created with the provided shiftId.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] CreateEmployeeWithAccountDto dto)
        {
            try
            {
                var (isAuthorized, errorMessage) = await _authorizationService.ValidateAdminAccessAsync(User);
                if (!isAuthorized)
                {
                    var statusCode = errorMessage == "Not authorized" ? 401 : 403;
                    return StatusCode(statusCode, ApiResponse<string>.ErrorResponse(
                        errorMessage ?? "Access denied", statusCode));
                }

                var currentDate = DateTime.Now;

                var employee = new Models.Employees
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    HireDate = currentDate,
                    StartDate = currentDate,
                    EndDate = dto.EndDate,
                    ShiftId = dto.ShiftId,
                    DepartmentId = dto.DepartmentId,
                    JobId = dto.JobId,
                    CountryId = dto.CountryId,
                    IsSystemActive = dto.IsSystemActive
                };

                var employeeId = await _employeesRepo.AddAsync(employee);

                var userAccount = new Models.UserAccount
                {
                    Id = Guid.NewGuid().ToString(),
                    EmployeeId = employeeId,
                    Username = dto.Email,
                    Password = dto.Password,
                    RoleId = dto.RoleId
                };

                await _authRepo.CreateUserAccountAsync(userAccount);

                var employeeShift = new Models.EmployeeShift
                {
                    EmployeeId = employeeId,
                    StartDate = currentDate,
                    ShiftId = dto.ShiftId,
                    EndDate = null
                };

                await _employeeShiftsRepo.AddAsync(employeeShift);

                return Ok(ApiResponse<string>.SuccessResponse(
                    employeeId, "Employee added successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        /// <summary>
        /// Update employee and user account - Admin only
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <param name="dto">Updated employee and account data. Email will be used as username.</param>
        /// <returns>Success message</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/v1/employees/{id}
        ///     {
        ///         "firstName": "John",
        ///         "lastName": "Doe",
        ///         "email": "john.doe@company.com",
        ///         "phone": "+1234567890",
        ///         "shiftId": "d290f1ee-6c54-4b01-90e6-d701748f0851",
        ///         "departmentId": "3794CA53-1A31-4D4F-9A73-3FC4865C2CDD",
        ///         "jobId": "6FF6FADF-27C8-4EC4-B086-F57BD99D3221",
        ///         "countryId": "a005-tur-005",
        ///         "isSystemActive": true,
        ///         "password": "newPassword123",
        ///         "roleId": "role-id-here"
        ///     }
        ///     
        /// Note: 
        /// - Email will automatically be used as the username.
        /// - Password is optional. If not provided, the existing password will not be changed.
        /// - HireDate and StartDate from the original record are preserved.
        /// - If shiftId changes, the current shift assignment is ended and a new one is created.
        /// </remarks>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] string id, [FromBody] UpdateEmployeeWithAccountDto dto)
        {
            try
            {
                var (isAuthorized, errorMessage) = await _authorizationService.ValidateAdminAccessAsync(User);
                if (!isAuthorized)
                {
                    var statusCode = errorMessage == "Not authorized" ? 401 : 403;
                    return StatusCode(statusCode, ApiResponse<string>.ErrorResponse(
                        errorMessage ?? "Access denied", statusCode));
                }

                var existingEmployee = await _employeesRepo.GetByIdAsync(id);
                if (existingEmployee == null)
                {
                    return NotFound(ApiResponse<string>.ErrorResponse(
                        "Employee not found", 404));
                }

                var employee = new Models.Employees
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    HireDate = existingEmployee.HireDate,
                    StartDate = existingEmployee.StartDate,
                    EndDate = dto.EndDate,
                    ShiftId = dto.ShiftId,
                    DepartmentId = dto.DepartmentId,
                    JobId = dto.JobId,
                    CountryId = dto.CountryId,
                    IsSystemActive = dto.IsSystemActive
                };

                await _employeesRepo.UpdateAsync(id, employee);

                await _authRepo.UpdateUserAccountAsync(id, dto.Email, dto.Password, dto.RoleId);

                if (existingEmployee.ShiftId != dto.ShiftId)
                {
                    var currentDate = DateTime.Now;
                    var yesterday = currentDate.AddDays(-1);
                    
                    var currentShifts = await _employeeShiftsRepo.GetByEmployeeIdAsync(id);
                    var activeShift = currentShifts.FirstOrDefault(s => s.EndDate == null);
                    
                    if (activeShift != null)
                    {
                        activeShift.EndDate = yesterday;
                        await _employeeShiftsRepo.UpdateAsync(
                            activeShift.EmployeeId, 
                            activeShift.StartDate, 
                            activeShift);
                    }

                    var newEmployeeShift = new Models.EmployeeShift
                    {
                        EmployeeId = id,
                        StartDate = currentDate,
                        ShiftId = dto.ShiftId,
                        EndDate = null
                    };

                    await _employeeShiftsRepo.AddAsync(newEmployeeShift);
                }

                return Ok(ApiResponse<string>.SuccessResponse(
                    id, "Employee updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        /// <summary>
        /// Delete employee, user account, and all related records - Admin only
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Success message</returns>
        /// <remarks>
        /// Deletion order:
        /// 1. UserAccount (and cascading RefreshTokens)
        /// 2. Employee (and cascading EmployeeShifts, Violations)
        /// 
        /// Note: AttendanceLogs, OvertimeRequests, LeaveRequests, and LeaveBalances
        /// are also deleted as they reference the employee.
        /// </remarks>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] string id)
        {
            try
            {
                var (isAuthorized, errorMessage) = await _authorizationService.ValidateAdminAccessAsync(User);
                if (!isAuthorized)
                {
                    var statusCode = errorMessage == "Not authorized" ? 401 : 403;
                    return StatusCode(statusCode, ApiResponse<int>.ErrorResponse(
                        errorMessage ?? "Access denied", statusCode));
                }

                var existingEmployee = await _employeesRepo.GetByIdAsync(id);
                if (existingEmployee == null)
                {
                    return NotFound(ApiResponse<int>.ErrorResponse(
                        "Employee not found", 404));
                }

                await _authRepo.DeleteUserAccountByEmployeeIdAsync(id);

                var rowsAffected = await _employeesRepo.DeleteAsync(id);

                return Ok(ApiResponse<int>.SuccessResponse(
                    rowsAffected, "Employee and all related records deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.ErrorResponse(
                    ex.Message, 500));
            }
        }
    }
}
