using Scrabblos.Core;
using Scrabblos.Services;

namespace Scrabblos.MVVM.ViewModel;

internal class HomeViewModel : Core.ViewModel {

    private INavigationService _navigation;

    public INavigationService Navigation {
        get => _navigation;
        set {
            _navigation = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand NavigateToGameCommand { get; set; } // Příkaz pro přepnutí View

    public HomeViewModel(INavigationService navService) {
        Navigation = navService;
        NavigateToGameCommand = new RelayCommand(o => { Navigation.NavigateTo<GameViewModel>(); }, o => true); // Definování příkazu
    }
}