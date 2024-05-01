namespace Scrabblos;

class TileSet {

    public string name { get; } // Name of the tile set
    public Dictionary<Tile, int> set { get; } // Dictionary of tiles and their counts

    public TileSet(Dictionary<Tile, int> tiles, string name = "") {
        this.name = name;
        this.set = tiles;
    }
}
