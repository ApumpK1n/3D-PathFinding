using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pumpkin
{
    public class OctTree : MonoBehaviour
    {
        [SerializeField] private float minCellSize = 2;
        [SerializeField] private LayerMask mask = -1;


        /*
        .     111
        .
        .
        000 ...
        */
        public static readonly Vector3[] splitDirs = { new Vector3(-1, -1, -1), new Vector3(1, -1, -1), new Vector3(-1, 1, -1), new Vector3(1, 1, -1), new Vector3(-1, -1, 1), new Vector3(1, -1, 1), new Vector3(-1, 1, 1), new Vector3(1, 1, 1)};
        public enum NeighborDir
        {
            Left,
            Right,
            Top,
            Bottom,
            Front,
            Back,
        }


        public static 

        private BoxCollider boxCollider;
            
        private Element root;

        private Queue<Element> toBeSplit;

        private int cellCount;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            root = new Element(null, boxCollider.bounds, 0);
            toBeSplit = new Queue<Element>();

        }

        private void Start()
        {
            toBeSplit.Enqueue(root);
		    cellCount++;
		    float doubleMinCellSize = minCellSize * 2f;

            while (toBeSplit.Count > 0)
            {
                var element = toBeSplit.Dequeue();

                element.IsEmpty = !Physics.CheckBox(element.Bounds.center, element.Bounds.extents, Quaternion.identity, mask, QueryTriggerInteraction.Ignore);

                if (element.Bounds.size.magnitude > doubleMinCellSize && !element.IsEmpty)
                {
                    element.Split(0.5f, splitDirs);

                    foreach (var child in element.Children)
                    {
                        toBeSplit.Enqueue(child);
                        cellCount++;
                    }
                }
            }

            CalculateNeighborsRecursive(root);
        }

        private void CalculateNeighborsRecursive(Element element)
        {
            if (element.Children == null && element != root) // 叶节点
            {
                element.Neigbors = new Element[6];
                for (int i = 0; i < 6; i++)
                {
                    List<Element> neighbors = new List<Element>();
                    GetNeighbors(element, (NeighborDir)i, neighbors);
                    element.Neigbors[i] = neighbors.ToArray();
                }
            }
            else
            {
                for (int i = 0; i < element.Children.Length; i++)
                {
                    CalculateNeighborsRecursive(element.Children[i]);
                }
            }
        }


        private Vector3Int OffsetCoordinateInSameParent(NeighborDir dir, Vector3Int coordinate)
        {
            switch (dir)
            {
                case NeighborDir.Left:
                    coordinate.x -= 1;
                    break;
                case NeighborDir.Right:
                    coordinate.x += 1;
                    break;
                case NeighborDir.Top:
                    coordinate.z += 1;
                    break;
                case NeighborDir.Bottom:
                    coordinate.z -= 1;
                    break;
                case NeighborDir.Front:
                    coordinate.y += 1;
                    break;
                case NeighborDir.Back:
                    coordinate.y -= 1;
                    break;
            }
            return coordinate;
        }

        private Vector3Int OffsetCoordinateCrossNode(NeighborDir dir, Vector3Int coordinate)
        {
            switch (dir)
            {
                case NeighborDir.Left:
                    coordinate.x += 1;
                    break;
                case NeighborDir.Right:
                    coordinate.x -= 1;
                    break;
                case NeighborDir.Top:
                    coordinate.z -= 1;
                    break;
                case NeighborDir.Bottom:
                    coordinate.z += 1;
                    break;
                case NeighborDir.Front:
                    coordinate.y -= 1;
                    break;
                case NeighborDir.Back:
                    coordinate.y += 1;
                    break;
            }
            return coordinate;
        }

        private void GetNeighbors(Element element, NeighborDir neighborDir, List<Element> neighbors)
        {
            Vector3Int coordinate = OffsetCoordinateInSameParent(neighborDir, element.Coordinate);

            Element neighbor = null;
            if (element.IsCoordinateValidInParent(coordinate.x, coordinate.y, coordinate.z))
            {
                int index = element.GetIndex(coordinate.x, coordinate.y, coordinate.z);
                neighbor = element.parentNode.Children[index];
            }
            else
            {
                Element searchNode = element.parentNode.parentNode;
                Element parentNode = element.parentNode;
                if (searchNode != null)
                {
                    Vector3Int parentCoordinate = OffsetCoordinateInSameParent(neighborDir, parentNode.Coordinate);
                    // 边界的点对应方向邻居为null
                    if (parentNode.IsCoordinateValidInParent(parentCoordinate.x, parentCoordinate.y, parentCoordinate.z))
                    {
                        // 找到相邻父节点
                        int index = parentNode.GetIndex(parentCoordinate.x, parentCoordinate.y, parentCoordinate.z);
                        Element parentNeighbor = searchNode.Children[index];
                        if (parentNeighbor.IsEmpty) // 相邻父节点没有分割，邻居设定为父节点
                        {
                            neighbor = parentNeighbor;
                        }
                        else
                        {
                            // 跨节点坐标寻找邻居
                            Vector3Int neighborCoordinate = OffsetCoordinateCrossNode(neighborDir, element.Coordinate);
                            int neighborIndex = parentNeighbor.GetIndex(neighborCoordinate.x, neighborCoordinate.y, neighborCoordinate.z);
                            neighbor = parentNeighbor.Children[neighborIndex];
                        }

                    }
                }
            }

            neighbors.Add(neighbor);

        }
    }
}
