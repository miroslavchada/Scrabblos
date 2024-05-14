using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Scrabblos.MVVM.View;

/// <summary>
/// Interakční logika pro GameView.xaml
/// </summary>
public partial class GameView : UserControl {
    public GameView() {
        InitializeComponent();

        Instance = this;

        TileBlock test = new TileBlock(new Tile(' ', 0), "mirek.ico");
        AddTileToDockGrid(test, 2);
        DockGridFill();
    }

    public static GameView Instance { get; private set; }

    private bool escapeMenu;
    private int currentSetIndex = 0;

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
            { new Tile('_', 0), 2 }
        }, "Čeština oficiální")
    };

    private TileBlock?[,] playArray = new TileBlock?[15, 15];
    private TileBlock?[] dockArray = new TileBlock?[7];

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

        // From which grid the tile is dragged is in the foreground
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

            // If there is already a tile in the cell - do nothing
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

            // If there is already a tile in the cell
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

    Image preview = new()
    {
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

    private void DockGridFill() {

        for (int i = 0; i < dockArray.Length; i++) {
            if (dockArray[i] != null)
                continue;

            Tile tile = GetRandomAvailableTile();
            //TileBlock tileBlock = new TileBlock(tile, $"tile{tile.character}.jpg");
            TileBlock tileBlock = new TileBlock(tile, $"tileS.jpg");
            AddTileToDockGrid(tileBlock, i);
        }
    }

    private Tile GetRandomAvailableTile() {
        List<int> availableIndexes = new();

        for (int i = 0; i < sets[currentSetIndex].usedArray.Length; i++) {
            if (!sets[currentSetIndex].usedArray[i]) {
                availableIndexes.Add(i);
            }
        }
        if (availableIndexes.Count > 0) {
            int randomIndex = availableIndexes[Random.Shared.Next(0, availableIndexes.Count)];
            sets[currentSetIndex].usedArray[randomIndex] = true;
            return sets[currentSetIndex].tileArray[randomIndex];
        }

        return null;
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

    private void AddTileToDockGrid(TileBlock tileBlock, int column) {
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

        // If in bounds of PlayGrid
        if (hoverPlayCellColumn != null && hoverPlayCellRow != null) {
            TBlockInfo.Text = $"Buňka pro položení: {CoordsToCell((int)hoverPlayCellColumn, (int)hoverPlayCellRow)}";
        } // If in bounds of DockGrid
        else if (hoverDockCellColumn != null && hoverDockCellRow != null) {
            TBlockInfo.Text = $"Buňka pro položení: DOCK{(int)hoverDockCellColumn}";
        } else {
            TBlockInfo.Text = "Informace jak pán";
        }
    }

    private string CoordsToCell(int columnIndex, int rowIndex)
    {
        char[] letters = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O' };
        
        return $"{columnIndex + 1}{letters[rowIndex]}";
    }
}