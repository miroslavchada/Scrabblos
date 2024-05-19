using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabblos {
    internal class GameManager {
        public static GameManager Instance { get; private set; }

        private GameManager() {
            Instance = this;
        }
    }
}
