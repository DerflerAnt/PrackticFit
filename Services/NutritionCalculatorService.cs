using PrackticFit.Models;

namespace PrackticFit.Services
{
    /// <summary>
    /// Результат аналізу модуля «Ранок-Вечір».
    /// </summary>
    public class MorningEveningAnalysis
    {
        /// <summary>Відсоток добових вуглеводів, спожитих після 20:00.</summary>
        public double EveningCarbsPercent { get; init; }

        /// <summary>Відсоток добових калорій, спожитих після 20:00.</summary>
        public double EveningCaloriesPercent { get; init; }

        /// <summary>Загальна кількість грамів вуглеводів, спожитих увечері.</summary>
        public double EveningCarbsGrams { get; init; }

        /// <summary>Загальна кількість калорій, спожитих увечері.</summary>
        public double EveningCaloriesKcal { get; init; }

        /// <summary>True якщо вечірні вуглеводи перевищують 40% добової норми.</summary>
        public bool EveningCarbsWarning { get; init; }

        /// <summary>Текстове попередження або порада.</summary>
        public string Message { get; init; } = string.Empty;
    }

    /// <summary>
    /// Центральний сервіс обчислювальної логіки застосунку PrackticFit.
    /// Реалізує:
    ///   - формулу Міффліна-Сан Жеора для розрахунку BMR та TDEE,
    ///   - коригування добової норми калорій відповідно до мети (схуднення / підтримка / набір маси),
    ///   - підрахунок макро- та мікронутрієнтів з журналу прийомів їжі,
    ///   - аналіз «Ранок-Вечір» із попередженням при >40% вуглеводів увечері,
    ///   - розрахунок персональних цілей за макронутрієнтами.
    /// </summary>
    public class NutritionCalculatorService
    {
        // ---- Константи PAL-коефіцієнтів ----
        private static readonly Dictionary<ActivityLevel, double> ActivityMultipliers = new()
        {
            { ActivityLevel.Sedentary,        1.2   },
            { ActivityLevel.LightlyActive,    1.375 },
            { ActivityLevel.ModeratelyActive, 1.55  },
            { ActivityLevel.VeryActive,       1.725 },
            { ActivityLevel.ExtraActive,      1.9   }
        };

        // ---- Коригування калорій за ціллю ----
        private static readonly Dictionary<FitnessGoal, double> GoalCalorieAdjustment = new()
        {
            { FitnessGoal.WeightLoss,     -500 },  // Безпечний дефіцит ~0.5 кг/тиждень
            { FitnessGoal.MaintainWeight,    0 },
            { FitnessGoal.MuscleGain,     +300 },  // Профіцит для гіпертрофії
        };

        // ======================================================================
        // 1. ФОРМУЛА МІФФЛІНА-САН ЖЕОРА
        // ======================================================================

        /// <summary>
        /// Розраховує базальний рівень метаболізму (BMR) за формулою Міффліна-Сан Жеора.
        /// Чоловіки: BMR = 10 × weight(kg) + 6.25 × height(cm) − 5 × age + 5
        /// Жінки:    BMR = 10 × weight(kg) + 6.25 × height(cm) − 5 × age − 161
        /// </summary>
        public double CalculateBMR(UserProfile profile)
        {
            ArgumentNullException.ThrowIfNull(profile);

            double bmr = 10 * profile.WeightKg
                       + 6.25 * profile.HeightCm
                       - 5.0 * profile.Age;

            return profile.Gender == Gender.Male ? bmr + 5 : bmr - 161;
        }

        /// <summary>
        /// Розраховує добову потребу в калоріях з урахуванням фізичної активності (TDEE).
        /// TDEE = BMR × PAL
        /// </summary>
        public double CalculateTDEE(UserProfile profile)
        {
            double bmr = CalculateBMR(profile);
            double pal = ActivityMultipliers[profile.ActivityLevel];
            return Math.Round(bmr * pal, 0);
        }

