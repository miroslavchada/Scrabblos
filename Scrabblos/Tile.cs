namespace Scrabblos;

public class Tile {
    public char Character { private set; get; }
    public int Points { private set; get; }

    public Tile(char character, int points) {
        Character = character;
        Points = points;
    }
}
