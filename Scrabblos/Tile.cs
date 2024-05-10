namespace Scrabblos;

public class Tile {
    public char character { private set; get; }
    public int points { private set; get; }

    // Index of the tile in the set (for tracking)
    public int index = -1;

    public Tile(char character, int points) {
        this.character = character;
        this.points = points;
    }
}
