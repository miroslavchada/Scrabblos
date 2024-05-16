using System.Windows;
using System.Windows.Controls;

namespace Scrabblos.MVVM.View;

/// <summary>
/// Interakční logika pro HomeView.xaml
/// </summary>
public partial class HomeView : UserControl {
    public HomeView() {
        InitializeComponent();
        Instance = this;
    }

    public static HomeView Instance { get; private set; }

    public Action<string[]> OnStartGame;

    private void CloseApp_OnClick(object sender, RoutedEventArgs e) {
        Environment.Exit(0);
    }

    private void StartGame_OnClick(object sender, RoutedEventArgs e) {
        throw new NotImplementedException();
    }

    private void PlayerTextBox_OnTextChanged(object sender, TextChangedEventArgs e) {
        throw new NotImplementedException();
    }
}