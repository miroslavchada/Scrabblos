using Scrabblos.Core;
using Scrabblos.Services;

namespace Scrabblos.MVVM.ViewModel;

internal class GameViewModel : Core.ViewModel
{
    private INavigationService _navigation;

    public INavigationService Navigation
    {
        get => _navigation;
        set
        {
            _navigation = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand NavigateToHomeCommand { get; set; } // Příkaz pro přepnutí View

    public GameViewModel(INavigationService navService)
    {
        Navigation = navService;
        NavigateToHomeCommand = new RelayCommand(o => { Navigation.NavigateTo<HomeViewModel>(); }, o => true); // Definování příkazu
    }
}