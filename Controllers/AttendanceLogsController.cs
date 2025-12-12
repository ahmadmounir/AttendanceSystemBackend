using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.AttendanceLogs;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("attendancelogs")]
    public class AttendanceLogsController : Controller
    {
        private readonly IAttendanceLogsRepo _attendanceLogsRepo;

        public AttendanceLogsController(IAttendanceLogsRepo attendanceLogsRepo)
        {
            _attendanceLogsRepo = attendanceLogsRepo;
        }

        // GET /api/v1/attendancelogs
        [HttpGet]
        public async Task<IActionResult> GetAllAttendanceLogs()
        {
            try
            {
                var items = await _attendanceLogsRepo.GetAllAsync();
                var response = ApiResponse<IEnumerable<Models.AttendanceLog>>.SuccessResponse(
                    items,
                    "Attendance logs retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Models.AttendanceLog>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/attendancelogs/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttendanceLogById([FromRoute] string id)
        {
            try
            {
                var item = await _attendanceLogsRepo.GetByIdAsync(id);
                if (item == null)
                {
                    var notFoundResponse = ApiResponse<Models.AttendanceLog>.ErrorResponse(
                        "Attendance log not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<Models.AttendanceLog>.SuccessResponse(
                    item,
                    "Attendance log retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<Models.AttendanceLog>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // POST /api/v1/attendancelogs
        [HttpPost]
        public async Task<IActionResult> AddAttendanceLog([FromBody] Models.AttendanceLog attendanceLog)
        {
            try
            {
                var newId = await _attendanceLogsRepo.AddAsync(attendanceLog);
                var response = ApiResponse<string>.SuccessResponse(
                    newId,
                    "Attendance log added successfully"
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

        // PUT /api/v1/attendancelogs/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttendanceLog([FromRoute] string id, [FromBody] Models.AttendanceLog attendanceLog)
        {
            try
            {
                var updated = await _attendanceLogsRepo.UpdateAsync(id, attendanceLog);
                var response = ApiResponse<Models.AttendanceLog>.SuccessResponse(
                    updated,
                    "Attendance log updated successfully"
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
