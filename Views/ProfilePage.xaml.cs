using Microsoft.Maui.Controls;
using PrackticFit.ViewModels;

namespace PrackticFit.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage(ProfileViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
