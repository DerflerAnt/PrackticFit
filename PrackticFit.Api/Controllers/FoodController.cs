using Microsoft.AspNetCore.Mvc;
using PrackticFit.Api.Services;

namespace PrackticFit.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodController : ControllerBase
    {
        private readonly ExternalFoodApiService _externalApiService;

        public FoodController(ExternalFoodApiService externalApiService)
        {
            _externalApiService = externalApiService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchFoods([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Запит не може бути порожнім.");

            var results = await _externalApiService.SearchProductsAsync(query);
            return Ok(results);
        }
    }
}
