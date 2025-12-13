using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Models.DTOs;
using AttendanceSystemBackend.Repositories.JobTitles;
using AttendanceSystemBackend.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("jobtitles")]
    [Authorize]
    public class JobTitlesController : Controller
    {
        private readonly IJobTitlesRepo _jobTitlesRepo;
        private readonly IUserAuthorizationService _authorizationService;

        public JobTitlesController(IJobTitlesRepo jobTitlesRepo, IUserAuthorizationService authorizationService)
        {
            _jobTitlesRepo = jobTitlesRepo;
            _authorizationService = authorizationService;
        }

        // GET /api/v1/jobtitles (All users can view)
        [HttpGet]
        public async Task<IActionResult> GetAllJobTitles()
        {
            try
            {
                var items = await _jobTitlesRepo.GetAllAsync();
                var response = ApiResponse<IEnumerable<Models.JobTitle>>.SuccessResponse(
                    items,
                    "Job titles retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Models.JobTitle>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/jobtitles/{id} (All users can view)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobTitleById([FromRoute] string id)
        {
            try
            {
                var item = await _jobTitlesRepo.GetByIdAsync(id);
                if (item == null)
                {
                    var notFoundResponse = ApiResponse<Models.JobTitle>.ErrorResponse(
                        "Job title not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<Models.JobTitle>.SuccessResponse(
                    item,
                    "Job title retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<Models.JobTitle>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // POST /api/v1/jobtitles (Admin only)
        [HttpPost]
        public async Task<IActionResult> AddJobTitle([FromBody] JobTitleRequestDto jobTitle)
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

                var newId = await _jobTitlesRepo.AddAsync(
                    jobTitle.TitleName, 
                    jobTitle.MinSalary, 
                    jobTitle.MaxSalary);
                
                var response = ApiResponse<string>.SuccessResponse(
                    newId,
                    "Job title added successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<string>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // PUT /api/v1/jobtitles/{id} (Admin only)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobTitle([FromRoute] string id, [FromBody] JobTitleRequestDto jobTitle)
        {
            try
            {
                var (isAuthorized, errorMessage) = await _authorizationService.ValidateAdminAccessAsync(User);
                if (!isAuthorized)
                {
                    var statusCode = errorMessage == "Not authorized" ? 401 : 403;
                    return StatusCode(statusCode, ApiResponse<Models.JobTitle>.ErrorResponse(
                        errorMessage ?? "Access denied", statusCode));
                }

                var updated = await _jobTitlesRepo.UpdateAsync(
                    id, 
                    jobTitle.TitleName, 
                    jobTitle.MinSalary, 
                    jobTitle.MaxSalary);
                
                var response = ApiResponse<Models.JobTitle>.SuccessResponse(
                    updated,
                    "Job title updated successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<int>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // DELETE /api/v1/jobtitles/{id} (Admin only)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobTitle([FromRoute] string id)
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

                var rowsAffected = await _jobTitlesRepo.DeleteAsync(id);
                var response = ApiResponse<int>.SuccessResponse(
                    rowsAffected,
                    "Job title deleted successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<int>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }
    }
}
