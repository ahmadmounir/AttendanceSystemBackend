using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.Countries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("countries")]
    [Authorize]
    public class CountriesController : Controller
    {
        private readonly ICountriesRepo _countriesRepo;

        public CountriesController(ICountriesRepo countriesRepo)
        {
            _countriesRepo = countriesRepo;
        }

        // GET /api/v1/countries
        [HttpGet]
        public async Task<IActionResult> GetAllCountries()
        {
            try
            {
                var countries = await _countriesRepo.GetAllAsync();
                var response = ApiResponse<IEnumerable<Country>>.SuccessResponse(
                    countries,
                    "Countries retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Country>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/countries/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCountryById([FromRoute] string id)
        {
            try
            {
                var country = await _countriesRepo.GetByIdAsync(id);
                if (country == null)
                {
                    var notFoundResponse = ApiResponse<Country>.ErrorResponse(
                        "Country not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<Country>.SuccessResponse(
                    country,
                    "Country retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<Country>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }
    }
}
