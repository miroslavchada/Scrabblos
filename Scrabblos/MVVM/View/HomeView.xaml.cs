using System.Windows;
using System.Windows.Controls;

namespace Scrabblos.MVVM.View;

/// <summary>
/// Interakční logika pro HomeView.xaml
/// </summary>
public partial class HomeView : UserControl {
    public HomeView() {
        InitializeComponent();
    }

    private void CloseApp_OnClick(object sender, RoutedEventArgs e)
    {
        Environment.Exit(0);
    }
}