        /// <summary>
        /// Розраховує цільовий добовий калораж з урахуванням фітнес-мети.
        /// </summary>
        public double CalculateDailyCalorieTarget(UserProfile profile)
        {
            double tdee = CalculateTDEE(profile);
            double adjustment = GoalCalorieAdjustment[profile.FitnessGoal];
            // Мінімальний безпечний поріг — 1200 ккал (жінки) / 1500 ккал (чоловіки)
            double minimum = profile.Gender == Gender.Male ? 1500 : 1200;
            return Math.Max(minimum, Math.Round(tdee + adjustment, 0));
        }

        // ======================================================================
        // 2. РОЗРАХУНОК ЦІЛЕЙ ЗА МАКРОНУТРІЄНТАМИ
        // ======================================================================

        /// <summary>
        /// Генерує персональні цілі за макронутрієнтами на основі цільового калоражу.
        /// Розподіл AMDR: Білки 30% / Вуглеводи 40% / Жири 30%
        /// </summary>
        public (double ProteinG, double CarbsG, double FatsG) CalculateMacroGoals(UserProfile profile)
        {
            double targetCalories = CalculateDailyCalorieTarget(profile);

            // 4 ккал/г для білків та вуглеводів, 9 ккал/г для жирів
            double proteinG = Math.Round(targetCalories * 0.30 / 4.0, 0);
            double carbsG   = Math.Round(targetCalories * 0.40 / 4.0, 0);
            double fatsG    = Math.Round(targetCalories * 0.30 / 9.0, 0);

            return (proteinG, carbsG, fatsG);
        }

        // ======================================================================
        // 3. АГРЕГАЦІЯ ЩОДЕННОГО ХАРЧОВОГО ЖУРНАЛУ
        // ======================================================================

        /// <summary>
        /// Агрегує усі записи прийомів їжі за вказану дату в об'єкт NutrientData.
        /// Заповнює і макро-, і мікронутрієнти.
        /// </summary>
        public NutrientData AggregateDailyNutrition(
            UserProfile profile,
            IEnumerable<MealEntry> entries,
            DateTime? date = null)
        {
            var targetDate = (date ?? DateTime.Today).Date;
            var dayEntries = entries
                .Where(e => e.ConsumedAt.Date == targetDate)
                .ToList();

            var (proteinGoal, carbsGoal, fatsGoal) = CalculateMacroGoals(profile);

            var data = new NutrientData
            {
                CalorieGoal   = CalculateDailyCalorieTarget(profile),
                ProteinGoalG  = proteinGoal,
                CarbsGoalG    = carbsGoal,
                FatsGoalG     = fatsGoal,
            };

            foreach (var entry in dayEntries)
            {
                var p = entry.Product;
                double factor = entry.QuantityGrams / 100.0;

                data.CaloriesConsumed  += Math.Round(p.Calories   * factor, 1);
                data.ProteinConsumed   += Math.Round(p.Protein    * factor, 1);
                data.CarbsConsumed     += Math.Round(p.Carbs      * factor, 1);
                data.FatsConsumed      += Math.Round(p.Fats       * factor, 1);
                data.FiberConsumed     += Math.Round(p.Fiber      * factor, 1);
                data.SugarConsumed     += Math.Round(p.Sugar      * factor, 1);

                // Мікронутрієнти
                data.VitaminC_mg   += Math.Round(p.VitaminC   * factor, 2);
                data.VitaminD_mcg  += Math.Round(p.VitaminD   * factor, 2);
                data.VitaminB12_mcg+= Math.Round(p.VitaminB12 * factor, 2);
                data.Iron_mg       += Math.Round(p.Iron       * factor, 2);
                data.Calcium_mg    += Math.Round(p.Calcium    * factor, 1);
                data.Magnesium_mg  += Math.Round(p.Magnesium  * factor, 1);
            }

            // Фінальне округлення агрегатів
            data.CaloriesConsumed = Math.Round(data.CaloriesConsumed, 1);

            return data;
        }

