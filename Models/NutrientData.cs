namespace PrackticFit.Models
{
    /// <summary>
    /// Щоденні норми та фактичне споживання калорій та нутрієнтів.
    /// </summary>
    public class NutrientData
    {
        // ---- Калорії ----
        public double CalorieGoal { get; set; } = 2000;
        public double CaloriesConsumed { get; set; }

        // ---- Макронутрієнти (спожито) ----
        public double ProteinConsumed { get; set; }
        public double CarbsConsumed { get; set; }
        public double FatsConsumed { get; set; }
        public double FiberConsumed { get; set; }
        public double SugarConsumed { get; set; }

        // ---- Макронутрієнти (норми) ----
        public double ProteinGoalG { get; set; } = 150;
        public double CarbsGoalG { get; set; } = 250;
        public double FatsGoalG { get; set; } = 70;

        // ---- Мікронутрієнти (спожито) ----
        public double VitaminC_mg { get; set; }
        public double VitaminD_mcg { get; set; }
        public double VitaminB12_mcg { get; set; }
        public double Iron_mg { get; set; }
        public double Calcium_mg { get; set; }
        public double Magnesium_mg { get; set; }

        // ---- Мікронутрієнти (норми EFSA / МОЗ України) ----
        public double VitaminC_Goal { get; set; } = 90;    // мг
        public double VitaminD_Goal { get; set; } = 15;    // мкг
        public double VitaminB12_Goal { get; set; } = 2.4; // мкг
        public double Iron_Goal { get; set; } = 14;        // мг
        public double Calcium_Goal { get; set; } = 1000;   // мг
        public double Magnesium_Goal { get; set; } = 375;  // мг

        // ---- Обчислювані властивості ----

        /// <summary>Залишок калорій (може бути від'ємним при перевищенні).</summary>
        public double CaloriesBalance => CalorieGoal - CaloriesConsumed;

        /// <summary>Залишок для зручного відображення (мін. 0).</summary>
        public double RemainingCalories => Math.Max(0, CaloriesBalance);

        /// <summary>Дефіцит/профіцит Вітаміну C в мг.</summary>
        public double VitaminC_Balance => VitaminC_mg - VitaminC_Goal;

        /// <summary>Дефіцит/профіцит Вітаміну D в мкг.</summary>
        public double VitaminD_Balance => VitaminD_mcg - VitaminD_Goal;

        /// <summary>Дефіцит/профіцит Вітаміну B12 в мкг.</summary>
        public double VitaminB12_Balance => VitaminB12_mcg - VitaminB12_Goal;

        /// <summary>Дефіцит/профіцит Заліза в мг.</summary>
        public double Iron_Balance => Iron_mg - Iron_Goal;

        /// <summary>Дефіцит/профіцит Кальцію в мг.</summary>
        public double Calcium_Balance => Calcium_mg - Calcium_Goal;

        /// <summary>Дефіцит/профіцит Магнію в мг.</summary>
        public double Magnesium_Balance => Magnesium_mg - Magnesium_Goal;
    }
}
