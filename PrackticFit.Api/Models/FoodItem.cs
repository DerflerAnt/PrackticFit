using System.ComponentModel.DataAnnotations;

namespace PrackticFit.Api.Models
{
    public class FoodItem
    {
        public int Id { get; set; }
        public string ExternalId { get; set; } = string.Empty; // ID from external API

        [Required]
        public string Name { get; set; } = string.Empty;
        
        // Маронутрієнти
        public double CaloriesPer100g { get; set; }
        public double ProteinPer100g { get; set; }
        public double CarbsPer100g { get; set; }
        public double FatsPer100g { get; set; }

        // Мікронутрієнти (Хімічний склад)
        public double VitaminC_mg { get; set; }
        public double VitaminD_mcg { get; set; }
        public double Iron_mg { get; set; }
        public double Calcium_mg { get; set; }
    }
}
