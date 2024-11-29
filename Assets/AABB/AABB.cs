/*---------------------------------------------------------------------------------------
-- 负责人: onemt
-- 创建时间: 2024-11-29 11:29:33
-- 概述:
---------------------------------------------------------------------------------------*/

using System;
using UnityEngine;

namespace AABB
{
    public class AABB
    {
        /// <summary>
        /// 左上角
        /// </summary>
        public Vector3 minCorner;
        /// <summary>
        /// 右下角
        /// </summary>
        public Vector3 maxCorner;

        public Vector3 size => maxCorner - minCorner;
        public Vector3 center => (maxCorner + minCorner) * 0.5f;

        public void Reset()
        {
            this.minCorner = Vector3.one * float.MinValue;
            this.maxCorner = Vector3.one * float.MaxValue;
        }
        public void Reset(Vector3 minCorner,Vector3 maxCorner)
        {
            this.minCorner = minCorner;
            this.maxCorner = maxCorner;
        }
        
        public AABB(Vector3 minCorner,Vector3 maxCorner)
        {
            this.minCorner = minCorner;
            this.maxCorner = maxCorner;
        }

        public AABB Union(AABB aabb)
        {
            Vector3 newMinCorner = Vector3.zero;
            newMinCorner.x = Mathf.Min(minCorner.x, aabb.minCorner.x);
            newMinCorner.y = Mathf.Min(minCorner.y, aabb.minCorner.y);
            newMinCorner.z = Mathf.Min(minCorner.z, aabb.minCorner.z);

            Vector3 newMaxCorner = Vector3.zero;
            newMaxCorner.x = Mathf.Max(maxCorner.x, aabb.maxCorner.x);
            newMaxCorner.y = Mathf.Max(maxCorner.y, aabb.maxCorner.y);
            newMaxCorner.z = Mathf.Max(maxCorner.z, aabb.maxCorner.z);

            AABB unionAABB = new AABB(newMinCorner, newMaxCorner);
            return unionAABB;
        }

        public bool Intersects(AABB aabb)
        {
            if (minCorner.x > aabb.maxCorner.x || maxCorner.x < aabb.minCorner.x) return false;
            if (minCorner.y > aabb.maxCorner.y || maxCorner.y < aabb.minCorner.y) return false;
            if (minCorner.z > aabb.maxCorner.z || maxCorner.z < aabb.minCorner.z) return false;
            return true;
        }

        public bool ContainPoint(Vector3 point)
        {
            if (point.x < minCorner.x) return false;
            if (point.y < minCorner.y) return false;
            if (point.z < minCorner.z) return false;
            if (point.x > maxCorner.x) return false;
            if (point.y > maxCorner.y) return false;
            if (point.z > maxCorner.z) return false;
            return true;
        }
        
    }
}