namespace Scrabblos;

class TileSet {

    public string Name { get; private set; } // Name of the Tile set

    public Tile[] TileArray { private set; get; } // Array of tiles
    public bool[] UsedArray { private set; get; } // Array about availability of tiles

    public TileSet(Dictionary<Tile, int> tiles, string name = "") {
        Name = name;

        List<Tile> tilesTemp = new();
        List<bool> usedTemp = new();
        foreach (var tileBundle in tiles) {
            for (int i = 0; i < tileBundle.Value - 1; i++) {
                tileBundle.Key.Index = i;
                tilesTemp.Add(tileBundle.Key);
                usedTemp.Add(false);
            }
        }
        TileArray = tilesTemp.ToArray();
        UsedArray = usedTemp.ToArray();
    }
}
