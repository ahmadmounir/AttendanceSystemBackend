using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.Regions;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("regions")]
    public class RegionsController : Controller
    {
        private readonly IRegionsRepo _regionsRepo;

        public RegionsController(IRegionsRepo regionsRepo)
        {
            _regionsRepo = regionsRepo;
        }

        // GET /api/v1/regions
        [HttpGet]
        public async Task<IActionResult> GetAllRegions()
        {
            try
            {
                var regions = await _regionsRepo.GetAllAsync();
                var response = ApiResponse<IEnumerable<Region>>.SuccessResponse(
                    regions,
                    "Regions retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Region>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/regions/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegionById([FromRoute] string id)
        {
            try
            {
                var region = await _regionsRepo.GetByIdAsync(id);
                if (region == null)
                {
                    var notFoundResponse = ApiResponse<Region>.ErrorResponse(
                        "Region not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<Region>.SuccessResponse(
                    region,
                    "Region retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<Region>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }
    }
}
