using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Application = System.Windows.Application;
using Image = System.Windows.Controls.Image;

namespace Scrabblos.MVVM.View;

/// <summary>
/// Interakční logika pro GameView.xaml
/// </summary>
public partial class GameView {
    public GameView() {
        InitializeComponent();
        Instance = this;

        // Set propper z-indexes
        Panel.SetZIndex(BtnNextPlayer, 2);

        // Reads the dictionary from a file in resources
        SetDictionary(Path.Combine(Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).Parent.Parent.FullName, "Resources") + "\\slovnik_utf8.tsv");

        SetPlayers(Application.Current.Properties["Players"] as string[]);
        ScoreBoardRender();
    }

    public static GameView? Instance { get; private set; }

    private bool _escapeMenu;
    private readonly int _currentSetIndex = 0;

    private Player[]? _playerArray;
    private int _currentPlayerIndex;

    private readonly List<TileSet> _sets = new() {
        new TileSet(new List<(Tile, int)> {
            ( new Tile('A', 1), 5 ),
            ( new Tile('Á', 2), 2 ),
            ( new Tile('B', 3), 2 ),
            ( new Tile('C', 2), 3 ),
            ( new Tile('Č', 4), 1 ),
            ( new Tile('D', 1), 3 ),
            ( new Tile('Ď', 8), 1 ),
            ( new Tile('E', 1), 5 ),
            ( new Tile('É', 3), 2 ),
            ( new Tile('Ě', 3), 2 ),
            ( new Tile('F', 5), 1 ),
            ( new Tile('G', 5), 1 ),
            ( new Tile('H', 2), 3 ),
            ( new Tile('I', 1), 4 ),
            ( new Tile('Í', 2), 3 ),
            ( new Tile('J', 2), 2 ),
            ( new Tile('K', 1), 3 ),
            ( new Tile('L', 1), 3 ),
            ( new Tile('M', 2), 3 ),
            ( new Tile('N', 1), 5 ),
            ( new Tile('Ň', 6), 1 ),
            ( new Tile('O', 1), 6 ),
            ( new Tile('Ó', 7), 1 ),
            ( new Tile('P', 1), 3 ),
            ( new Tile('R', 1), 3 ),
            ( new Tile('Ř', 4), 2 ),
            ( new Tile('S', 1), 4 ),
            ( new Tile('Š', 4), 2 ),
            ( new Tile('T', 1), 4 ),
            ( new Tile('Ť', 7), 1 ),
            ( new Tile('U', 2), 3 ),
            ( new Tile('Ú', 5), 1 ),
            ( new Tile('Ů', 4), 1 ),
            ( new Tile('V', 1), 4 ),
            ( new Tile('X', 10), 1 ),
            ( new Tile('Y', 2), 2 ),
            ( new Tile('Ý', 4), 2 ),
            ( new Tile('Z', 2), 2 ),
            ( new Tile('Ž', 4), 1 )
        }, "Čeština oficiální"),

        new TileSet(new List<(Tile, int)> {
            ( new Tile('A', 1), 1 ),
            ( new Tile('Á', 2), 1 ),
            ( new Tile('B', 3), 1 ),
            ( new Tile('C', 2), 1 ),
            ( new Tile('Č', 4), 1 ),
            ( new Tile('D', 1), 1 ),
            ( new Tile('Ď', 8), 0 ),
            ( new Tile('E', 1), 1 ),
            ( new Tile('É', 3), 0 ),
            ( new Tile('Ě', 3), 0 ),
            ( new Tile('F', 5), 0 ),
            ( new Tile('G', 5), 0 ),
            ( new Tile('H', 2), 1 ),
            ( new Tile('I', 1), 1 ),
            ( new Tile('Í', 2), 1 ),
            ( new Tile('J', 2), 1 ),
            ( new Tile('K', 1), 1 ),
            ( new Tile('L', 1), 1 ),
            ( new Tile('M', 2), 1 ),
            ( new Tile('N', 1), 1 ),
            ( new Tile('Ň', 6), 0 ),
            ( new Tile('O', 1), 1 ),
            ( new Tile('Ó', 7), 0 ),
            ( new Tile('P', 1), 1 ),
            ( new Tile('R', 1), 1 ),
            ( new Tile('Ř', 4), 0 ),
            ( new Tile('S', 1), 1 ),
            ( new Tile('Š', 4), 0 ),
            ( new Tile('T', 1), 1 ),
            ( new Tile('Ť', 7), 0 ),
            ( new Tile('U', 2), 1 ),
            ( new Tile('Ú', 5), 0 ),
            ( new Tile('Ů', 4), 0 ),
            ( new Tile('V', 1), 1 ),
            ( new Tile('X', 10), 0 ),
            ( new Tile('Y', 2), 1 ),
            ( new Tile('Ý', 4), 0 ),
            ( new Tile('Z', 2), 0 ),
            ( new Tile('Ž', 4), 0 )
        }, "Zkrácený testovací")
    };

    private bool _allTilesUsed;
    private bool _gameEnded;

    private readonly HashSet<(string, int)> _dictionary = new();
    private readonly HashSet<string> _dictionaryJustWords = new();
    private readonly char[] _allowedChars = {
        'A', 'Á', 'B', 'C', 'Č', 'D', 'Ď', 'E', 'É', 'Ě', 'F', 'G', 'H', 'I', 'Í', 'J', 'K', 'L', 'M', 'N', 'Ň', 'O', 'Ó', 'P', 'R', 'Ř', 'S', 'Š', 'T', 'Ť', 'U', 'Ú', 'Ů', 'V', 'X', 'Y', 'Ý', 'Z', 'Ž'
    };
    private readonly bool _noDictionary = false;

    private readonly TileBlock?[,] _playArray = new TileBlock[15, 15];
    private readonly TileBlock?[,] _confirmedPlayArray = new TileBlock[15, 15];
    private readonly TileBlock?[] _dockArray = new TileBlock[7];

    private int _roundSkipStreak;
    private readonly int _roundSkipStreakMax = 4;

    private enum BonusType {
        None,
        DoubleLetter,
        TripleLetter,
        DoubleWord,
        TripleWord
    }

    private readonly BonusType[,] _bonusArray = new BonusType[15, 15] {
        { BonusType.TripleWord, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.TripleWord },
        { BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None },
        { BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None },
        { BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.DoubleLetter},
        { BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.None },
        { BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None },
        { BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None },
        { BonusType.TripleWord, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.TripleWord },
        { BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None },
        { BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None },
        { BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.None },
        { BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.DoubleLetter},
        { BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None },
        { BonusType.None, BonusType.DoubleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleWord, BonusType.None },
        { BonusType.TripleWord, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.None, BonusType.TripleWord, BonusType.None, BonusType.None, BonusType.None, BonusType.DoubleLetter, BonusType.None, BonusType.None, BonusType.TripleWord }
        };

    private void SetPlayers(string[] players) {
        _playerArray = new Player[players.Length];

        for (int i = 0; i < players.Length; i++) {
            // New player from Name string
            Player player = new Player(players[i]);
            SaveDock(player);
            _playerArray[i] = player;
        }

        LoadDock(_playerArray[_currentPlayerIndex]);
    }

    private void SetDictionary(string filePath) {
        using StreamReader sr = new(filePath);
        string? line;
        while ((line = sr.ReadLine()) != null) {
            string[] parts = line.Split('\t');
            bool isValid = false;
            foreach (char allowedChar in _allowedChars) {
                if (parts[0].Contains(allowedChar)) {
                    isValid = true;
                    break;
                }
            }

            if (isValid) {
                _ = _dictionary.Add((parts[0], Convert.ToInt32(parts[1])));
                _ = _dictionaryJustWords.Add(parts[0]);
            }
        }
    }

    #region Tile dragging

    protected bool IsDragging;
    private Point _clickPosition;
    private TranslateTransform? _originTt;

    private bool _tileMoved;

    private bool _previewActive;

    private int? _hoverPlayCellColumn, _hoverDockCellColumn;
    private int? _hoverPlayCellRow, _hoverDockCellRow;

    public void TileBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
        _tileMoved = false;
        var draggableControl = sender as TileBlock;
        _originTt = draggableControl!.RenderTransform as TranslateTransform ?? new TranslateTransform();
        IsDragging = true;
        _clickPosition = e.GetPosition(this);
        _ = draggableControl.CaptureMouse();

        // From which grid the Tile is dragged is in the foreground
        Grid? draggableParent = draggableControl.Parent as Grid;
        switch (draggableParent!.Name) {
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
        IsDragging = false;
        var draggable = sender as TileBlock;
        Grid? draggableParent = draggable!.Parent as Grid;

        var transform = draggable.RenderTransform as TranslateTransform ?? new TranslateTransform();

        // Snap to grid if placed correctly
        // PlayGrid
        if (_hoverPlayCellColumn != null && _hoverPlayCellRow != null) {
            // int? to int (can't be null anymore)
            int gameCellColumn = (int)_hoverPlayCellColumn;
            int gameCellRow = (int)_hoverPlayCellRow;

            // If there is already a Tile in the cell - do nothing
            if (_playArray[gameCellColumn, gameCellRow] != null) {
                transform.X = 0;
                transform.Y = 0;
                draggable.ReleaseMouseCapture();
                Panel.SetZIndex(draggable, 5);
                return;
            }

            // Transition from DockGrid to PlayGrid
            if (draggableParent != PlayGrid) {
                RemoveTileFromDockGrid(draggable, false);
                AddTileToPlayGrid(draggable, gameCellColumn, gameCellRow);
            }
            // Move in PlayGrid if already in PlayGrid
            else {
                MoveTileInPlayGrid(draggable, gameCellColumn, gameCellRow);
            }
        } // DockGrid
        else if (_hoverDockCellColumn != null && _hoverDockCellRow != null) {
            // If mouse did not move, toggle the TileBlocks MarkedForExchange status
            if (!_tileMoved) {
                draggable.ToggleExchangeMark();
                draggable.ReleaseMouseCapture();
                _tileMoved = true;
                return;
            }

            // int? to int (can't be null anymore)
            int dockCellColumn = (int)_hoverDockCellColumn;

            // If there is already a Tile in the cell
            if (_dockArray[dockCellColumn] != null) {
                // Swap tiles if dragged from DockGrid
                if (draggableParent == DockGrid) {
                    SwapTilesInDockGrid(draggable, _dockArray[dockCellColumn]!);
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
                RemoveTileFromPlayGrid(draggable);
                AddTileToDockGrid(draggable, dockCellColumn);
            }
            // Move in DockGrid if already in DockGrid
            else {
                MoveTileInDockGrid(draggable, dockCellColumn);
            }
        }

        DisablePreview();
        draggable.CancelExchangeMark();

        // Reset position in cell
        transform.X = 0;
        transform.Y = 0;
        draggable.ReleaseMouseCapture();
        Panel.SetZIndex(draggable, 5);
    }


    private readonly Image _preview = new() {
        Opacity = 0.2,

        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center
    };

    private void DisablePreview() {
        if (_previewActive) {
            PlayGrid.Children.Remove(_preview);
            DockGrid.Children.Remove(_preview);
            _previewActive = false;
        }
    }

    public void TileBlock_MouseMove(object sender, MouseEventArgs e) {
        _tileMoved = true;
        if (IsDragging && sender is Image draggableControl) {
            Point currentPosition = e.GetPosition(this);
            var transform = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
            transform.X = _originTt!.X + ((currentPosition.X - _clickPosition.X) * GetActualWidthMultiplier());
            transform.Y = _originTt!.Y + ((currentPosition.Y - _clickPosition.Y) * GetActualHeightMultiplier());
            draggableControl.RenderTransform = new TranslateTransform(transform.X, transform.Y);

            _preview.Source = draggableControl.Source;
            _preview.Height = draggableControl.Height;
            _preview.Width = draggableControl.Width;
            Margin = draggableControl.Margin;

            // Preview projection
            Grid? grid = null;
            int previewCellColumn = 0;
            int previewCellRow = 0;

            // Unify cell position
            if (_hoverPlayCellColumn != null && _hoverPlayCellRow != null) {
                grid = PlayGrid;
                previewCellColumn = (int)_hoverPlayCellColumn;
                previewCellRow = (int)_hoverPlayCellRow;
            }
            else if (_hoverDockCellColumn != null && _hoverDockCellRow != null) {
                grid = DockGrid;
                previewCellColumn = (int)_hoverDockCellColumn;
                previewCellRow = (int)_hoverDockCellRow;
            }

            if (grid != null) {
                if (!_previewActive) {
                    _ = grid.Children.Add(_preview);
                    Panel.SetZIndex(_preview, 4);
                }

                Grid.SetColumn(_preview, previewCellColumn);
                Grid.SetRow(_preview, previewCellRow);

                _previewActive = true;
            }
            else {
                DisablePreview();
            }
        }
    }

    private void GameView_OnMouseMove(object sender, MouseEventArgs e) {
        // Gets cell by dividing pixels by PlayGrid column/row size
        _hoverPlayCellColumn = (int)Math.Floor(e.GetPosition(PlayGrid).X / 96);
        _hoverPlayCellRow = (int)Math.Floor(e.GetPosition(PlayGrid).Y / 96);

        // Sets to null if out of bounds
        int lastPlayColumn = PlayGrid.ColumnDefinitions.Count - 1;
        int lastPlayRow = PlayGrid.RowDefinitions.Count - 1;
        _hoverPlayCellColumn = _hoverPlayCellColumn >= 0 && _hoverPlayCellColumn <= lastPlayColumn ? _hoverPlayCellColumn : null;
        _hoverPlayCellRow = _hoverPlayCellRow >= 0 && _hoverPlayCellRow <= lastPlayRow ? _hoverPlayCellRow : null;

        // Gets cell by dividing pixels by DockGrid column/row size
        _hoverDockCellColumn = (int)Math.Floor(e.GetPosition(DockGrid).X / 120);
        _hoverDockCellRow = (int)Math.Floor(e.GetPosition(DockGrid).Y / 140);

        // Sets to null if out of bounds
        int lastDockColumn = DockGrid.ColumnDefinitions.Count - 1;
        int lastDockRow = DockGrid.RowDefinitions.Count;
        _hoverDockCellColumn = _hoverDockCellColumn >= 0 && _hoverDockCellColumn <= lastDockColumn ? _hoverDockCellColumn : null;
        _hoverDockCellRow = _hoverDockCellRow >= 0 && _hoverDockCellRow <= lastDockRow ? _hoverDockCellRow : null;
    }

    #endregion Tile dragging

    private void NextPlayer() {
        _currentPlayerIndex = _currentPlayerIndex >= _playerArray!.Length - 1 ? 0 : _currentPlayerIndex + 1;
    }

    private void LoadDock(Player player) {
        // Clear grid
        foreach (TileBlock? tileBlock in _dockArray) {
            if (tileBlock == null)
                continue;
            RemoveTileFromDockGrid(tileBlock, false);
        }

        // Restore players dock
        TileBlock?[] dock = player.GetDock();
        for (int i = 0; i < _dockArray.Length; i++) {
            if (dock[i] != null)
                AddTileToDockGrid(dock[i]!, i);
        }
    }

    private void SaveDock(Player player) {
        DockGridFill();

        foreach (TileBlock? tileBlock in _dockArray) {
            if (tileBlock != null)
                tileBlock.CancelExchangeMark();
        }

        // Rewrite players dock
        player.SetDock(_dockArray!);

        // Clear grid
        foreach (TileBlock? tileBlock in _dockArray) {
            if (tileBlock != null)
                RemoveTileFromDockGrid(tileBlock, false);
        }
    }

    private void BtnRestoreDock_Click(object sender, RoutedEventArgs e) {
        ReturnTilesToDock();
    }

    private void ReturnTilesToDock() {
        foreach (TileBlock? tileBlock in GetNewPlayArray()) {
            if (tileBlock == null)
                continue;

            RemoveTileFromPlayGrid(tileBlock);
        }
        LoadDock(_playerArray![_currentPlayerIndex]);
    }

    private void RoundSkipped() {
        _roundSkipStreak++;

        if (_roundSkipStreak >= 2) {

            // Inflecting the word "remains"
            string remainsStringInflected;
            switch (_roundSkipStreakMax - _roundSkipStreak) {
                case 1:
                    remainsStringInflected = "Zbývá";
                    break;

                case 2 or 3 or 4:
                    remainsStringInflected = "Zbývají";
                    break;

                default:
                    remainsStringInflected = "Zbývá";
                    break;
            }
            TbDockInfo.Text = $"{remainsStringInflected} {_roundSkipStreakMax - _roundSkipStreak} přeskočení konce hry.";
        }
    }

    #region Word submitting and validation 

    private void BtnRoundApprove_Click(object sender, RoutedEventArgs e) {
        List<(string, int)> wordsWithScore = ValidatePlay();

        List<string> invalidWords = new();
        List<string> wrongPlacedWords = new();

        string info = "";
        TbDockInfo.Text = "";

        TileBlock?[,] newPlayArray = GetNewPlayArray();

        // Exchanging TileBlocks or passing round
        if (wordsWithScore.Count <= 0) {
            List<char> exchangedTileBlocks = new();
            for (int i = 0; i < _dockArray.Length; i++) {
                if (_dockArray[i] != null){
                    if (_dockArray[i]!.MarkedForExchange) {
                        exchangedTileBlocks.Add(_dockArray[i]!.Tile.Character);
                        RemoveTileFromDockGrid(_dockArray[i]!, true);
                        DockGridFill();
                    }
                }
            }

            RoundSkipped();
            
            if (_roundSkipStreak >= _roundSkipStreakMax) {
                EndGame();
                return;
            }

            info = exchangedTileBlocks.Count > 0 ? $"Vyměněná písmena: {string.Join(", ", exchangedTileBlocks)}" : "Tah přeskočen";
        }
        else {
            // Check for invalid and wrongly placed words
            foreach (var (word, _) in wordsWithScore) {
                if (word[0] == '-')
                    invalidWords.Add(word[1..]);

                if (word[0] == '_') {
                    wrongPlacedWords.Add(word[1..]);
                }
            }

            // If there are any invalid or wrongly placed words
            if (invalidWords.Count > 0 || wrongPlacedWords.Count > 0) {
                string infoInvalid = invalidWords.Count > 0 ? $"Neplatná slova: {string.Join(", ", invalidWords)}" : "";
                string infoWrongPlaced = wrongPlacedWords.Count > 0 ? $"Špatně položená slova: {string.Join(", ", wrongPlacedWords)}" : "";

                // Report to info bar
                if (invalidWords.Count > 0 && wrongPlacedWords.Count > 0)
                    info = $"{infoInvalid}\r\n{infoWrongPlaced}";
                else if (invalidWords.Count > 0)
                    info = infoInvalid;
                else if (wrongPlacedWords.Count > 0)
                    info = infoWrongPlaced;

                TbInfo.Text = info;
                return;
            }


            // Resetting round skip streak
            _roundSkipStreak = 0;

            // Add all gained score and add to player
            foreach (var (_, wordScore) in wordsWithScore) {
                _playerArray![_currentPlayerIndex].Score += wordScore;
            }

            info += $"Slova: {string.Join(", ", wordsWithScore)}";
        }
        TbInfo.Text = info;

        // Disables any mouse interaction with the tiles
        foreach (TileBlock? tileBlock in newPlayArray) {
            if (tileBlock == null)
                continue;

            tileBlock.UnsubscribeInteraction();
        }

        Array.Copy(_playArray, _confirmedPlayArray, _playArray.Length);

        ControlsGrid.IsEnabled = false;
        ControlsGrid.Visibility = Visibility.Hidden;
        BtnNextPlayer.IsEnabled = true;
        BtnNextPlayer.Visibility = Visibility.Visible;

        // Save dock to player's dock
        SaveDock(_playerArray![_currentPlayerIndex]);

        // End game if all tiles are used
        if (_allTilesUsed && _playerArray![_currentPlayerIndex].GetDockCount() == 0) {
            EndGame();
            return;
        }

        // Transition to next player
        NextPlayer();
        ScoreBoardRender();

        // If single player, auto next round
        if (_playerArray.Length == 1) {
            BtnNextPlayer.RaiseEvent(e);
        }
    }

    private List<(string word, int score)> ValidatePlay() {
        TileBlock?[,] newPlayArray = GetNewPlayArray();

        List<(string, int)> wordsWithScore = new();

        // Check rows
        for (int row = 0; row < 15; row++) {
            int startColumn = -1;
            int endColumn = -1;
            bool continuous = false;
            bool connected = false;
            int wordScoreMultiplier = 1;
            int possibleScore = 0;

            int? predictedColumn = null;
            int predictedScore = 0;

            for (int column = 0; column < 15; column++) {
                // Predicting the word by checking already placed tiles in case the new word follows
                if (!continuous && newPlayArray[column, row] == null) {
                    if (_confirmedPlayArray[column, row] != null) {
                        predictedScore += GetLetterScore(column, row, false);
                        predictedColumn ??= column;
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
                            connected = true;
                        }
                        continuous = true;
                    }

                    // Checks if the word is placed in the middle of the board
                    if (column == 7 && row == 7)
                        connected = true;

                    // Checks if the word is connected to another word on top or bottom
                    if (row == 0) {
                        if (_confirmedPlayArray[column, row + 1] != null)
                            connected = true;
                    }
                    else if (row == 14) {
                        if (_confirmedPlayArray[column, row - 1] != null)
                            connected = true;
                    }
                    else {
                        if (_confirmedPlayArray[column, row - 1] != null || _confirmedPlayArray[column, row + 1] != null)
                            connected = true;
                    }

                    endColumn = column;
                    possibleScore += GetLetterScore(column, row, true);
                    wordScoreMultiplier *= GetWordBonus(column, row);
                }
                else if (continuous && _playArray[column, row] != null) {
                    endColumn = column;
                    possibleScore += GetLetterScore(column, row, false);
                    connected = true;
                }
                else if (continuous) {
                    // Checking a single letter word, only here, checking all borders
                    bool single = false;
                    if (startColumn == endColumn)
                        single = IsSingle(startColumn, row);

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
            int wordScoreMultiplier = 1;
            int possibleScore = 0;

            int? predictedRow = null;
            int predictedScore = 0;

            for (int row = 0; row < 15; row++) {
                // Predicting the word by checking already placed tiles in case the new word follows
                if (!continuous && newPlayArray[column, row] == null) {
                    if (_confirmedPlayArray[column, row] != null) {
                        predictedScore += GetLetterScore(column, row, false);
                        predictedRow ??= row;
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
                        startRow = row;
                        // Apply predicted values if the word follows an already placed letter
                        if (predictedRow != null) {
                            startRow = (int)predictedRow;
                            predictedRow = null;
                            possibleScore += predictedScore;
                            predictedScore = 0;
                            connected = true;
                        }
                        continuous = true;
                    }

                    // Checks if the word is placed in the middle of the board
                    if (column == 7 && row == 7)
                        connected = true;

                    // Checks if the word is connected to another word on left or right
                    if (column == 0) {
                        if (_confirmedPlayArray[column + 1, row] != null)
                            connected = true;
                    }
                    else if (column == 14) {
                        if (_confirmedPlayArray[column - 1, row] != null)
                            connected = true;
                    }
                    else {
                        if (_confirmedPlayArray[column - 1, row] != null || _confirmedPlayArray[column + 1, row] != null)
                            connected = true;
                    }

                    endRow = row;
                    possibleScore += GetLetterScore(column, row, true);
                    wordScoreMultiplier *= GetWordBonus(column, row);
                }
                else if (continuous && _playArray[column, row] != null) {
                    endRow = row;
                    possibleScore += GetLetterScore(column, row, false);
                    connected = true;
                }
                else if (continuous) {
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

        return wordsWithScore;
    }

    private bool IsSingle(int column, int row) {
        bool top = row == 0 || _playArray[column, row - 1] == null;
        bool bottom = row == 14 || _playArray[column, row + 1] == null;
        bool left = column == 0 || _playArray[column - 1, row] == null;
        bool right = column == 14 || _playArray[column + 1, row] == null;

        return top && bottom && left && right;
    }

    private bool IsInDictionary(string wordToFind) {
        return _noDictionary || _dictionaryJustWords.Contains(wordToFind);
    }

    private int GetLetterScore(int column, int row, bool withBonus) {
        int scoreToAdd = _playArray[column, row]!.Tile.Points;

        if (withBonus) {
            switch (_bonusArray[column, row]) {
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
        return _bonusArray[column, row] switch {
            BonusType.DoubleWord => 2,
            BonusType.TripleWord => 3,
            _ => 1,
        };
    }

    private TileBlock?[,] GetNewPlayArray() {
        TileBlock?[,] newPlayArray = new TileBlock[15, 15];

        // Gets an array of newly placed TileBlocks by subtracting _confirmedPlayArray from _playArray
        for (int column = 0; column < 15; column++) {
            for (int row = 0; row < 15; row++) {
                TileBlock? tile = _playArray[column, row];
                if (tile != null && _confirmedPlayArray[column, row] != tile) {
                    newPlayArray[column, row] = tile;
                }
            }
        }

        return newPlayArray;
    }

    #endregion Word submitting and validation 

    private void BtnNextPlayer_Click(object sender, RoutedEventArgs e) {
        ControlsGrid.IsEnabled = true;
        ControlsGrid.Visibility = Visibility.Visible;
        BtnNextPlayer.IsEnabled = false;
        BtnNextPlayer.Visibility = Visibility.Hidden;

        LoadDock(_playerArray![_currentPlayerIndex]);
        DockGridFill();
        _playerArray[_currentPlayerIndex].SetDock(_dockArray!);
    }

    private void DockGridFill() {
        for (int i = 0; i < _dockArray.Length; i++) {
            if (_dockArray[i] != null)
                continue;

            (Tile, int)? tile = GetRandomAvailableTile();
            if (tile != null) {
                TileBlock tileBlock = new TileBlock(tile.Value.Item1, tile.Value.Item2, $"tile{tile.Value.Item1.Character}.png");
                AddTileToDockGrid(tileBlock, i);
            }
        }
    }

    // Returns a random available Tile from the current set and an index of the Tile in the set
    private (Tile, int)? GetRandomAvailableTile() {
        List<int> availableIndexes = new();

        for (int i = 0; i < _sets[_currentSetIndex].TileArray.Length; i++) {
            if (!_sets[_currentSetIndex].TileArray[i].Item2) {
                availableIndexes.Add(i);
            }
        }

        TbTilesRemaining.Text = (availableIndexes.Count - 1).ToString();

        if (availableIndexes.Count > 0) {
            int randomIndex = availableIndexes[Random.Shared.Next(0, availableIndexes.Count)];
            _sets[_currentSetIndex].TileArray[randomIndex].Item2 = true;
            return (_sets[_currentSetIndex].TileArray[randomIndex].Item1, randomIndex);
        }
        
        TbTilesRemaining.Text = "0";
        _allTilesUsed = true;
        return null;
    }

    private string GetWordFromPlayArray(int start, int end, int fixedIndex, bool isRow) {
        string word = "";

        if (isRow) {
            for (int column = start; column <= end; column++) {
                TileBlock? tile = _playArray[column, fixedIndex];
                if (tile != null) {
                    word += tile.Tile.Character;
                }
                else {
                    return "";
                }
            }
        }
        else {
            for (int row = start; row <= end; row++) {
                TileBlock? tile = _playArray[fixedIndex, row];
                if (tile != null) {
                    word += tile.Tile.Character;
                }
                else {
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
        _ = ScoreBoard.Children.Add(title);

        for (int i = 0; i < _playerArray!.Length; i++) {
            TextBlock player = new() {
                Text = $"{i + 1}) {_playerArray[i].Name}: {_playerArray[i].Score}",
                FontSize = 42,
                FontWeight = FontWeights.Normal,
                Margin = new Thickness(0, 0, 0, 3)
            };

            if (i == _currentPlayerIndex) {
                player.FontSize = 48;
                player.FontWeight = FontWeights.SemiBold;
                player.Foreground = Brushes.White;
                player.Background = Brushes.RoyalBlue;
            }

            _ = ScoreBoard.Children.Add(player);
        }
    }

    #region Tile manipulation

    private void AddTileToPlayGrid(TileBlock tileBlock, int column, int row) {
        if (_playArray[column, row] != null)
            return;

        _playArray[column, row] = tileBlock;
        _ = PlayGrid.Children.Add(tileBlock);
        Panel.SetZIndex(tileBlock, 5);
        Grid.SetColumn(tileBlock, column);
        Grid.SetRow(tileBlock, row);
    }

    private void MoveTileInPlayGrid(TileBlock tileBlock, int column, int row) {
        if (_playArray[column, row] != null)
            return;

        int oldColumn = Grid.GetColumn(tileBlock);
        int oldRow = Grid.GetRow(tileBlock);
        _playArray[oldColumn, oldRow] = null;

        Grid.SetColumn(tileBlock, column);
        Grid.SetRow(tileBlock, row);
        _playArray[column, row] = tileBlock;
    }

    private void RemoveTileFromPlayGrid(TileBlock tileBlock) {
        int column = Grid.GetColumn(tileBlock);
        int row = Grid.GetRow(tileBlock);

        _playArray[column, row] = null;
        PlayGrid.Children.Remove(tileBlock);
    }

    private void AddTileToDockGrid(TileBlock tileBlock, int column) {
        if (_dockArray[column] != null)
            return;

        _dockArray[column] = tileBlock;
        _ = DockGrid.Children.Add(tileBlock);
        Panel.SetZIndex(tileBlock, 5);
        Grid.SetColumn(tileBlock, column);
    }

    private void MoveTileInDockGrid(TileBlock tileBlock, int column) {
        if (_dockArray[column] != null)
            return;

        int oldColumn = Grid.GetColumn(tileBlock);
        _dockArray[oldColumn] = null;

        Grid.SetColumn(tileBlock, column);
        Grid.SetRow(tileBlock, 0);
        _dockArray[column] = tileBlock;
    }

    private void SwapTilesInDockGrid(TileBlock tileBlock1, TileBlock tileBlock2) {
        int column1 = Grid.GetColumn(tileBlock1);
        int column2 = Grid.GetColumn(tileBlock2);

        _dockArray[column1] = tileBlock2;
        _dockArray[column2] = tileBlock1;

        Grid.SetColumn(tileBlock1, column2);
        Grid.SetColumn(tileBlock2, column1);
    }

    private void RemoveTileFromDockGrid(TileBlock tileBlock, bool toExchange) {
        int column = Grid.GetColumn(tileBlock);
        _dockArray[column] = null;
        DockGrid.Children.Remove(tileBlock);

        if (toExchange)
            _sets[_currentSetIndex].TileArray[tileBlock.TileSetIndex].Item2 = false;
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
        // After game ended
        if (_gameEnded) {
            switch (key) {
                case Key.Escape:
                case Key.Space:
                    BtnBackToMenu.Command.Execute(null);
                    break;
            }
        }
        // Normal behavior
        else {
            switch (key) {
                case Key.Escape:
                    ToggleEscapeMenu();
                    break;
            }
        }
    }

    private void ToggleEscapeMenuButton(object sender, RoutedEventArgs e) {
        ToggleEscapeMenu();
    }

    private void ToggleEscapeMenu() {
        if (!_escapeMenu) {
            EscapeMenu.IsEnabled = true;
            EscapeMenu.Visibility = Visibility.Visible;
        }
        else {
            EscapeMenu.IsEnabled = false;
            EscapeMenu.Visibility = Visibility.Hidden;
        }

        _escapeMenu = !_escapeMenu;
    }

    private void EndGame() {
        BtnRestoreDock.Visibility = Visibility.Hidden;
        BtnRoundApprove.Visibility = Visibility.Hidden;
        BtnPauseGame.Visibility = Visibility.Hidden;
        BtnNextPlayer.Visibility = Visibility.Hidden;
        TbDockInfo.Visibility = Visibility.Hidden;
        ScoreBoard.Visibility = Visibility.Hidden;
        DockGrid.Visibility = Visibility.Hidden; 
        WinnerScreen.Visibility = Visibility.Visible;
        _gameEnded = true;

        // Calculate unused score from all players
        int unusedScore = 0;

        // Index of a player that has ended the game by clearing his dock
        int? clearedPlayerIndex = null;

        for (int i = 0; i < _playerArray!.Length; i++) {
            if (_playerArray[i].GetDockCount() == 0) {
                clearedPlayerIndex = i;
            }
            else {
                unusedScore += _playerArray[i].GetDockValue();
            }

            // Subtract player's dock value from his score
            _playerArray[i].Score -= _playerArray[i].GetDockValue();
        }

        // If there is a player that has cleared his dock
        // Add unused score to his score
        if (clearedPlayerIndex != null) {
            _playerArray[clearedPlayerIndex.Value].Score += unusedScore;
            TbEndInfo.Text = $"{_playerArray[clearedPlayerIndex.Value].Name} se zbavil(a) všech svých kamenů.";
        }
        else {
            TbEndInfo.Text = $"Hra skončila po {_roundSkipStreakMax} po sobě jdoucích přeskočení.";
        }

        // Render winner screen
        int contentHeight = 0;

        // Sort players by score
        _playerArray = _playerArray.OrderByDescending(p => p.Score).ToArray();


        // Winner array with players that have same score (in case of a draw)
        Player[] winnerArray = _playerArray.Where(p => p.Score == _playerArray[0].Score).ToArray();

        // Defeated players array
        Player[] defeatedArray = _playerArray.Skip(winnerArray.Length).ToArray();

        foreach (Player winner in winnerArray) {
            TextBlock winnerName = new() {
                Text = winner.Name,
                TextAlignment = TextAlignment.Center,
                FontSize = 126,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 30, 0, -12)
            };
            // Add height and margin for each winner name to screen height
            contentHeight += 168 + 18;

            TextBlock winnerScore = new() {
                Text = $"Body: {winner.Score}",
                TextAlignment = TextAlignment.Center,
                FontSize = 64,
                FontWeight = FontWeights.SemiBold,
            };
            // Add height for each winner score to screen height
            contentHeight += 85;

            // Bigger margin on last winner
            if (winner == winnerArray[winnerArray.Length - 1]) {
                winnerScore.Margin = new Thickness(0, 0, 0, 12);
                // Add margin for last winner score to screen height
                contentHeight += 12;
            }

            _ = WinnerStackPanel.Children.Add(winnerName);
            _ = WinnerStackPanel.Children.Add(winnerScore);
        }

        foreach (Player player in defeatedArray) {
            TextBlock playerName = new() {
                Text = player.Name,
                TextAlignment = TextAlignment.Center,
                FontSize = 72,
                FontWeight = FontWeights.Normal,
                Margin = new Thickness(0, 24, 0, 0)
            };
            // Add height and margin for each player name to screen height
            contentHeight += 96 + 24;

            TextBlock playerScore = new() {
                Text = $"Body: {player.Score}",
                TextAlignment = TextAlignment.Center,
                FontSize = 36,
                FontWeight = FontWeights.SemiBold
            };
            // Add height for each player score to screen height
            contentHeight += 56;

            _ = WinnerStackPanel.Children.Add(playerName);
            _ = WinnerStackPanel.Children.Add(playerScore);
        }

        // Add height to WinnerGrid
        WinnerGrid.Height += contentHeight;
    }
}
