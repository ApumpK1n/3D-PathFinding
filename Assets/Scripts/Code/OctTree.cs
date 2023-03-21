using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public static readonly Vector3[] splitDirs = { new Vector3(-1, -1, -1), new Vector3(1, -1, -1), new Vector3(-1, -1, 1), new Vector3(1, -1, 1), new Vector3(-1, 1, -1), new Vector3(1, 1, -1), new Vector3(-1, 1, 1), new Vector3(1, 1, 1)};
        public enum NeighborDir
        {
            Left,
            Right,
            Top,
            Bottom,
            Front,
            Back,
        }

        public bool IsBuilding { get { return toBeSplit.Count > 0; } }

        private BoxCollider boxCollider;
            
        private Element root;

        private Queue<Element> toBeSplit;

        private int cellCount;

        private Vector3 tmpGetNodePos;

        private Element tmpStart;
        private Element tmpEnd;

        private List<Element> elements = new List<Element>();

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

                if (element.Bounds.size.magnitude > doubleMinCellSize /*&& !element.IsEmpty*/)
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
                    Element neighbor = GetNeighbor(element, (NeighborDir)i, element.Depth, element.Bounds.center);
                    if (neighbor == null) elements.Add(element);
                    element.Neigbors[i] = neighbor;
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
                    coordinate.y += 1;
                    break;
                case NeighborDir.Bottom:
                    coordinate.y -= 1;
                    break;
                case NeighborDir.Front:
                    coordinate.z += 1;
                    break;
                case NeighborDir.Back:
                    coordinate.z -= 1;
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
                    coordinate.y -= 1;
                    break;
                case NeighborDir.Bottom:
                    coordinate.y += 1;
                    break;
                case NeighborDir.Front:
                    coordinate.z -= 1;
                    break;
                case NeighborDir.Back:
                    coordinate.z += 1;
                    break;
            }
            return coordinate;
        }


        private Element GetNeighbor(Element element, NeighborDir neighborDir, int startDepth, Vector3 center)
        {
            Element parent = element.parentNode;
            if (parent == null) return null;

            Vector3Int coordinate = OffsetCoordinateInSameParent(neighborDir, element.Coordinate);

            Element neighbor = null;
            // 先尝试在同级下找
            if (Element.IsCoordinateValidInParent(coordinate))
            {
                int index = Element.GetIndex(coordinate);
                neighbor = parent.Children[index];
            } 
            else 
            {
                // 跨节点
                Element topmostNeighbor = GetNeighbor(parent, neighborDir, startDepth, center);
                if (topmostNeighbor == null) 
                {
                    return null; // 边缘
                } 

                Element node = topmostNeighbor;
                neighbor = node;
                while (node.Children != null && node.Depth < startDepth)
                {
                    //Element searchNode = parentNode.parentNode;

                    //Vector3Int parentOffsetCoordinate = OffsetCoordinateInSameParent(neighborDir, parentNode.Coordinate);
                    //if (Element.IsCoordinateValidInParent(parentOffsetCoordinate))
                    //{
                    //    // 找到相邻父节点
                    //    int index = Element.GetIndex(parentOffsetCoordinate);
                    //    Element parentNeighbor = searchNode.Children[index];
                    //    if (parentNeighbor.IsEmpty) // 相邻父节点没有分割，邻居设定为父节点
                    //    {
                    //        neighbor = parentNeighbor;
                    //    }
                    //    else
                    //    {  
                    //        // 跨节点坐标寻找邻居
                    //        Vector3Int neighborCoordinate = OffsetCoordinateCrossNode(neighborDir, element.Coordinate);
                    //        int neighborIndex = Element.GetIndex(neighborCoordinate);
                    //        neighbor = parentNeighbor.Children[neighborIndex];
                    //    }

                    //    break;
                    //}
                    //else
                    //{
                    //    // 父节点边界的点需要再往上找，直到找到为止
                    //    parentNode = searchNode;
                    //}
                    // 跨节点坐标寻找邻居
                    //int neighborIndex;
                    //if (node.Depth == startDepth + 1)
                    //{
                    //    Vector3Int neighborCoordinate = OffsetCoordinateCrossNode(neighborDir, startCoordinate);
                    //    neighborIndex = Element.GetIndex(neighborCoordinate);
                    //}
                    //else
                    //{
                    //    neighborIndex = Element.GetIndex(node.Coordinate);
                    //}
                    float distance = float.MaxValue;
                    foreach(var ele in node.Children)
                    {
                        float dis = Vector3.Distance(ele.Bounds.center, center);
                        if (dis < distance)
                        {
                            neighbor = ele;
                            distance = dis;
                        }
                    }

                    node = neighbor;

                }
            }

            return neighbor;
        }

        public PathRequest GetPath(Vector3 start, Vector3 end)
        {
            Element elementA = GetNode(start);
            Element elementB = GetNode(end);
            if (elementA != null && elementB != null)
            {
                tmpStart = elementA;
                tmpEnd = elementB;
                return PathFinderManager.GetInstance().RequestGetPath(elementA, elementB);
            }
            return null;
        }

        private Element GetNode(Vector3 position)
        {
            tmpGetNodePos = position;
            return GetNode(root);
        }

        private Element GetNode(Element node)
        {
            if (node.Bounds.Contains(tmpGetNodePos))
            {
                if (node.Children != null)
                {
                    for (int i = 0; i < node.Children.Length; i++)
                    {
                        Element child = GetNode(node.Children[i]);
                        if (child != null) return child;
                    }
                }
                else
                {
                    return node;
                }
            }
            return null;
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (root == null) return;
            //root.DrawBounds();
            if (tmpStart == null || tmpEnd == null) return;
            //tmpStart.DrawSelf();
            //tmpEnd.DrawSelf();

            tmpStart.DrawNeighbors(Color.black);

            tmpEnd.DrawNeighbors(Color.black);

        }
    }
#endif
}
