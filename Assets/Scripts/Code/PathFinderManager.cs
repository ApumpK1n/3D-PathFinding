using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Pumpkin
{
    public class PathFinderManager : MonoBehaviour
    {
        private static PathFinderManager instance;

        public delegate void PathfindingJobComplete(List<Element> path);

        PathFinder pathFinder;

        void Awake()
        {
            instance = this;
            pathFinder = new PathFinder();
        }

        public static PathFinderManager GetInstance()
        {
            return instance;
        }

        public List<Element> RequestPathfind(Element start, Element target)
        {
            return pathFinder.FindPath(start, target, PathfindingAlgorith.AStar);
        }

    }
}
