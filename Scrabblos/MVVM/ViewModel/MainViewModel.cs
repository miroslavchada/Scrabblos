using Scrabblos.Core;
using Scrabblos.Services;

namespace Scrabblos.MVVM.ViewModel;

internal class MainViewModel : Core.ViewModel {
    private INavigationService _navigation;

    public INavigationService Navigation {
        get => _navigation;
        set {
            _navigation = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand NavigateToHomeCommand { get; set; }

    public MainViewModel(INavigationService navService) {
        Navigation = navService;
        NavigateToHomeCommand = new RelayCommand(o => { Navigation.NavigateTo<HomeViewModel>(); }, o => true);

        // Navigate to HomeViewModel after initialization
        NavigateToHomeCommand.Execute(null);
    }
}