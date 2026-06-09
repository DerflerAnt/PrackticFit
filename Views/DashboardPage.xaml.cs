using Microsoft.Maui.Controls;
using PrackticFit.ViewModels;

namespace PrackticFit.Views
{
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage(DashboardViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