        // ======================================================================
        // 4. МОДУЛЬ «РАНОК-ВЕЧІР»
        // ======================================================================

        /// <summary>
        /// Аналізує часовий розподіл вуглеводів та калорій протягом дня.
        /// Генерує попередження якщо вечірні (після 20:00) вуглеводи > 40% добового споживання.
        /// </summary>
        /// <param name="entries">Усі записи прийомів їжі за день.</param>
        /// <param name="eveningStartHour">Година початку «вечірнього» вікна (за замовч. 20).</param>
        /// <param name="warningThresholdPercent">Поріг вуглеводів у відсотках (за замовч. 40%).</param>
        public MorningEveningAnalysis AnalyzeMorningEvening(
            IEnumerable<MealEntry> entries,
            int eveningStartHour = 20,
            double warningThresholdPercent = 40.0)
        {
            var list = entries.ToList();

            double totalCarbs    = list.Sum(e => e.CarbsTotal);
            double totalCalories = list.Sum(e => e.CaloriesTotal);

            var eveningEntries = list.Where(e => e.ConsumedAt.Hour >= eveningStartHour).ToList();
            double eveningCarbs    = eveningEntries.Sum(e => e.CarbsTotal);
            double eveningCalories = eveningEntries.Sum(e => e.CaloriesTotal);

            double eveningCarbsPct    = totalCarbs    > 0 ? eveningCarbs    / totalCarbs    * 100.0 : 0;
            double eveningCaloriesPct = totalCalories > 0 ? eveningCalories / totalCalories * 100.0 : 0;

            bool warning = eveningCarbsPct > warningThresholdPercent;

            string message = warning
                ? $"⚠️ Увага! {eveningCarbsPct:F0}% добових вуглеводів спожито після {eveningStartHour}:00. " +
                  $"Рекомендується перенести вуглеводні страви на першу половину дня."
                : $"✅ Розподіл вуглеводів протягом дня в нормі ({eveningCarbsPct:F0}% — після {eveningStartHour}:00).";

            return new MorningEveningAnalysis
            {
                EveningCarbsPercent    = Math.Round(eveningCarbsPct, 1),
                EveningCaloriesPercent = Math.Round(eveningCaloriesPct, 1),
                EveningCarbsGrams      = Math.Round(eveningCarbs, 1),
                EveningCaloriesKcal    = Math.Round(eveningCalories, 1),
                EveningCarbsWarning    = warning,
                Message                = message
            };
        }

        // ======================================================================
        // 5. ДОПОМІЖНІ МЕТОДИ
        // ======================================================================

        /// <summary>
        /// Повертає читабельний опис ІМТ (Індекс маси тіла).
        /// </summary>
        public (double Bmi, string Category) CalculateBmi(UserProfile profile)
        {
            double heightM = profile.HeightCm / 100.0;
            double bmi = Math.Round(profile.WeightKg / (heightM * heightM), 1);

            string category = bmi switch
            {
                < 18.5 => "Недостатня вага",
                < 25.0 => "Нормальна вага",
                < 30.0 => "Надмірна вага",
                _      => "Ожиріння"
            };

            return (bmi, category);
        }

        /// <summary>
        /// Генерує підсумковий текст рекомендації за поточним балансом калорій.
        /// </summary>
        public string GetCalorieBalanceSummary(NutrientData data)
        {
            double balance = data.CaloriesBalance;

            return balance switch
            {
                > 0  => $"✅ Залишок: {balance:F0} ккал. Продовжуйте в тому ж дусі!",
                < -50=> $"🔴 Перевищення норми на {Math.Abs(balance):F0} ккал. Зверніть увагу на раціон.",
                _    => "🎯 Ви досягли денної норми калорій!"
            };
        }
    }
}
