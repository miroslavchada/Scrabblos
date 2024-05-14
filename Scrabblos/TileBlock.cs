using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Scrabblos.MVVM.View;

namespace Scrabblos;

class TileBlock : Image
{
    public Tile tile { get; private set; }

    public TileBlock(Tile tile, string resourceName)
    {
        this.tile = tile;
        BitmapImage image = new BitmapImage(new Uri($"pack://application:,,,/Scrabblos;component/Resources/{resourceName}"));
        Source = image;
        Height = 96;
        Width = 96;
        Margin = new System.Windows.Thickness(0);

        HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
        VerticalAlignment = System.Windows.VerticalAlignment.Center;

        MouseLeftButtonDown += GameView.Instance.TileBlock_MouseLeftButtonDown;
        MouseLeftButtonUp += GameView.Instance.TileBlock_MouseLeftButtonUp;
        MouseMove += GameView.Instance.TileBlock_MouseMove;
    }
}