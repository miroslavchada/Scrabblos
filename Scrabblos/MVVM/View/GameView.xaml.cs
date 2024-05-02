﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Scrabblos.MVVM.View;

/// <summary>
/// Interakční logika pro GameView.xaml
/// </summary>
public partial class GameView : UserControl {
    public GameView() {
        InitializeComponent();
    }

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

    private bool escapeMenu = false;

    #region Tile dragging

    protected bool isDragging;
    private Point clickPosition;
    private TranslateTransform originTT;

    private bool hoveringPlayGrid;

    private void Tile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
        var draggableControl = sender as Image;
        originTT = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
        isDragging = true;
        clickPosition = e.GetPosition(this);
        draggableControl.CaptureMouse();
    }

    private void Tile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
        isDragging = false;
        var draggable = sender as Image;
        var transform = draggable.RenderTransform as TranslateTransform ?? new TranslateTransform();
        // Snap to grid if placed correctly
        if (false) {

        } else {
            // Else get back to origin
            transform.X = originTT.X;
            transform.Y = originTT.Y;
        }
        draggable.ReleaseMouseCapture();
    }

    private void Tile_MouseMove(object sender, MouseEventArgs e) {
        var draggableControl = sender as Image;
        if (isDragging && draggableControl != null) {
            Point currentPosition = e.GetPosition(this);
            var transform = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
            transform.X = originTT.X + (currentPosition.X - clickPosition.X) * GetActualWidthMultiplier();
            transform.Y = originTT.Y + (currentPosition.Y - clickPosition.Y) * GetActualHeightMultiplier();
            draggableControl.RenderTransform = new TranslateTransform(transform.X, transform.Y);
        }
    }
    private void PlayGrid_OnMouseEnter(object sender, MouseEventArgs e) {
        hoveringPlayGrid = true;
    }

    private void PlayGrid_OnMouseLeave(object sender, MouseEventArgs e) {
        hoveringPlayGrid = false;
        TBlockInfo.Text = "Informace jak pán";
    }

    private void PlayGrid_OnMouseMove(object sender, MouseEventArgs e)
    {
        TBlockInfo.Text = $"";
    }

    #endregion

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
}