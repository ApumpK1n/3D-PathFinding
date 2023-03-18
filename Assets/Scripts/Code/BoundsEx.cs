using UnityEngine;
using System.Collections;

namespace Pumpkin
{
    public static class BoundsEx
    {
        /// <summary>
        /// 绘制包围盒
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="color"></param>
        public static void DrawBounds(this Bounds bounds, Color color)
        {
            Gizmos.color = color;

            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        /// <summary>
        /// 判断bounds是否在相机内
        /// </summary>
        /// <param name="bound"></param>
        /// <param name="camera"></param>
        /// <param name="viewRatio"></param>
        /// <returns></returns>
        public static bool CheckBoundIsInCamera(this Bounds bound, Camera camera, float viewRatio = 1)
        {
            System.Func<Vector4, int> ComputeOutCode = (projectionPos) =>
            {
                int _code = 0;
                if (projectionPos.x < -projectionPos.w) _code |= 1;
                if (projectionPos.x > projectionPos.w) _code |= 2;
                if (projectionPos.y < -projectionPos.w) _code |= 4;
                if (projectionPos.y > projectionPos.w) _code |= 8;
                if (projectionPos.z < -projectionPos.w) _code |= 16;
                if (projectionPos.z > projectionPos.w) _code |= 32;
                return _code;
            };

            Vector4 worldPos = Vector4.one;
            int code = 63; // 0111111
            for (int i = -1; i <= 1; i += 2)
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    for (int k = -1; k <= 1; k += 2)
                    {
                        worldPos.x = bound.center.x + i * bound.extents.x;
                        worldPos.y = bound.center.y + j * bound.extents.y;
                        worldPos.z = bound.center.z + k * bound.extents.z;

                        code &= ComputeOutCode(camera.projectionMatrix * camera.worldToCameraMatrix * worldPos * viewRatio);
                    }
                }
            }
            return code == 0 ? true : false;
        }

        /// <summary>
        /// 合并两个包围盒
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="boundsUnion"></param>
        /// <returns></returns>
        public static Bounds Union(this Bounds bounds, Bounds boundsUnion)
        {
            if (boundsUnion.size == Vector3.zero) return bounds;
            if (bounds.size == Vector3.zero) return boundsUnion;

            Vector3 min = Vector3.Min(bounds.min, boundsUnion.min);
            Vector3 max = Vector3.Max(bounds.max, boundsUnion.max);

            Bounds result = new Bounds(Vector3.zero, Vector3.zero);
            result.SetMinMax(min, max);
            return result;
        }

        /// <summary>
        /// 判断AABB哪个轴的长度最长
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static int MaxExtent(this Bounds bounds)
        {
            Vector3 d = bounds.max - bounds.min;
            if (d.x > d.y && d.x > d.z)
                return 0; //x
            else if (d.y > d.z)
                return 1; //y
            else
                return 2; //z
        }
    }

}
