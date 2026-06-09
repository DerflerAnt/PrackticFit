namespace PrackticFit.Models
{
    /// <summary>
    /// Запис про один прийом їжі в харчовому щоденнику.
    /// </summary>
    public class MealEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Продукт, що був спожитий.</summary>
        public FoodProduct Product { get; set; }

        /// <summary>Кількість в грамах.</summary>
        public double QuantityGrams { get; set; } = 100.0;

        /// <summary>Час споживання.</summary>
        public DateTime ConsumedAt { get; set; } = DateTime.Now;

        // ---- Розраховані макроси на реальну кількість ----

        public double CaloriesTotal =>
            (Product?.Calories ?? 0) * QuantityGrams / 100.0;

        public double ProteinTotal =>
            (Product?.Protein ?? 0) * QuantityGrams / 100.0;

        public double CarbsTotal =>
            (Product?.Carbs ?? 0) * QuantityGrams / 100.0;

        public double FatsTotal =>
            (Product?.Fats ?? 0) * QuantityGrams / 100.0;

        /// <summary>
        /// Повертає true якщо прийом їжі відбувся у вечірній час (після 20:00).
        /// </summary>
        public bool IsEveningMeal => ConsumedAt.Hour >= 20;
    }
}
