namespace Scrabblos {
    internal class Player {
        public string Name { get; private set; }
        public int Score = 0;
        private readonly TileBlock[] _dock = new TileBlock[7];

        public Player(string name) {
            Name = name;
        }

        public void SetDock(TileBlock[] dock) {
            Array.Copy(dock, _dock, dock.Length);
        }

        public TileBlock[] GetDock() {
            return _dock;
        }
    }
}
