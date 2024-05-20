using Microsoft.Extensions.DependencyInjection;
using Scrabblos.Core;
using Scrabblos.MVVM.ViewModel;
using Scrabblos.Services;
using System.Windows;

namespace Scrabblos;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    private readonly ServiceProvider _serviceProvider;

    public App() {
        IServiceCollection services = new ServiceCollection();

        // Register ViewModels
        services.AddSingleton<MainWindow>(provider => new MainWindow {
            DataContext = provider.GetRequiredService<MainViewModel>()
        });
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<HomeViewModel>();
        services.AddSingleton<GameViewModel>();

        // Register navigation service
        services.AddSingleton<INavigationService, NavigationService>();
        // Register navigation function
        services.AddSingleton<Func<Type, ViewModel>>(serviceProvider => viewModelType => (ViewModel)serviceProvider.GetRequiredService(viewModelType));

        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);
        MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}