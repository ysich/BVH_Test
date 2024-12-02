using System.Collections.Generic;
using UnityEngine;

namespace TOctree
{
    public class OctreeNode
    {
        public List<GameObject> areaObjects;

        public int areaObjectCount => areaObjects.Count;

        public Vector3 center;

        public float size;
        
        /// <summary>
        /// 因为是八叉树所以每个节点都有8个子节点
        /// </summary>
        public const int childNodeCount = 8;

        public OctreeNode[] childNodes = new OctreeNode[childNodeCount];

        public OctreeNode(Vector3 center, float size)
        {
            this.center = center;
            this.size = size;
            areaObjects = new List<GameObject>();
        }

        public bool Contains(Vector3 position)
        {
            var halfSize = size * 0.5f;
            return Mathf.Abs(position.x - center.x) <= halfSize &&
                   Mathf.Abs(position.y - center.y) <= halfSize &&
                   Mathf.Abs(position.z - center.z) <= halfSize;
        }

        public void ClearAreaGameObjects()
        {
            areaObjects.Clear();
        }
        
        public void AddAreaGameObject(GameObject gameObject)
        {
            areaObjects.Add(gameObject);
        }

        #region 节点扩展

        public OctreeNode top1
        {
            get { return childNodes[0]; }
            set { childNodes[0] = value; }
        }

        public OctreeNode top2
        {
            get { return childNodes[1]; }
            set { childNodes[1] = value; }
        }

        public OctreeNode top3
        {
            get { return childNodes[2]; }
            set { childNodes[2] = value; }
        }

        public OctreeNode top4
        {
            get { return childNodes[3]; }
            set { childNodes[3] = value; }
        }

        public OctreeNode bottom1
        {
            get { return childNodes[4]; }
            set { childNodes[4] = value; }
        }

        public OctreeNode bottom2
        {
            get { return childNodes[5]; }
            set { childNodes[5] = value; }
        }

        public OctreeNode bottom3
        {
            get { return childNodes[6]; }
            set { childNodes[6] = value; }
        }

        public OctreeNode bottom4
        {
            get { return childNodes[7]; }
            set { childNodes[7] = value; }
        }

        #endregion
        
    }
}