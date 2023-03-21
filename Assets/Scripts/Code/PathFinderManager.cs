using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Pumpkin
{
    public class PathFinderManager : MonoBehaviour
    {
        private static PathFinderManager instance;

        public delegate void PathfindingJobComplete(List<Element> path);

        private Queue<PathRequest> requests = new Queue<PathRequest>();
        private List<PathRequest> running = new List<PathRequest>();

        [SerializeField] private float maxActivePathfinds = 6;

        PathFinder pathFinder;

        void Awake()
        {
            instance = this;
            pathFinder = new PathFinder();
        }

        void Update()
        {
            if (requests.Count > 0 && running.Count < maxActivePathfinds)
            {
                var newRequest = requests.Dequeue();
                ThreadPool.QueueUserWorkItem(GetPathAstar, newRequest);
                running.Add(newRequest);
            }

            if (running.Count > 0)
            {
                for (int i = running.Count - 1; i >= 0; i--)
                {
                    if (!running[i].isCalculating)
                    {
                        running.RemoveAt(i);
                    }
                }
            }
        }

        public static PathFinderManager GetInstance()
        {
            return instance;
        }

        public PathRequest RequestGetPath(Element start, Element end)
        {
            PathRequest request = new PathRequest() { from = start, to = end, isCalculating = true };
            requests.Enqueue(request);
            return request;
        }

        private void GetPathAstar(object context)
        {
            PathRequest request = (PathRequest)context;
            try
            {
                pathFinder.FindPath(request.from, request.to, PathfindingAlgorith.AStar, request.Path);
                if (request.Path.Count == 0)
                {
                    Debug.Log("Find 0 node");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                request.isCalculating = false;
            }
        }

    }
}
