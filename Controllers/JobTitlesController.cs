using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.JobTitles;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("jobtitles")]
    public class JobTitlesController : Controller
    {
        private readonly IJobTitlesRepo _jobTitlesRepo;

        public JobTitlesController(IJobTitlesRepo jobTitlesRepo)
        {
            _jobTitlesRepo = jobTitlesRepo;
        }

        // GET /api/v1/jobtitles
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

        // POST /api/v1/jobtitles
        [HttpPost]
        public async Task<IActionResult> AddJobTitle([FromBody] Models.JobTitle jobTitle)
        {
            try
            {
                var newId = await _jobTitlesRepo.AddAsync(jobTitle);
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

        // GET /api/v1/jobtitles/{id}
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

        // DELETE /api/v1/jobtitles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobTitle([FromRoute] string id)
        {
            try
            {
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

        // PUT /api/v1/jobtitles/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobTitle([FromRoute] string id, [FromBody] Models.JobTitle jobTitle)
        {
            try
            {
                var updated = await _jobTitlesRepo.UpdateAsync(id, jobTitle);
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
    }
}
