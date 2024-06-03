using System.Windows;
using System.Windows.Input;

namespace Scrabblos;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        Instance = this;
    }

    public static MainWindow Instance { get; private set; }

    public Action<Key> OnAnyKeyDown;

    private bool _fullscreen = true;

    private void MainWindow_OnKeyDown(object sender, KeyEventArgs e) {
        switch (e.Key) {
            case Key.F11:
                if (_fullscreen) {
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    WindowState = WindowState.Normal;
                    ResizeMode = ResizeMode.CanResize;
                }
                else {
                    WindowStyle = WindowStyle.None;
                    WindowState = WindowState.Maximized;
                    ResizeMode = ResizeMode.NoResize;
                }
                _fullscreen = !_fullscreen;
                break;
            default:
                OnAnyKeyDown?.Invoke(e.Key);
                break;
        }
    }
}
