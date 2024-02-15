using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RH
{
    [Serializable]
    public class ClassWFCRunner
    {
        public int Width = 8;
        public int Height = 8;
        public ConfigTile ConfigTile;

        private TileDataModel _tileDataModel;
        private ClassWFC _WFC;


        public ClassWFCRunner()
        {
            _tileDataModel = ConfigTile.Convert();
            _WFC = new ClassWFC(Width, Height, _tileDataModel);
        }


        public void Run()
        {
            // Collapses the Wavefunction until it is fully collapsed 
            while (!_WFC.IsFullyCollapsed())
            {
                Iterate();
            }

            _WFC.GetAllCollapsed();
        }

        public void Iterate()
        {
            // Find coordinate with lowest entropy

            // Collapse the wavefunction at that coordinate

            // Propagate the changes
            Propagate((0, 0));
        }

        public void Propagate((int, int) coords)
        {
            Queue<(int, int)> queue = new Queue<(int, int)>();

            while(queue.Count > 0)
            {
                (int, int) currentCoords = queue.Dequeue();
                // Get all possible tiles for the current coordinate

                // For each immediately adjacent tiles
                // foreach

            }
        }
    }
}
