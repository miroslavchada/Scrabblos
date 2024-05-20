﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabblos {
    internal class Player {
        public string Name { get; private set; }
        public int Score = 0;
        private TileBlock[] Dock = new TileBlock[7];

        public Player(string name) {
            Name = name;
        }

        public void SetDock(TileBlock[] dock) {
            Array.Copy(dock, Dock, dock.Length);
        }

        public TileBlock[] GetDock() {
            return Dock;
        }
    }
}
