using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Scrabblos.MVVM.View;

namespace Scrabblos;

class TileBlock : Image
{
    public Tile Tile { get; private set; }

    private readonly float _scale = .92f;

    public bool MarkedForExchange { get; private set; }

    public TileBlock(Tile tile, string resourceName)
    {
        Tile = tile;
        BitmapImage image = new BitmapImage(new Uri($"pack://application:,,,/Scrabblos;component/Resources/{resourceName}"));
        Source = image;
        Height = 96 * _scale;
        Width = 96 * _scale;
        Margin = new System.Windows.Thickness(0);
        MarkedForExchange = false;

        HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
        VerticalAlignment = System.Windows.VerticalAlignment.Center;

        MouseLeftButtonDown += GameView.Instance.TileBlock_MouseLeftButtonDown;
        MouseLeftButtonUp += GameView.Instance.TileBlock_MouseLeftButtonUp;
        MouseMove += GameView.Instance.TileBlock_MouseMove;
    }

    public void ToggleExchangeMark() {
        MarkedForExchange = !MarkedForExchange;
        Opacity = !MarkedForExchange ? 1 : 0.4;
    }

    public void CancelExchangeMark() {
        MarkedForExchange = false;
        Opacity = 1;
    }

    public void UnsubscribeInteraction() {
        MouseLeftButtonDown -= GameView.Instance.TileBlock_MouseLeftButtonDown;
        MouseLeftButtonUp -= GameView.Instance.TileBlock_MouseLeftButtonUp;
        MouseMove -= GameView.Instance.TileBlock_MouseMove;
    }
}