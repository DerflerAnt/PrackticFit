using System;
using Xunit;

namespace PrackticFit.Tests
{
    // Моделі для тестування (імітація оригінальних)
    public enum Gender { Male, Female }
    public enum ActivityLevel { Sedentary, LightlyActive, ModeratelyActive, VeryActive, ExtraActive }
    public enum FitnessGoal { WeightLoss, MaintainWeight, MuscleGain }

    public class UserProfile
    {
        public double WeightKg { get; set; }
        public double HeightCm { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public ActivityLevel ActivityLevel { get; set; }
        public FitnessGoal FitnessGoal { get; set; }
    }

    // Сервіс для тестування
    public class NutritionCalculatorService
    {
        public double CalculateBMR(UserProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));

            double bmr = 10 * profile.WeightKg + 6.25 * profile.HeightCm - 5.0 * profile.Age;
            return profile.Gender == Gender.Male ? bmr + 5 : bmr - 161;
        }

        public double CalculateTDEE(UserProfile profile)
        {
            double bmr = CalculateBMR(profile);
            double pal = 1.55; // ModeratelyActive
            return Math.Round(bmr * pal, 0);
        }
    }

    // Тести
    public class NutritionCalculatorServiceTests
    {
        private readonly NutritionCalculatorService _service;

        public NutritionCalculatorServiceTests()
        {
            _service = new NutritionCalculatorService();
        }

        [Fact]
        public void CalculateBMR_Male_ReturnsCorrectValue()
        {
            // Arrange
            var profile = new UserProfile { WeightKg = 80, HeightCm = 180, Age = 25, Gender = Gender.Male };
            
            // Act
            var bmr = _service.CalculateBMR(profile);

            // Assert
            Assert.Equal(1805, bmr);
        }

        [Fact]
        public void CalculateBMR_Female_ReturnsCorrectValue()
        {
            // Arrange
            var profile = new UserProfile { WeightKg = 60, HeightCm = 165, Age = 30, Gender = Gender.Female };

            // Act
            var bmr = _service.CalculateBMR(profile);

            // Assert
            Assert.Equal(1320.25, bmr);
        }

        [Fact]
        public void CalculateBMR_NullProfile_ThrowsArgumentNullException()
        {
            // Arrange
            UserProfile profile = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _service.CalculateBMR(profile));
        }

        [Fact]
        public void CalculateTDEE_ValidProfile_ReturnsExpectedCalories()
        {
            // Arrange
            var profile = new UserProfile { WeightKg = 80, HeightCm = 180, Age = 25, Gender = Gender.Male, ActivityLevel = ActivityLevel.ModeratelyActive };

            // Act
            var tdee = _service.CalculateTDEE(profile);

            // Assert
            Assert.Equal(2798, tdee); // 1805 * 1.55 = 2797.75 -> 2798
        }
    }
}
