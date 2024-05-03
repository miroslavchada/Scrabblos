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

        DockGrid.Children.Add(new TileBlock(new Tile('S', 1), "tileS.jpg"));
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
            { new Tile(' ', 0), 2 }
        }, "Čeština oficiální")
    };

    

    #region Tile dragging

    protected bool isDragging;
    private Point clickPosition;
    private TranslateTransform originTT;

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
                Canvas.SetZIndex(PlayGrid, 1);
                Canvas.SetZIndex(DockGrid, 0);
                break;

            case "DockGrid":
                Canvas.SetZIndex(PlayGrid, 0);
                Canvas.SetZIndex(DockGrid, 1);
                break;
        }
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

            // Change tiles parent if it is not same as the new one
            Grid draggableParent = draggable.Parent as Grid;
            if (draggableParent != PlayGrid) {
                draggableParent.Children.Remove(draggable);
                PlayGrid.Children.Add(draggable);
            }

            // Set the tile correct cell
            Grid.SetColumn(draggable, gameCellColumn);
            Grid.SetRow(draggable, gameCellRow);
        } // DockGrid
        else if (hoverDockCellColumn != null && hoverDockCellRow != null) {
            // int? to int (can't be null anymore)
            int dockCellColumn = (int)hoverDockCellColumn;
            int dockCellRow = (int)hoverDockCellRow;

            // Change tiles parent if it is not same as the new one
            Grid draggableParent = draggable.Parent as Grid;
            if (draggableParent != DockGrid) {
                draggableParent.Children.Remove(draggable);
                DockGrid.Children.Add(draggable);
            }

            // Set the tile correct cell
            Grid.SetColumn(draggable, dockCellColumn);
            Grid.SetRow(draggable, dockCellRow);
        }
        transform.X = 0;
        transform.Y = 0;

        draggable.ReleaseMouseCapture();
    }

    public void TileBlock_MouseMove(object sender, MouseEventArgs e) {
        var draggableControl = sender as Image;
        if (isDragging && draggableControl != null) {
            Point currentPosition = e.GetPosition(this);
            var transform = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
            transform.X = originTT.X + (currentPosition.X - clickPosition.X) * GetActualWidthMultiplier();
            transform.Y = originTT.Y + (currentPosition.Y - clickPosition.Y) * GetActualHeightMultiplier();
            draggableControl.RenderTransform = new TranslateTransform(transform.X, transform.Y);
        }
    }

    #endregion

    private Tile[] DockGridLoad() {
        Tile[] tiles = new Tile[7];

        foreach (TileBlock tileBlock in DockGrid.Children) {
            tiles[DockGrid.Children.IndexOf(tileBlock)] = tileBlock.tile;
        }

        return tiles;
    }

    private void DockGridFill() {
        Tile[] dockGridTiles = DockGridLoad();
        for (int i = 0; i < DockGrid.ColumnDefinitions.Count; i++) {
            if (dockGridTiles[i] == null)
                return;

            DockGrid.Children.Add(new TileBlock(, $"tile{dockGridTiles[i].character}.jpg"));
        }
    }

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
        int lastDockRow = DockGrid.RowDefinitions.Count - 1;
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