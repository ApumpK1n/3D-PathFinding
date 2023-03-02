using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pumpkin
{
    public enum PathfindingAlgorith
    {
        AStar,
        Greedy
    }

    public class PathFinder
    {
        private PathfindingAlgorith algorithm = PathfindingAlgorith.AStar;
    }
}
