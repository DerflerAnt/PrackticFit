using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrackticFit.Api.Data;
using PrackticFit.Api.Models;
using PrackticFit.Api.Services;

namespace PrackticFit.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Потребує JWT
    public class DiaryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly NutritionCalculatorService _calculatorService;

        public DiaryController(AppDbContext context, NutritionCalculatorService calculatorService)
        {
            _context = context;
            _calculatorService = calculatorService;
        }

        [HttpGet("analysis/today")]
        public async Task<IActionResult> GetTodayAnalysis()
        {
            // У реальному додатку ID дістаємо з JWT Claims
            // var userId = int.Parse(User.FindFirst("id").Value);
            var userId = 1; 

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var todayLogs = await _context.MealLogs
                .Include(m => m.FoodItem)
                .Where(m => m.UserId == userId && m.ConsumedAt.Date == DateTime.UtcNow.Date)
                .ToListAsync();

            var analysis = _calculatorService.AnalyzeDailyNutrition(user, todayLogs);
            
            return Ok(analysis);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddMealLog([FromBody] MealLog log)
        {
            var userId = 1; // Заглушка для JWT

            var foodItem = await _context.FoodItems.FindAsync(log.FoodItemId);
            if (foodItem == null) return NotFound("Продукт не знайдено");

            log.UserId = userId;
            log.ConsumedAt = DateTime.UtcNow;

            // Зберігаємо фактичні макроси на момент додавання
            double multiplier = log.QuantityGrams / 100.0;
            log.Calories = foodItem.CaloriesPer100g * multiplier;
            log.Protein = foodItem.ProteinPer100g * multiplier;
            log.Carbs = foodItem.CarbsPer100g * multiplier;
            log.Fats = foodItem.FatsPer100g * multiplier;

            _context.MealLogs.Add(log);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Прийом їжі успішно додано", logId = log.Id });
        }
    }
}
