namespace PrackticFit.Models
{
    /// <summary>
    /// Стать користувача для розрахунку BMR за формулою Міффліна-Сан Жеора.
    /// </summary>
    public enum Gender { Male, Female }

    /// <summary>
    /// Рівень фізичної активності (коефіцієнт Харіса-Бенедикта / PAL).
    /// </summary>
    public enum ActivityLevel
    {
        Sedentary,       // 1.2  — малорухливий спосіб життя
        LightlyActive,   // 1.375 — легкі навантаження 1-3 дні/тиждень
        ModeratelyActive,// 1.55  — помірні навантаження 3-5 днів/тиждень
        VeryActive,      // 1.725 — інтенсивні навантаження 6-7 днів/тиждень
        ExtraActive      // 1.9   — дуже важка фізична праця або двічі на день
    }

    /// <summary>
    /// Ціль користувача, що визначає коригування добового калоражу.
    /// </summary>
    public enum FitnessGoal
    {
        WeightLoss,     // Дефіцит -500 ккал (безпечне схуднення ~0.5 кг/тиждень)
        MaintainWeight, // Без коригування
        MuscleGain      // Профіцит +300 ккал (набір м'язової маси)
    }

    /// <summary>
    /// Антропометричні дані та налаштування профілю користувача.
    /// </summary>
    public class UserProfile
    {
        public string Name { get; set; } = "Користувач";
        public int Age { get; set; } = 25;

        /// <summary>Вага в кілограмах.</summary>
        public double WeightKg { get; set; } = 75.0;

        /// <summary>Зріст в сантиметрах.</summary>
        public double HeightCm { get; set; } = 175.0;

        public Gender Gender { get; set; } = Gender.Male;
        public ActivityLevel ActivityLevel { get; set; } = ActivityLevel.ModeratelyActive;
        public FitnessGoal FitnessGoal { get; set; } = FitnessGoal.MaintainWeight;
    }
}
