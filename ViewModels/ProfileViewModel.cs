using System.Windows.Input;
using PrackticFit.Models;
using PrackticFit.Services;

namespace PrackticFit.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly NutritionCalculatorService _calculatorService;
        private UserProfile _profile;
        private double _tdee;
        private double _bmr;

        public ProfileViewModel(NutritionCalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
            
            // Ініціалізуємо профіль
            _profile = new UserProfile
            {
                Name = "Олександр",
                WeightKg = 75,
                HeightCm = 180,
                Age = 25,
                Gender = Gender.Male,
                ActivityLevel = ActivityLevel.ModeratelyActive,
                FitnessGoal = FitnessGoal.MaintainWeight
            };

            SaveCommand = new Command(SaveProfile);
            Recalculate();
        }

        public string Name
        {
            get => _profile.Name;
            set { _profile.Name = value; OnPropertyChanged(); }
        }

        public double Weight
        {
            get => _profile.WeightKg;
            set { _profile.WeightKg = value; OnPropertyChanged(); Recalculate(); }
        }

        public double Height
        {
            get => _profile.HeightCm;
            set { _profile.HeightCm = value; OnPropertyChanged(); Recalculate(); }
        }

        public int Age
        {
            get => _profile.Age;
            set { _profile.Age = value; OnPropertyChanged(); Recalculate(); }
        }

        public double TDEE
        {
            get => _tdee;
            set => SetProperty(ref _tdee, value);
        }

        public double BMR
        {
            get => _bmr;
            set => SetProperty(ref _bmr, value);
        }

        public ICommand SaveCommand { get; }

        private void Recalculate()
        {
            BMR = _calculatorService.CalculateBMR(_profile);
            TDEE = _calculatorService.CalculateTDEE(_profile);
        }

        private void SaveProfile()
        {
            // У майбутньому тут буде запит до бекенду
            Application.Current.MainPage.DisplayAlert("Збережено", $"Ваші дані успішно оновлено! Ваша денна норма: {_calculatorService.CalculateDailyCalorieTarget(_profile)} ккал", "OK");
        }
    }
}
