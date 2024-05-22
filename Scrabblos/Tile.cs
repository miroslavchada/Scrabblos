namespace Scrabblos;

public class Tile {
    public char Character { private set; get; }
    public int Points { private set; get; }

    // Index of the Tile in the set (for tracking)
    public int Index = -1;

    public Tile(char character, int points) {
        Character = character;
        Points = points;
    }
}
