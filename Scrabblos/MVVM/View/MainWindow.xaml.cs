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

    public Action<Key> OnAnyKeyDown;

    public static MainWindow Instance { get; private set; }

    private void MainWindow_OnKeyDown(object sender, KeyEventArgs e) {
        OnAnyKeyDown?.Invoke(e.Key);
    }
}