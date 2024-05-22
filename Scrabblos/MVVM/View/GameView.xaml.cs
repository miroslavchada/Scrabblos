using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace Scrabblos.MVVM.View;

/// <summary>
/// Interakční logika pro GameView.xaml
/// </summary>
public partial class GameView : UserControl {
    public GameView() {
        InitializeComponent();
        Instance = this;

        // Set propper z-indexes
        Grid.SetZIndex(BtnNextPlayer, 2);

        // Reads the dictionary from a file in resources
        dictionary = File.ReadAllLines(Path.Combine(Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).Parent.Parent.FullName, "Resources") + "\\slovnik_utf8.tsv");

        SetPlayers(Application.Current.Properties["Players"] as string[]);
        ScoreBoardRender(); 
    }

    public static GameView Instance { get; private set; }

    private bool escapeMenu;
    private int currentSetIndex = 0;

    private Player[] playerArray;
    private int currentPlayerIndex = 0;

    private List<TileSet> sets = new() {
        new TileSet(new Dictionary<Tile, int> {
            { new Tile('A', 1), 5 },
            { new Tile('Á', 2), 2 },
            { new Tile('B', 3), 2 },
            { new Tile('C', 2), 3 },
            { new Tile('Č', 4), 1 },
            { new Tile('D', 1), 3 },
            { new Tile('Ď', 8), 1 },
            { new Tile('E', 1), 5 },
            { new Tile('É', 3), 2 },
            { new Tile('Ě', 3), 2 },
            { new Tile('F', 5), 1 },
            { new Tile('G', 5), 1 },
            { new Tile('H', 2), 3 },
            { new Tile('I', 1), 4 },
            { new Tile('Í', 2), 3 },
            { new Tile('J', 2), 2 },
            { new Tile('K', 1), 3 },
            { new Tile('L', 1), 3 },
            { new Tile('M', 2), 3 },
            { new Tile('N', 1), 5 },
            { new Tile('Ň', 6), 1 },
            { new Tile('O', 1), 6 },
            { new Tile('Ó', 7), 1 },
            { new Tile('P', 1), 3 },
            { new Tile('R', 1), 3 },
            { new Tile('Ř', 4), 2 },
            { new Tile('S', 1), 4 },
            { new Tile('Š', 4), 2 },
            { new Tile('T', 1), 4 },
            { new Tile('Ť', 7), 1 },
            { new Tile('U', 2), 3 },
            { new Tile('Ú', 5), 1 },
            { new Tile('Ů', 4), 1 },
            { new Tile('V', 1), 4 },
            { new Tile('X', 10), 1 },
            { new Tile('Y', 2), 2 },
            { new Tile('Ý', 4), 2 },
            { new Tile('Z', 2), 2 },
            { new Tile('Ž', 4), 1 },
        }, "Čeština oficiální")
    };
    private string[] dictionary;

    private TileBlock[,] playArray = new TileBlock[15, 15];
    private TileBlock[,] confirmedPlayArray = new TileBlock[15, 15];
    private TileBlock[] dockArray = new TileBlock[7];

    private enum BonusType {
        None,
        DoubleLetter,
        TripleLetter,
        DoubleWord,
        TripleWord
    }

    private BonusType[,] bonusArray = new BonusType[15, 15] {
        { BonusType.TripleWord, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.TripleWord },
        { BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None },
        { BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None },
        { BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.DoubleLetter}, 
        { BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.None },
        { BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None },
        { BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None },
        { BonusType.TripleWord, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.TripleWord },
        { BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None },
        { BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None },
        { BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.None },
        { BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.DoubleLetter},
        { BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None },
        { BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None },
        { BonusType.TripleWord, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.TripleWord }
    };

    public void SetPlayers(string[] players) {
        playerArray = new Player[players.Length];

        for (int i = 0; i < players.Length; i++) {
            // New player from Name string
            Player player = new Player(players[i]);
            SaveDock(player);
            playerArray[i] = player;
        }

        LoadDock(playerArray[currentPlayerIndex]);
    }

    #region Tile dragging

    protected bool isDragging;
    private Point clickPosition;
    private TranslateTransform originTT;

    private bool previewActive;

    private int? hoverPlayCellColumn, hoverDockCellColumn;
    private int? hoverPlayCellRow, hoverDockCellRow;

    public void TileBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
        var draggableControl = sender as Image;
        originTT = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
        isDragging = true;
        clickPosition = e.GetPosition(this);
        draggableControl.CaptureMouse();

        // From which grid the Tile is dragged is in the foreground
        Grid draggableParent = draggableControl.Parent as Grid;
        switch (draggableParent.Name) {
            case "PlayGrid":
                Panel.SetZIndex(PlayGrid, 1);
                Panel.SetZIndex(DockGrid, 0);
                break;

            case "DockGrid":
                Panel.SetZIndex(PlayGrid, 0);
                Panel.SetZIndex(DockGrid, 1);
                break;
        }

        Panel.SetZIndex(draggableControl, 6);
    }

    public void TileBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {

        isDragging = false;
        var draggable = sender as Image;
        var transform = draggable.RenderTransform as TranslateTransform ?? new TranslateTransform();

        // Snap to grid if placed correctly
        // PlayGrid
        if (hoverPlayCellColumn != null && hoverPlayCellRow != null)
        {
            // int? to int (can't be null anymore)
            int gameCellColumn = (int)hoverPlayCellColumn;
            int gameCellRow = (int)hoverPlayCellRow;

            // If there is already a Tile in the cell - do nothing
            if (playArray[gameCellColumn, gameCellRow] != null) {
                transform.X = 0;
                transform.Y = 0;
                draggable.ReleaseMouseCapture();
                Panel.SetZIndex(draggable, 5);
                return;
            }

            // Transition from DockGrid to PlayGrid
            Grid draggableParent = draggable.Parent as Grid;
            if (draggableParent != PlayGrid) {
                RemoveTileFromDockGrid(draggable as TileBlock);
                AddTileToPlayGrid(draggable as TileBlock, gameCellColumn, gameCellRow);
            }
            // Move in PlayGrid if already in PlayGrid
            else {
                MoveTileInPlayGrid(draggable as TileBlock, gameCellColumn, gameCellRow);
            }
        } // DockGrid
        else if (hoverDockCellColumn != null && hoverDockCellRow != null) {
            // int? to int (can't be null anymore)
            Grid draggableParent = draggable.Parent as Grid;
            int dockCellColumn = (int)hoverDockCellColumn;

            // If there is already a Tile in the cell
            if (dockArray[dockCellColumn] != null) {
                // Swap tiles if dragged from DockGrid
                if (draggableParent == DockGrid) {
                    SwapTilesInDockGrid(draggable as TileBlock, dockArray[dockCellColumn]);
                }
                // Snap back if not placed in dock grid
                else {
                    transform.X = 0;
                    transform.Y = 0;
                    draggable.ReleaseMouseCapture();
                    Panel.SetZIndex(draggable, 5);
                    return;
                }
            }

            // Transition from PlayGrid to DockGrid
            if (draggableParent != DockGrid) {
                RemoveTileFromPlayGrid(draggable as TileBlock);
                AddTileToDockGrid(draggable as TileBlock, dockCellColumn);
            }
            // Move in DockGrid if already in DockGrid
            else {
                MoveTileInDockGrid(draggable as TileBlock, dockCellColumn);
            }
        }

        DisablePreview();

        // Reset position in cell
        transform.X = 0;
        transform.Y = 0;
        draggable.ReleaseMouseCapture();
        Panel.SetZIndex(draggable, 5);
    }

    private Image preview = new() {
        Opacity = 0.2,

        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
        VerticalAlignment = System.Windows.VerticalAlignment.Center
    };

    private void DisablePreview() {
        if (previewActive) {
            PlayGrid.Children.Remove(preview);
            DockGrid.Children.Remove(preview);
            previewActive = false;
        }
    }

    public void TileBlock_MouseMove(object sender, MouseEventArgs e) {
        var draggableControl = sender as Image;
        if (isDragging && draggableControl != null) {
            Point currentPosition = e.GetPosition(this);
            var transform = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
            transform.X = originTT.X + (currentPosition.X - clickPosition.X) * GetActualWidthMultiplier();
            transform.Y = originTT.Y + (currentPosition.Y - clickPosition.Y) * GetActualHeightMultiplier();
            draggableControl.RenderTransform = new TranslateTransform(transform.X, transform.Y);

            preview.Source = draggableControl.Source;
            preview.Height = draggableControl.Height;
            preview.Width = draggableControl.Width;
            Margin = draggableControl.Margin;

            // Preview projection
            Grid? grid = null;
            int previewCellColumn = 0;
            int previewCellRow = 0;

            // Unify cell position
            if (hoverPlayCellColumn != null && hoverPlayCellRow != null) {
                grid = PlayGrid;
                previewCellColumn = (int)hoverPlayCellColumn;
                previewCellRow = (int)hoverPlayCellRow;
            }
            else if (hoverDockCellColumn != null && hoverDockCellRow != null) {
                grid = DockGrid;
                previewCellColumn = (int)hoverDockCellColumn;
                previewCellRow = (int)hoverDockCellRow;
            }

            if (grid != null) {
                if (!previewActive) {
                    grid.Children.Add(preview);
                    Panel.SetZIndex(preview, 4);
                }

                Grid.SetColumn(preview, previewCellColumn);
                Grid.SetRow(preview, previewCellRow);

                previewActive = true;
            }
            else {
                DisablePreview();
            }
        }
    }

    #endregion Tile dragging

    private void NextPlayer() {
        currentPlayerIndex = currentPlayerIndex >= playerArray.Length - 1 ? 0 : currentPlayerIndex + 1;
    }

    private void LoadDock(Player player) {
        // Clear grid
        foreach (TileBlock tileBlock in dockArray) {
            if (tileBlock == null) continue;
            RemoveTileFromDockGrid(tileBlock);
        }

        // Restore players dock
        for (int i = 0; i < dockArray.Length; i++) {
            AddTileToDockGrid(player.GetDock()[i], i);
        }
    }

    private void SaveDock(Player player) {
        DockGridFill();
        // Rewrite players dock
        player.SetDock(dockArray);

        // Clear grid
        foreach (TileBlock tileBlock in dockArray) {
            RemoveTileFromDockGrid(tileBlock);
        }
    }

    private void RestoreDock_Click(object sender, RoutedEventArgs e) {
        ReturnTilesToDock();
    }

    private void ReturnTilesToDock() {
        foreach (TileBlock tileBlock in GetNewPlayArray()) {
            if (tileBlock == null)
                continue;

            RemoveTileFromPlayGrid(tileBlock);
        }
        LoadDock(playerArray[currentPlayerIndex]);
    }

    private void RoundApprove_Click(object sender, RoutedEventArgs e) {
        List<(string, int)> wordsWithScore = ValidatePlay();
        List<string> invalidWords = new();
        List<string> wrongPlacedWords = new();
        int score = 0;

        string info = "";

        TileBlock[,] newPlayArray = GetNewPlayArray();

        if (wordsWithScore.Count <= 0) {
            TbInfo.Text = "Neplatný tah";
            return;
        }

        foreach (var (word, wordScore) in wordsWithScore) {
            if (word[0] == '-')
                invalidWords.Add(word.Substring(1));

            if (word[0] == '_') {
                wrongPlacedWords.Add(word.Substring(1));
            }
        }

        // If there are any invalid or wrongly placed words
        if (invalidWords.Count > 0 || wrongPlacedWords.Count > 0) {
            string infoInvalid = invalidWords.Count > 0 ? $"Neplatná slova: {string.Join(", ", invalidWords)}" : "";
            string infoWrongPlaced = wrongPlacedWords.Count > 0 ? $"Špatně položená slova: {string.Join(", ", wrongPlacedWords)}" : "";

            // Report to info bar
            if (invalidWords.Count > 0 && wrongPlacedWords.Count > 0)
                info = string.Join("\r\n", new[] { infoInvalid, infoWrongPlaced });
            else if (invalidWords.Count > 0)
                info = infoInvalid;
            else if (wrongPlacedWords.Count > 0)
                info = infoWrongPlaced;

            TbInfo.Text = info;
            return;
        }

        info += $"Slova: {string.Join(", ", wordsWithScore)}";
        TbInfo.Text = info;

        // Disables any mouse interaction with the tiles
        foreach (TileBlock tileBlock in newPlayArray) {
            if (tileBlock == null)
                continue;

            tileBlock.UnsubscribeInteraction();
        }

        Array.Copy(playArray, confirmedPlayArray, playArray.Length);

        ControlsGrid.IsEnabled = false;
        ControlsGrid.Visibility = Visibility.Hidden;
        BtnNextPlayer.IsEnabled = true;
        BtnNextPlayer.Visibility = Visibility.Visible;

        SaveDock(playerArray[currentPlayerIndex]);
        NextPlayer();
        ScoreBoardRender();
    }

    private List<(string word, int score)> ValidatePlay() {
        TileBlock[,] newPlayArray = GetNewPlayArray();

        List<(string, int)> wordsWithScore = new();
        int wordScoreMultiplier = 1;
        bool connectedToExistingWord = false;

        // Check rows
        for (int row = 0; row < 15; row++) {
            int startColumn = -1;
            int endColumn = -1;
            bool continuous = false;
            bool connected = false;
            int possibleScore = 0;

            int? predictedColumn = null;
            int predictedScore = 0;

            for (int column = 0; column < 15; column++) {
                // Predicting the word by checking already placed tiles in case the new word follows
                if (!continuous && newPlayArray[column, row] == null) {
                    if (confirmedPlayArray[column, row] != null) {
                        predictedScore += GetLetterScore(column, row, false);
                        if (predictedColumn == null)
                            predictedColumn = column;
                    }
                    // Throwing away predict - word doesnt follow
                    else {
                        predictedColumn = null;
                        predictedScore = 0;
                    }
                }

                // Found newly placed TileBlock
                if (newPlayArray[column, row] != null) {
                    if (!continuous) {
                        startColumn = column;
                        // Apply predicted values if the word follows an already placed letter
                        if (predictedColumn != null) {
                            startColumn = (int)predictedColumn;
                            predictedColumn = null;
                            possibleScore += predictedScore;
                            predictedScore = 0;
                            connectedToExistingWord = true;
                            connected = true;
                        }
                        continuous = true;
                    }

                    // Checks if the word is placed in the middle of the board
                    if (column == 7 && row == 7) {
                        connectedToExistingWord = true;
                        connected = true;
                    }

                    // Checks if the word is connected to another word on top or bottom
                    if (confirmedPlayArray[column, row - 1] != null || confirmedPlayArray[column, row + 1] != null)
                        connected = true;

                    endColumn = column;
                    possibleScore += GetLetterScore(column, row, true);
                } else if (continuous && playArray[column, row] != null) {
                    endColumn = column;
                    possibleScore += GetLetterScore(column, row, false);
                    connectedToExistingWord = true;
                    connected = true;
                } else if (continuous) {
                    // Checking a single letter word, only here, checking all borders
                    bool single = false;
                    if (startColumn == endColumn) {
                        if (playArray[column - 1, row - 1] == null && playArray[column - 1, row + 1] == null &&
                            playArray[column - 2, row] == null && playArray[column, row] == null)
                            single = true;
                    }

                    if (startColumn != endColumn || single) {
                        string word = GetWordFromPlayArray(startColumn, endColumn, row, true);
                        if (!string.IsNullOrEmpty(word)) {
                            // Marks a wrongly placed word
                            if (!connected) {
                                word = "_" + word;
                                possibleScore = 0;
                            }
                            // Marks a word that is not in dictionary
                            else if (!IsInDictionary(word)) {
                                word = "-" + word;
                                possibleScore = 0;
                            }
                            wordsWithScore.Add((word, possibleScore * wordScoreMultiplier));
                        }
                    }
                    continuous = false;
                    connected = false;
                    possibleScore = 0;
                }
            }

            // If the word ends at the end of the row
            if (continuous && startColumn != endColumn) {
                string word = GetWordFromPlayArray(startColumn, endColumn, row, true);
                if (!string.IsNullOrEmpty(word)) {
                    // Marks a wrongly placed word
                    if (!connected) {
                        word = "_" + word;
                        possibleScore = 0;
                    }
                    wordsWithScore.Add((word, possibleScore * wordScoreMultiplier));
                }
            }
        }

        // Check columns
        for (int column = 0; column < 15; column++) {
            int startRow = -1;
            int endRow = -1;
            bool continuous = false;
            bool connected = false;
            int possibleScore = 0;

            int? predictedRow = null;
            int predictedScore = 0;

            for (int row = 0; row < 15; row++) {
                // Predicting the word by checking already placed tiles in case the new word follows
                if (!continuous && newPlayArray[column, row] == null) {
                    if (confirmedPlayArray[column, row] != null) {
                        predictedScore += GetLetterScore(column, row, false);
                        if (predictedRow == null)
                            predictedRow = row;
                    }
                    // Throwing away predict - word doesnt follow
                    else {
                        predictedRow = null;
                        predictedScore = 0;
                    }
                }

                // Found newly placed TileBlock
                if (newPlayArray[column, row] != null) {
                    if (!continuous) {
                        startRow =  row;
                        // Apply predicted values if the word follows an already placed letter
                        if (predictedRow != null) {
                            startRow = (int)predictedRow;
                            predictedRow = null;
                            possibleScore += predictedScore;
                            predictedScore = 0;
                            connectedToExistingWord = true;
                            connected = true;
                        }
                        continuous = true;
                    }

                    // Checks if the word is placed in the middle of the board
                    if (column == 7 && row == 7) {
                        connectedToExistingWord = true;
                        connected = true;
                    }

                    // Checks if the word is connected to another word on left or right
                    if (confirmedPlayArray[column - 1, row] != null || confirmedPlayArray[column + 1, row] != null)
                        connected = true;

                    endRow = row;
                    possibleScore += GetLetterScore(column, row, true);
                } else if (continuous && playArray[column, row] != null) {
                    endRow = row;
                    possibleScore += GetLetterScore(column, row, false);
                    connectedToExistingWord = true;
                    connected = true;
                } else if (continuous) {
                    if (startRow != endRow) {
                        string word = GetWordFromPlayArray(startRow, endRow, column, false);
                        if (!string.IsNullOrEmpty(word)) {
                            // Marks a wrongly placed word
                            if (!connected) {
                                word = "_" + word;
                                possibleScore = 0;
                            }
                            // Marks a word that is not in dictionary
                            else if (!IsInDictionary(word)) {
                                word = "-" + word;
                                possibleScore = 0;
                            }
                            wordsWithScore.Add((word, possibleScore * wordScoreMultiplier));
                        }
                    }
                    continuous = false;
                    connected = false;
                    possibleScore = 0;
                }
            }

            // If the word ends at the end of the column
            if (continuous && startRow != endRow) {
                string word = GetWordFromPlayArray(startRow, endRow, column, false);
                if (!string.IsNullOrEmpty(word)) {
                    // Marks a wrongly placed word
                    if (!connected) {
                        word = "_" + word;
                        possibleScore = 0;
                    }
                    wordsWithScore.Add((word, possibleScore * wordScoreMultiplier));
                }
            }
        }

        //if (!connectedToExistingWord) {
        //    for (int i = 0; i < wordsWithScore.Count; i++) {
        //        wordsWithScore[i] = "" + wordsWithScore[i];
        //    }
        //}

        return wordsWithScore;
    }

    private bool IsInDictionary(string word) {
        return true;
    }

    private int GetLetterScore(int column, int row, bool withBonus) {
        int scoreToAdd = playArray[column, row].Tile.Points;

        if (withBonus) {
            switch (bonusArray[column, row]) {
                case BonusType.DoubleLetter:
                    scoreToAdd *= 2;
                    break;
                case BonusType.TripleLetter:
                    scoreToAdd *= 3;
                    break;
            }
        }

        return scoreToAdd;
    }

    private int GetWordBonus(int column, int row) {
        switch (bonusArray[column, row]) {
            case BonusType.DoubleWord: return 2;
            case BonusType.TripleWord: return 3;
            default: return 1;
        }
    }

    private TileBlock[,] GetNewPlayArray() {
        TileBlock[,] newPlayArray = new TileBlock[15, 15];

        // Gets an array of newly placed TileBlocks by subtracting confirmedPlayArray from playArray
        for (int column = 0; column < 15; column++) {
            for (int row = 0; row < 15; row++) {
                TileBlock tile = playArray[column, row];
                if (tile != null && confirmedPlayArray[column, row] != tile) {
                    newPlayArray[column, row] = tile;
                }
            }
        }

        return newPlayArray;
    }

    private void BtnNextPlayer_Click(object sender, RoutedEventArgs e) {
        ControlsGrid.IsEnabled = true;
        ControlsGrid.Visibility = Visibility.Visible;
        BtnNextPlayer.IsEnabled = false;
        BtnNextPlayer.Visibility = Visibility.Hidden;

        LoadDock(playerArray[currentPlayerIndex]);
        DockGridFill();
        playerArray[currentPlayerIndex].SetDock(dockArray);
    }

    private void DockGridFill() {
        for (int i = 0; i < dockArray.Length; i++) {
            if (dockArray[i] != null)
                continue;

            Tile tile = GetRandomAvailableTile();
            TileBlock tileBlock = new TileBlock(tile, $"tile{tile.Character}.png");
            AddTileToDockGrid(tileBlock, i);
        }
    }

    private Tile GetRandomAvailableTile() {
        List<int> availableIndexes = new();

        for (int i = 0; i < sets[currentSetIndex].UsedArray.Length; i++) {
            if (!sets[currentSetIndex].UsedArray[i]) {
                availableIndexes.Add(i);
            }
        }
        if (availableIndexes.Count > 0) {
            int randomIndex = availableIndexes[Random.Shared.Next(0, availableIndexes.Count)];
            sets[currentSetIndex].UsedArray[randomIndex] = true;
            return sets[currentSetIndex].TileArray[randomIndex];
        }

        return null;
    }

    private string GetWordFromPlayArray(int start, int end, int fixedIndex, bool isRow) {
        string word = "";

        if (isRow) {
            for (int column = start; column <= end; column++) {
                TileBlock tile = playArray[column, fixedIndex];
                if (tile != null) {
                    word += tile.Tile.Character;
                } else {
                    return "";
                }
            }
        } else {
            for (int row = start; row <= end; row++) {
                TileBlock tile = playArray[fixedIndex, row];
                if (tile != null) {
                    word += tile.Tile.Character;
                } else {
                    return "";
                }
            }
        }

        return word;
    }

    private void ScoreBoardRender() {
        ScoreBoard.Children.Clear();

        TextBlock title = new() {
            Text = "Tabulka hráčů",
            FontSize = 68,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 20)
        };
        ScoreBoard.Children.Add(title);

        for (int i = 0; i < playerArray.Length; i++) {
            TextBlock player = new() {
                Text = $"{i}) {playerArray[i].Name}: {playerArray[i].Score}",
                FontSize = 42,
                FontWeight = FontWeights.Normal,
                Margin = new Thickness(0, 0, 0, 3)
            };

            if (i == currentPlayerIndex) {
                player.FontSize = 48;
                player.FontWeight = FontWeights.SemiBold;
            }

            ScoreBoard.Children.Add(player);
        }
    }

    #region Tile manipulation

    private void AddTileToPlayGrid(TileBlock tileBlock, int column, int row) {
        if (playArray[column, row] != null)
            return;

        playArray[column, row] = tileBlock;
        PlayGrid.Children.Add(tileBlock);
        Panel.SetZIndex(tileBlock, 5);
        Grid.SetColumn(tileBlock, column);
        Grid.SetRow(tileBlock, row);
    }

    private void MoveTileInPlayGrid(TileBlock tileBlock, int column, int row) {
        if (playArray[column, row] != null)
            return;

        int oldColumn = Grid.GetColumn(tileBlock);
        int oldRow = Grid.GetRow(tileBlock);
        playArray[oldColumn, oldRow] = null;

        Grid.SetColumn(tileBlock, column);
        Grid.SetRow(tileBlock, row);
        playArray[column, row] = tileBlock;
    }

    private void RemoveTileFromPlayGrid(TileBlock tileBlock) {
        int column = Grid.GetColumn(tileBlock);
        int row = Grid.GetRow(tileBlock);

        playArray[column, row] = null;
        PlayGrid.Children.Remove(tileBlock);
    }

    private void AddTileToDockGrid(TileBlock tileBlock, int column)
    {
        if (dockArray[column] != null)
            return;

        dockArray[column] = tileBlock;
        DockGrid.Children.Add(tileBlock);
        Panel.SetZIndex(tileBlock, 5);
        Grid.SetColumn(tileBlock, column);
    }

    private void MoveTileInDockGrid(TileBlock tileBlock, int column) {
        if (dockArray[column] != null)
            return;

        int oldColumn = Grid.GetColumn(tileBlock);
        dockArray[oldColumn] = null;

        Grid.SetColumn(tileBlock, column);
        Grid.SetRow(tileBlock, 0);
        dockArray[column] = tileBlock;
    }

    private void SwapTilesInDockGrid(TileBlock tileBlock1, TileBlock tileBlock2) {
        int column1 = Grid.GetColumn(tileBlock1);
        int column2 = Grid.GetColumn(tileBlock2);

        dockArray[column1] = tileBlock2;
        dockArray[column2] = tileBlock1;

        Grid.SetColumn(tileBlock1, column2);
        Grid.SetColumn(tileBlock2, column1);
    }

    private void RemoveTileFromDockGrid(TileBlock tileBlock) {
        int column = Grid.GetColumn(tileBlock);
        dockArray[column] = null;
        DockGrid.Children.Remove(tileBlock);
    }

    #endregion Tile manipulation

    private double GetActualHeightMultiplier() {
        return 2160 / GameViewBox.ActualHeight;
    }

    private double GetActualWidthMultiplier() {
        return 2880 / GameViewBox.ActualWidth;
    }

    private void GameView_OnLoaded(object sender, RoutedEventArgs e) {
        MainWindow.Instance.OnAnyKeyDown += OnAnyKeyDown;
    }

    private void GameView_OnUnloaded(object sender, RoutedEventArgs e) {
        MainWindow.Instance.OnAnyKeyDown -= OnAnyKeyDown;
    }

    private void OnAnyKeyDown(Key key) {
        switch (key) {
            case Key.Escape:
                ToggleEscapeMenu();
                break;
        }
    }

    private void ToggleEscapeMenuButton(object sender, RoutedEventArgs e) {
        ToggleEscapeMenu();
    }

    private void ToggleEscapeMenu() {
        if (!escapeMenu) {
            EscapeMenu.IsEnabled = true;
            EscapeMenu.Visibility = Visibility.Visible;
        } else {
            EscapeMenu.IsEnabled = false;
            EscapeMenu.Visibility = Visibility.Hidden;
        }

        escapeMenu = !escapeMenu;
    }

    private void GameView_OnMouseMove(object sender, MouseEventArgs e) {
        // Gets cell by dividing pixels by PlayGrid column/row size
        hoverPlayCellColumn = (int)Math.Floor(e.GetPosition(PlayGrid).X / 96);
        hoverPlayCellRow = (int)Math.Floor(e.GetPosition(PlayGrid).Y / 96);

        // Sets to null if out of bounds
        int lastPlayColumn = PlayGrid.ColumnDefinitions.Count - 1;
        int lastPlayRow = PlayGrid.RowDefinitions.Count - 1;
        hoverPlayCellColumn = hoverPlayCellColumn >= 0 && hoverPlayCellColumn <= lastPlayColumn ? hoverPlayCellColumn : null;
        hoverPlayCellRow = hoverPlayCellRow >= 0 && hoverPlayCellRow <= lastPlayRow ? hoverPlayCellRow : null;

        // Gets cell by dividing pixels by DockGrid column/row size
        hoverDockCellColumn = (int)Math.Floor(e.GetPosition(DockGrid).X / 120);
        hoverDockCellRow = (int)Math.Floor(e.GetPosition(DockGrid).Y / 140);

        // Sets to null if out of bounds
        int lastDockColumn = DockGrid.ColumnDefinitions.Count - 1;
        int lastDockRow = DockGrid.RowDefinitions.Count;
        hoverDockCellColumn = hoverDockCellColumn >= 0 && hoverDockCellColumn <= lastDockColumn ? hoverDockCellColumn : null;
        hoverDockCellRow = hoverDockCellRow >= 0 && hoverDockCellRow <= lastDockRow ? hoverDockCellRow : null;
    }
}