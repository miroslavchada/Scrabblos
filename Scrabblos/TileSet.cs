using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Scrabblos;

class TileSet {
    public TileSet (List<Tile> tiles, string name = "") {
        
    }

    public TileSet(string path)
    {
        Tile? deserialized = JsonSerializer.Deserialize<Tile>(path);
    }

    public List<Tile> GetAllTiles()
    {
        List<Tile> tileList = new();

        return tileList;
    }
}
