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

        public float hCost; // 估算
        public float gCost; // 实际消耗
        public Bounds Bounds;
        public int Depth;
        public bool IsEmpty = false;

        // 基于父节点的排序
        public int x; // 2^0
        public int y; // 2^2
        public int z; // 2^1


        public Element[] Children;
        public Element[] Neigbors;

        public Element parentNode; //树构建

        public Element searchingParentNode; // 寻路回溯

        public static int ChildrenCount = 8;

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
                return x * 1 + z * 2 + y * 2 * 2;
            }
        }

        public static int GetIndex(int x, int y, int z)
        {
            if (!IsCoordinateValidInParent(x, y, z))
            {
                return -1;
            }
            else
            {
                return x * 1 + z * 2 + y * 2 * 2;
            }

        }

        public static int GetIndex(Vector3Int coordinate)
        {
            return GetIndex(coordinate.x, coordinate.y, coordinate.z);
        }

        public static bool IsCoordinateValidInParent(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0) return false;
            if (x > 1 || y > 1 || z > 1) return false;
            return true;
        }

        public static bool IsCoordinateValidInParent(Vector3Int coordinate)
        {
            return IsCoordinateValidInParent(coordinate.x, coordinate.y, coordinate.z);
        }

        public void Split(float splitScale, Vector3[] splitDirs)
        {
            Children = new Element[splitDirs.Length];
            
            for(int x =0; x<2; x++)
            {
                for(int y=0; y<2; y++)
                {
                    for(int z=0; z<2; z++)
                    {
                        int i = GetIndex(x, y, z);
                        Element child = new Element(this, new Bounds(Bounds.center + Vector3.Scale(splitDirs[i], Bounds.extents * splitScale), Bounds.extents), Depth + 1)
                        {
                            x = x,
                            y = y,
                            z = z
                        };
                        Children[i] = child;
                    }
                }
            }
        }

#if UNITY_EDITOR
        public void DrawBounds()
        {
            Color minDepth = Color.white;
            Color maxDepth = Color.red;

            float d = (float)Depth / 100;
            Color color = Color.Lerp(minDepth, maxDepth, d);

            if (Children != null)
            {
                for (int i = 0; i < Children.Length; ++i)
                {
                    Element node = Children[i];
                    if (node != null)
                    {
                        node.DrawBounds();
                    }

                }
            }
            //Bounds.DrawBounds(color);
            if (Neigbors != null && Neigbors.Length >= 6)
            {
                DrawNeighbors();
            }


        }

        public void DrawNeighbors(Color color)
        {
            if (Neigbors != null)
            {
                for (int i = 0; i < Neigbors.Length; ++i)
                {
                    Element node = Neigbors[i];
                    if (node != null)
                    {
                        node.Bounds.DrawBounds(color);
                    }

                }
            }
        }

        public void DrawNeighbors()
        {
            DrawNeighbors(Color.yellow);
        }

        public void DrawSelf()
        {
            Color color = Color.red;
            Bounds.DrawBounds(color);
        }

        public void DrawSelf(Color color)
        {
            Bounds.DrawBounds(color);
        }
#endif
    }
}

