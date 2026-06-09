using System.Text.Json;
using PrackticFit.Api.Models;

namespace PrackticFit.Api.Services
{
    /// <summary>
    /// Інтеграція зі сторонніми сервісами (наприклад OpenFoodFacts / USDA API)
    /// для миттєвого отримання хімічного складу продуктів.
    /// </summary>
    public class ExternalFoodApiService
    {
        private readonly HttpClient _httpClient;

        public ExternalFoodApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<FoodItem>> SearchProductsAsync(string query)
        {
            // У реальному додатку тут був би виклик до OpenFoodFacts API:
            // var response = await _httpClient.GetAsync($"https://world.openfoodfacts.org/cgi/search.pl?search_terms={query}&search_simple=1&action=process&json=1");
            
            // Для цілей демонстрації/тестування імітуємо отримання даних зі стороннього API:
            await Task.Delay(500); // Симуляція мережевої затримки

            var mockDatabase = new List<FoodItem>
            {
                new FoodItem { Name = "Яблуко", CaloriesPer100g = 52, ProteinPer100g = 0.3, CarbsPer100g = 14, FatsPer100g = 0.2, VitaminC_mg = 4.6, ExternalId = "ext_1" },
                new FoodItem { Name = "Куряче філе", CaloriesPer100g = 165, ProteinPer100g = 31, CarbsPer100g = 0, FatsPer100g = 3.6, Iron_mg = 1.1, ExternalId = "ext_2" },
                new FoodItem { Name = "Гречка", CaloriesPer100g = 343, ProteinPer100g = 13.3, CarbsPer100g = 71.5, FatsPer100g = 3.4, Calcium_mg = 18, ExternalId = "ext_3" }
            };

            return mockDatabase
                .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
