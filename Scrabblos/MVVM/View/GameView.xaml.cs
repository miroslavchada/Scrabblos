using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Scrabblos.MVVM.View;

/// <summary>
/// Interakční logika pro GameView.xaml
/// </summary>
public partial class GameView : UserControl {
    public GameView() {
        InitializeComponent();
    }

    private bool escapeMenu = false;

    private void GameView_OnLoaded(object sender, RoutedEventArgs e) {
        MainWindow.Instance.OnAnyKeyDown += OnAnyKeyDown;
    }

    private void GameView_OnUnloaded(object sender, RoutedEventArgs e) {
        MainWindow.Instance.OnAnyKeyDown -= OnAnyKeyDown;
    }

    private void OnAnyKeyDown(Key key) {
        switch (key) {
            case Key.Escape:
                if (!escapeMenu) {
                    EscapeMenu.IsEnabled = true;
                    EscapeMenu.Visibility = Visibility.Visible;
                } else {
                    EscapeMenu.IsEnabled = false;
                    EscapeMenu.Visibility = Visibility.Hidden;
                }
                escapeMenu = !escapeMenu;
                break;
            default:
                TbPressed.Text = $"Zmáčknuté tlačítko: {key}";
                MessageBox.Show(key.ToString());
                break;
        }
    }
}