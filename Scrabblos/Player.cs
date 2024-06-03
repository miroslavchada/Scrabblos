namespace Scrabblos {
    internal class Player {
        public string Name { get; private set; }
        public int Score = 0;
        private readonly TileBlock?[] _dock = new TileBlock[7];

        public Player(string name) {
            Name = name;
        }

        public void SetDock(TileBlock[] dock) => Array.Copy(dock, _dock, dock.Length);

        public TileBlock[] GetDock() => _dock;

        public int GetDockValue() {
            int dockValue = 0;
            foreach (TileBlock tileBlock in _dock) {
                if (tileBlock != null) {
                    dockValue += tileBlock.Tile.Points;
                }
            }
            return dockValue;
        }

        public int GetDockCount() {
            int count = 0;
            foreach (TileBlock tileBlock in _dock) {
                if (tileBlock != null) {
                    count++;
                }
            }
            return count;
        }
    }
}
