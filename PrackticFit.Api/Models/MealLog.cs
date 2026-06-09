using System.ComponentModel.DataAnnotations;

namespace PrackticFit.Api.Models
{
    public class MealLog
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int FoodItemId { get; set; }
        public FoodItem? FoodItem { get; set; }

        public double QuantityGrams { get; set; }
        public DateTime ConsumedAt { get; set; } = DateTime.UtcNow;

        // Збережені фактичні макроси на випадок зміни рецептури FoodItem в майбутньому
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fats { get; set; }
    }
}
