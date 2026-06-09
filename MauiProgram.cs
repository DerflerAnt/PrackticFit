using Microsoft.Extensions.Logging;
using PrackticFit.Services;
using PrackticFit.ViewModels;
using PrackticFit.Views;

namespace PrackticFit;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Services
		builder.Services.AddSingleton<NutritionCalculatorService>();

		// ViewModels
		builder.Services.AddTransient<DashboardViewModel>();
		builder.Services.AddTransient<ProfileViewModel>();
		builder.Services.AddTransient<SearchViewModel>();

		// Views
		builder.Services.AddTransient<DashboardPage>();
		builder.Services.AddTransient<ProfilePage>();
		builder.Services.AddTransient<SearchPage>();

		return builder.Build();
	}
}
