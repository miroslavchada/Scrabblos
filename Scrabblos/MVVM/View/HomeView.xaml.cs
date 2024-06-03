using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scrabblos.MVVM.View;

/// <summary>
/// Interakční logika pro HomeView.xaml
/// </summary>
public partial class HomeView : UserControl {
    public HomeView() {
        InitializeComponent();
    }


    private readonly List<string> _players = new();

    private void StartGame_OnClick(object sender, RoutedEventArgs e) {
        Application.Current.Properties["Players"] = _players.ToArray();
    }

    private void CloseApp_OnClick(object sender, RoutedEventArgs e) {
        Environment.Exit(0);
    }

    private void PlayerTextBox_OnTextChanged(object sender, TextChangedEventArgs e) {

        TextBox[] playerTb = { Player1TextBox, Player2TextBox, Player3TextBox, Player4TextBox };
        TextBlock[] playerLb = { Player1Label, Player2Label, Player3Label, Player4Label };

        for (int i = 1; i < playerTb.Length; i++) {
            // Disables empty TB (not 1st player)
            playerTb[i].IsEnabled = playerTb[i].Text != "";

            // Grays off label above an empty TB (not 1st player)
            playerLb[i].Foreground = playerTb[i].Text != "" ? Brushes.Black : Brushes.DarkGray;
        }

        for (int i = 0; i < playerTb.Length - 1; i++) {
            // Enables 1 TB after 1st not empty TB
            if (playerTb[i].Text.Trim() != "" && playerTb[i + 1].Text == "") {
                playerTb[i + 1].IsEnabled = true;
                playerLb[i + 1].Foreground = Brushes.Black;
                break;
            }
        }

        // Allows player to start the game if at least one Name is filled
        _players.Clear();
        bool atLeastOneFilled = false;
        foreach (TextBox tb in playerTb) {
            if (tb.Text.Trim() != "") {
                atLeastOneFilled = true;
                _players.Add(tb.Text.Trim());
            }
        }
        StartGame.IsEnabled = atLeastOneFilled;
    }
}
