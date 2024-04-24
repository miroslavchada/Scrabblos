using System.Windows.Controls;
using System.Windows.Input;

namespace Scrabblos.MVVM.View;

/// <summary>
/// Interakční logika pro GameView.xaml
/// </summary>
public partial class GameView : UserControl {
    public GameView() {
        InitializeComponent();

        MainWindow.Instance.OnAnyKeyDown += OnAnyKeyDown;
    }

    private void OnAnyKeyDown(Key key) {
        throw new NotImplementedException();
    }
}