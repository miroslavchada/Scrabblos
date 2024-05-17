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
        Instance = this;
    }

    public static HomeView Instance { get; private set; }

    public Action<string[]> OnStartGame;

    private List<string> players = new();

    private void CloseApp_OnClick(object sender, RoutedEventArgs e) {
        Environment.Exit(0);
    }

    private void StartGame_OnClick(object sender, RoutedEventArgs e) {
        OnStartGame?.Invoke(players.ToArray());
    }

    private void PlayerTextBox_OnTextChanged(object sender, TextChangedEventArgs e) {
        TextBox[] playerTB = { Player1TextBox, Player2TextBox, Player3TextBox, Player4TextBox };
        TextBlock[] playerLb = { Player1Label, Player2Label, Player3Label, Player4Label };

        for (int i = 1; i < playerTB.Length; i++) {
            // Disables empty TB (not 1st player)
            playerTB[i].IsEnabled = playerTB[i].Text != "";

            // Grays off label above an empty TB (not 1st player)
            playerLb[i].Foreground = playerTB[i].Text != "" ? Brushes.Black : Brushes.DarkGray;
        }

        for (int i = 0; i < playerTB.Length - 1; i++) {
            // Enables 1 TB after 1st not empty TB
            if (playerTB[i].Text != "" && playerTB[i + 1].Text == "") {
                playerTB[i + 1].IsEnabled = true;
                playerLb[i + 1].Foreground = Brushes.Black;
                break;
            }
        }

        // Allows player to start the game if at least one name is filled
        bool atLeastOneFilled = false;
        players.Clear();
        foreach (TextBox tb in playerTB) {
            if (tb.Text != "") {
                atLeastOneFilled = true;
                players.Add(tb.Text);
                break;
            }
        }
        StartGame.IsEnabled = atLeastOneFilled;
    }
}