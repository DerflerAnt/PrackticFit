using System.ComponentModel.DataAnnotations;

namespace PrackticFit.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string Name { get; set; } = "Користувач";

        // Антропометрія
        public double WeightKg { get; set; } = 75.0;
        public double HeightCm { get; set; } = 175.0;
        public int Age { get; set; } = 25;
        public string Gender { get; set; } = "Male"; // Male, Female
        public string ActivityLevel { get; set; } = "ModeratelyActive"; // Sedentary, LightlyActive, ModeratelyActive, VeryActive, ExtraActive
        public string FitnessGoal { get; set; } = "MaintainWeight"; // WeightLoss, MaintainWeight, MuscleGain

        // Цілі (розраховуються сервером)
        public double DailyCalorieTarget { get; set; }
        public double ProteinGoal { get; set; }
        public double CarbsGoal { get; set; }
        public double FatsGoal { get; set; }

        public List<MealLog> MealLogs { get; set; } = new();
    }
}
