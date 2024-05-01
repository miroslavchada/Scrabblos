using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Scrabblos
{
    /// <summary>
    /// Pro použití tohoto vlastního ovládacího prvku v souboru XAML postupujte podle kroků 1 a pak 2.
    ///
    /// Krok 1) Použití tohoto vlastního ovládacího prvku v XAML souboru, který už je v aktuálním projektu.
    /// Přidejte tento XmlNamespace atribut do kořenového elementu označovacího souboru, kde 
    /// bude použit:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Scrabblos"
    ///
    /// Krok 2)
    /// Pokračujte dále a použijte svůj ovládací prvek v souboru XAML.
    ///
    ///     <MyNamespace:TileBlock/>
    ///
    /// </summary>
    
    public class TileBlock : Image {
        private Tile tile;

        public TileBlock(Tile tile) {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TileBlock), new FrameworkPropertyMetadata(typeof(Image)));

            this.tile = tile;
        }
    }
}
