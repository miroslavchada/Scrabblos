namespace Scrabblos;

public class Tile {
    public char character { private set; get; }
    public int points { private set; get; }

    public Tile(char character, int points) {
        this.character = character;
        this.points = points;
    }
}
