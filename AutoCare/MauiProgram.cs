using Microsoft.Extensions.Logging;
using AutoCare.Services;
using AutoCare.ViewModels;
using AutoCare.Views;
namespace AutoCare
{
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
            builder.Services.AddSingleton<AutoCare.Services.DatabaseService>();
            builder.Services.AddSingleton<DatabaseService>();

            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<AddServiceViewModel>();
            builder.Services.AddTransient<HistoryViewModel>();
            builder.Services.AddTransient<CalculatorViewModel>();
            builder.Services.AddTransient<AlertsViewModel>();

            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<AddServicePage>();
            builder.Services.AddTransient<HistoryPage>();
            builder.Services.AddTransient<CalculatorPage>();
            builder.Services.AddTransient<AlertsPage>();
            builder.Services.AddTransient<ServiceIntervalsViewModel>();
            builder.Services.AddTransient<ServiceIntervalsPage>();
            return builder.Build();
        }
    }
}
