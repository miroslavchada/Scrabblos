namespace Scrabblos;

class TileSet {

    public string name { get; private set; } // Name of the tile set

    public Tile[] tileArray { private set; get; } // Array of tiles
    public bool[] usedArray { private set; get; } // Array of availability of tiles

    public TileSet(Dictionary<Tile, int> tiles, string name = "") {
        this.name = name;

        List<Tile> tilesTemp = new();
        List<bool> usedTemp = new();
        foreach (var tileBundle in tiles) {
            for (int i = 0; i < tileBundle.Value - 1; i++) {
                tileBundle.Key.index = i;
                tilesTemp.Add(tileBundle.Key);
                usedTemp.Add(false);
            }
        }
        this.tileArray = tilesTemp.ToArray();
        usedArray = usedTemp.ToArray();
    }
}
