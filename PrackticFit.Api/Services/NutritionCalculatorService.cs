using PrackticFit.Api.Models;

namespace PrackticFit.Api.Services
{
    /// <summary>
    /// Серверна бізнес-логіка для математичного розрахунку індивідуальної добової норми калорій
    /// та аналізу збалансованості раціону (Дефіцит/Профіцит і Ранок-Вечір).
    /// </summary>
    public class NutritionCalculatorService
    {
        public void CalculateUserGoals(User user)
        {
            // 1. Формула Міффліна-Сан Жеора
            double bmr = 10 * user.WeightKg + 6.25 * user.HeightCm - 5.0 * user.Age;
            bmr = user.Gender == "Male" ? bmr + 5 : bmr - 161;

            // 2. Коефіцієнт активності (PAL)
            double pal = user.ActivityLevel switch
            {
                "Sedentary" => 1.2,
                "LightlyActive" => 1.375,
                "ModeratelyActive" => 1.55,
                "VeryActive" => 1.725,
                "ExtraActive" => 1.9,
                _ => 1.2
            };

            double tdee = bmr * pal;

            // 3. Фітнес-мета
            double adjustment = user.FitnessGoal switch
            {
                "WeightLoss" => -500,
                "MuscleGain" => +300,
                _ => 0
            };

            double target = Math.Max(1200, Math.Round(tdee + adjustment, 0));
            user.DailyCalorieTarget = target;

            // 4. Макронутрієнти (30% білки, 40% вуглеводи, 30% жири)
            user.ProteinGoal = Math.Round(target * 0.30 / 4.0, 0);
            user.CarbsGoal = Math.Round(target * 0.40 / 4.0, 0);
            user.FatsGoal = Math.Round(target * 0.30 / 9.0, 0);
        }

        public object AnalyzeDailyNutrition(User user, List<MealLog> dailyLogs)
        {
            double consumedCalories = dailyLogs.Sum(l => l.Calories);
            double consumedProtein = dailyLogs.Sum(l => l.Protein);
            double consumedCarbs = dailyLogs.Sum(l => l.Carbs);
            double consumedFats = dailyLogs.Sum(l => l.Fats);

            // Аналіз Ранок-Вечір (вуглеводи після 20:00)
            var eveningLogs = dailyLogs.Where(l => l.ConsumedAt.ToLocalTime().Hour >= 20).ToList();
            double eveningCarbs = eveningLogs.Sum(l => l.Carbs);
            double eveningCarbsPercent = consumedCarbs > 0 ? (eveningCarbs / consumedCarbs) * 100.0 : 0;
            
            bool eveningWarning = eveningCarbsPercent > 40.0;

            return new
            {
                CalorieTarget = user.DailyCalorieTarget,
                CaloriesConsumed = consumedCalories,
                CaloriesRemaining = Math.Max(0, user.DailyCalorieTarget - consumedCalories),
                
                Macros = new {
                    Protein = new { Consumed = consumedProtein, Goal = user.ProteinGoal },
                    Carbs = new { Consumed = consumedCarbs, Goal = user.CarbsGoal },
                    Fats = new { Consumed = consumedFats, Goal = user.FatsGoal }
                },

                MorningEveningAnalysis = new
                {
                    EveningCarbsGrams = eveningCarbs,
                    EveningCarbsPercent = Math.Round(eveningCarbsPercent, 1),
                    Warning = eveningWarning,
                    Message = eveningWarning 
                        ? $"Увага: {Math.Round(eveningCarbsPercent, 1)}% вуглеводів спожито після 20:00." 
                        : "Розподіл вуглеводів в нормі."
                }
            };
        }
    }
}
