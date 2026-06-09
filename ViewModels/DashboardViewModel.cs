using System.Collections.ObjectModel;
using System.Windows.Input;
using PrackticFit.Models;
using PrackticFit.Services;

namespace PrackticFit.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly NutritionCalculatorService _calculatorService;
        private NutrientData _nutrition;
        private UserProfile _profile;
        private ObservableCollection<MealEntry> _mealEntries;
        private string _eveningAnalysisMessage;

        public DashboardViewModel(NutritionCalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
            
            // Імітація профілю користувача
            _profile = new UserProfile
            {
                Name = "Олександр",
                Age = 25,
                WeightKg = 75,
                HeightCm = 180,
                Gender = Gender.Male,
                ActivityLevel = ActivityLevel.ModeratelyActive,
                FitnessGoal = FitnessGoal.MaintainWeight
            };

            // Імітація щоденника харчування з даними для наповненого вигляду
            _mealEntries = new ObservableCollection<MealEntry>
            {
                new MealEntry 
                { 
                    Product = new FoodProduct { Name = "Вівсяна каша з ягодами", Calories = 320, Protein = 12, Fats = 6, Carbs = 55 },
                    QuantityGrams = 200, 
                    ConsumedAt = DateTime.Today.AddHours(8) 
                },
                new MealEntry 
                { 
                    Product = new FoodProduct { Name = "Куряче філе на грилі", Calories = 165, Protein = 31, Fats = 3.6, Carbs = 0 },
                    QuantityGrams = 150, 
                    ConsumedAt = DateTime.Today.AddHours(13) 
                },
                new MealEntry 
                { 
                    Product = new FoodProduct { Name = "Гречка відварена", Calories = 130, Protein = 5, Fats = 1, Carbs = 28 },
                    QuantityGrams = 150, 
                    ConsumedAt = DateTime.Today.AddHours(13) 
                }
            };

            AddSnackCommand = new Command(AddSnack);
            AddWaterCommand = new Command(() => WaterGlasses++);
            RemoveWaterCommand = new Command(() => { if (WaterGlasses > 0) WaterGlasses--; });
            
            RefreshDashboard();
        }

        public NutrientData Nutrition
        {
            get => _nutrition;
            set 
            {
                if (SetProperty(ref _nutrition, value))
                {
                    OnPropertyChanged(nameof(ProteinProgress));
                    OnPropertyChanged(nameof(CarbsProgress));
                    OnPropertyChanged(nameof(FatsProgress));
                }
            }
        }

        public string EveningAnalysisMessage
        {
            get => _eveningAnalysisMessage;
            set => SetProperty(ref _eveningAnalysisMessage, value);
        }

        private bool _hasEveningWarning;
        public bool HasEveningWarning
        {
            get => _hasEveningWarning;
            set => SetProperty(ref _hasEveningWarning, value);
        }

        // Значення для ProgressBar від 0.0 до 1.0
        public double ProteinProgress => Nutrition?.ProteinGoalG > 0 ? Math.Min(1.0, Nutrition.ProteinConsumed / Nutrition.ProteinGoalG) : 0;
        public double CarbsProgress => Nutrition?.CarbsGoalG > 0 ? Math.Min(1.0, Nutrition.CarbsConsumed / Nutrition.CarbsGoalG) : 0;
        public double FatsProgress => Nutrition?.FatsGoalG > 0 ? Math.Min(1.0, Nutrition.FatsConsumed / Nutrition.FatsGoalG) : 0;

        public ObservableCollection<MealEntry> Meals => _mealEntries;

        private int _waterGlasses = 3;
        public int WaterGlasses
        {
            get => _waterGlasses;
            set 
            {
                if (SetProperty(ref _waterGlasses, value))
                {
                    OnPropertyChanged(nameof(WaterConsumedMl));
                }
            }
        }

        public int WaterConsumedMl => WaterGlasses * 250;

        public ICommand AddSnackCommand { get; }
        public ICommand AddWaterCommand { get; }
        public ICommand RemoveWaterCommand { get; }

        private void AddSnack()
        {
            // Симуляція додавання перекусу
            var snack = new FoodProduct
            {
                Name = "Перекус (Мікс горіхів)",
                Calories = 250,
                Protein = 10,
                Carbs = 30,
                Fats = 8
            };

            var entry = new MealEntry
            {
                Product = snack,
                QuantityGrams = 100,
                ConsumedAt = DateTime.Now // Можна симулювати вечірній час для тесту
            };

            _mealEntries.Add(entry);
            RefreshDashboard();
        }

        private void RefreshDashboard()
        {
            // Обчислення агрегованих даних
            Nutrition = _calculatorService.AggregateDailyNutrition(_profile, _mealEntries);

            // Аналіз "Ранок-Вечір"
            var analysis = _calculatorService.AnalyzeMorningEvening(_mealEntries);
            EveningAnalysisMessage = analysis.Message;
            HasEveningWarning = analysis.EveningCarbsWarning;
        }
    }
}
