using TAABB;
using UnityEngine;

namespace TBVH
{
    public class BVHNode
    {
        public BVHNode leftNode;
        
        public BVHNode rightNode;
        
        public BVHNode parentNode;

        public AABB aabb;

        public GameObject sceneObject;

        #region Debug

        public string name;
        private Color spaceColor;

        public void DrawGizmos()
        {
            Gizmos.color = spaceColor;
            Gizmos.DrawWireCube(this.aabb.center,this.aabb.size);
            Gizmos.DrawSphere(this.aabb.minCorner,0.1f);
            Gizmos.DrawSphere(this.aabb.maxCorner,0.1f);
        }

        public void DrawDepth(int depth)
        {
            if (depth > 0)
            {
                DrawGizmos();
                this.leftNode?.DrawDepth(depth - 1);
                this.rightNode?.DrawDepth(depth - 1);
            }
        }

        public void DrawTargetDepth(int depth)
        {
            if (depth <= 0)
            {
                DrawGizmos();
            }
            else
            {
                this.rightNode?.DrawTargetDepth(depth - 1);
                this.leftNode?.DrawTargetDepth(depth - 1);
            }
        }

        #endregion

        public BVHNode(string name,GameObject sceneObject)
        {
            BindSceneObject(sceneObject);
            InitialAABB();

            this.name = name;
            spaceColor = new Color(Random.value, Random.value, Random.value, 0.9f);
        }

        public void BindSceneObject(GameObject sceneObject)
        {
            this.sceneObject = sceneObject;
        }

        public void SetLeafNode(BVHNode left, BVHNode right)
        {
            this.leftNode = left;
            this.rightNode = right;
            if (left != null)
            {
                left.parentNode = this;
            }

            if (right != null)
            {
                right.parentNode = this;
            }

            this.sceneObject = null;
        }

        /// <summary>
        /// 初始化AABB
        /// </summary>
        public void InitialAABB()
        {
            ResetAABB();
            if (sceneObject == null) return;
            this.aabb = ComputeWordAABB(sceneObject);
            UnionAABB(aabb);
        }

        public static AABB ComputeWordAABB(GameObject obj)
        {
            //这里实际可能还有旋转缩放等操作。
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            var localMin = meshFilter.sharedMesh.bounds.min;
            var localMax = meshFilter.sharedMesh.bounds.max;
            var min = obj.transform.TransformPoint(localMin);
            var max = obj.transform.TransformPoint(localMax);
            AABB aabb = new AABB(min, max);
            return aabb;
        }

        public void ResetAABB()
        {
            this.aabb.Reset();
        }

        public void UnionAABB(AABB aabb)
        {
            AABB unionAABB = this.aabb.Union(aabb);
            this.aabb = unionAABB;
        }
    }
}