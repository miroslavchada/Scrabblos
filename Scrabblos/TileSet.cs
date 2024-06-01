namespace Scrabblos;

internal class TileSet {

    public string Name { get; private set; } // Name of the Tile set

    public (Tile, bool)[] TileArray { private set; get; } // Array of tiles and its availability

    public TileSet(List<(Tile, int)> tiles, string name = "") {
        Name = name;

        List<(Tile, bool)> tilesTemp = new();
        foreach (var (tile, count) in tiles) {
            for (int i = 0; i < count - 1; i++) {
                tilesTemp.Add((tile, false));
            }
        }
        TileArray = tilesTemp.ToArray();
    }
}
