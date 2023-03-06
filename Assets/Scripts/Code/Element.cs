using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pumpkin
{
    public class Element
    {

        public Element(Element parent, Bounds bounds, int depth)
        {
            parentNode = parent;
            Bounds = bounds;
            Depth = depth;
        }

        public float hCost;
        public float gCost;
        public Bounds Bounds;
        public int Depth;
        public bool IsEmpty;

        // 基于父节点的排序
        public int x; // 2^0
        public int y; // 2^1
        public int z; // 2^2


        public Element[] Children;
        public Element[] Neigbors;

        public Element parentNode;

        public float fCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public Vector3Int Coordinate
        {
            get
            {
                return new Vector3Int(x, y, z);
            }
        }

        public int Index
        {
            get
            {
                return x * 1 + y * 2 + z * 2 * 2;
            }
        }

        // change To  Static

        public int GetIndex(int x, int y, int z)
        {
            if (!IsCoordinateValidInParent(x, y, z))
            {
                return -1;
            }
            else
            {
                return x * 1 + y * 2 + z * 2 * 2;
            }

        }

        public bool IsCoordinateValidInParent(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0) return false;
            int count = parentNode.Children.Length;
            if (x >= count || y >= count || z >= count) return false;
            return true;
        }

        public void Split(float splitScale, Vector3[] splitDirs)
        {
            Children = new Element[splitDirs.Length];
            
            for(int x =0; x< 2; x++)
            {
                for(int y=0; y<2; y++)
                {
                    for(int z=0; z<2; z++)
                    {
                        int i = GetIndex(x, y, z);
                        Element child = new Element(this, new Bounds(Bounds.center + Vector3.Scale(splitDirs[i], Bounds.extents * splitScale), Bounds.extents), Depth + 1);
                        child.x = x;
                        child.y = y;
                        child.z = z;
                        Children[i] = child;
                    }
                }
            }
        }
    }
}

