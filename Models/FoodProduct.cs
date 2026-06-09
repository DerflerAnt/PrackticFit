namespace PrackticFit.Models
{
    /// <summary>
    /// Харчовий продукт із макро- та мікронутрієнтами на 100 г.
    /// </summary>
    public class FoodProduct
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        // ---- Макронутрієнти (на 100 г) ----
        public double Calories { get; set; }   // ккал
        public double Protein { get; set; }    // г
        public double Carbs { get; set; }      // г
        public double Fats { get; set; }       // г
        public double Fiber { get; set; }      // г — харчові волокна
        public double Sugar { get; set; }      // г — цукри

        // ---- Мікронутрієнти (на 100 г) ----
        public double VitaminC { get; set; }   // мг
        public double VitaminD { get; set; }   // мкг
        public double VitaminB12 { get; set; } // мкг
        public double Iron { get; set; }       // мг
        public double Calcium { get; set; }    // мг
        public double Magnesium { get; set; }  // мг

        /// <summary>
        /// Повертає копію продукту з масштабованими значеннями для зазначеної кількості в грамах.
        /// </summary>
        public FoodProduct ScaleToGrams(double grams)
        {
            double factor = grams / 100.0;
            return new FoodProduct
            {
                Name = Name,
                Category = Category,
                Calories   = Math.Round(Calories   * factor, 1),
                Protein    = Math.Round(Protein    * factor, 1),
                Carbs      = Math.Round(Carbs      * factor, 1),
                Fats       = Math.Round(Fats       * factor, 1),
                Fiber      = Math.Round(Fiber      * factor, 1),
                Sugar      = Math.Round(Sugar      * factor, 1),
                VitaminC   = Math.Round(VitaminC   * factor, 2),
                VitaminD   = Math.Round(VitaminD   * factor, 2),
                VitaminB12 = Math.Round(VitaminB12 * factor, 2),
                Iron       = Math.Round(Iron       * factor, 2),
                Calcium    = Math.Round(Calcium    * factor, 1),
                Magnesium  = Math.Round(Magnesium  * factor, 1),
            };
        }
    }
}
