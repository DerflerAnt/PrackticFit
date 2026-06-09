using Microsoft.Maui.Controls;
using PrackticFit.ViewModels;

namespace PrackticFit.Views
{
    public partial class SearchPage : ContentPage
    {
        public SearchPage(SearchViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
