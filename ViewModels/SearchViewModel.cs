using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PrackticFit.Models;

namespace PrackticFit.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        private string _searchQuery;
        private ObservableCollection<FoodProduct> _allProducts;
        private ObservableCollection<FoodProduct> _filteredProducts;

        public SearchViewModel()
        {
            // Імітація бази даних продуктів
            _allProducts = new ObservableCollection<FoodProduct>
            {
                new FoodProduct { Name = "Куряча грудка", Calories = 165, Protein = 31, Carbs = 0, Fats = 3, Category = "М'ясо" },
                new FoodProduct { Name = "Гречка (варена)", Calories = 132, Protein = 4, Carbs = 28, Fats = 1, Category = "Крупи" },
                new FoodProduct { Name = "Яйце куряче", Calories = 155, Protein = 13, Carbs = 1, Fats = 11, Category = "Яйця" },
                new FoodProduct { Name = "Банан", Calories = 89, Protein = 1, Carbs = 23, Fats = 0, Category = "Фрукти" },
                new FoodProduct { Name = "Сир кисломолочний 5%", Calories = 121, Protein = 17, Carbs = 1, Fats = 5, Category = "Молочні" },
                new FoodProduct { Name = "Авокадо", Calories = 160, Protein = 2, Carbs = 8, Fats = 14, Category = "Фрукти" }
            };

            FilteredProducts = new ObservableCollection<FoodProduct>(_allProducts);
            PerformSearchCommand = new Command(FilterProducts);
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set 
            {
                if (SetProperty(ref _searchQuery, value))
                {
                    FilterProducts();
                }
            }
        }

        public ObservableCollection<FoodProduct> FilteredProducts
        {
            get => _filteredProducts;
            set => SetProperty(ref _filteredProducts, value);
        }

        public ICommand PerformSearchCommand { get; }

        private void FilterProducts()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                FilteredProducts = new ObservableCollection<FoodProduct>(_allProducts);
            }
            else
            {
                var lowerQuery = SearchQuery.ToLower();
                var filtered = _allProducts.Where(p => p.Name.ToLower().Contains(lowerQuery) || p.Category.ToLower().Contains(lowerQuery));
                FilteredProducts = new ObservableCollection<FoodProduct>(filtered);
            }
        }
    }
}
