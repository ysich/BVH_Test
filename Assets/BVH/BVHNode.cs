using System;
using TAABB;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TBVH
{
    public class BVHNode
    {
        public BVHNode leftNode;
        
        public BVHNode rightNode;
        
        public BVHNode parentNode;
        public BVHNode rootNode { get; private set; }
        
        public AABB aabb;
        public float surfaceArea => aabb.surfaceArea;
        
        //根据有没有管理GameObject判断是否是叶子节点
        public bool isLeaf => sceneObject != null;

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
            rootNode = this;
            BindSceneObject(sceneObject);
            InitialAABB();

            this.name = name;
            spaceColor = new Color(Random.value, Random.value, Random.value, 0.9f);
        }

        public BVHNode(BVHNode sourceNode)
        {
            this.rootNode = sourceNode.rootNode;
            this.aabb = sourceNode.aabb;
            this.leftNode = sourceNode.leftNode;
            this.rightNode = sourceNode.rightNode;
            this.sceneObject = sourceNode.sceneObject;
            
            spaceColor = new Color(Random.value, Random.value, Random.value, 0.9f);
            this.name = sourceNode.name + "copied";
        }

        public bool ContainChildNode(BVHNode bvhNode)
        {
            if (bvhNode == null)
                return false;
            if (this.leftNode == bvhNode || this.rightNode == bvhNode)
                return true;
            return false;
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
                left.rootNode = this.rootNode;
            }

            if (right != null)
            {
                right.parentNode = this;
                right.rootNode = this.rootNode;
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

        #region Dynamic

        public BVHNode GetSibling()
        {
            return this.parentNode.GetTheOtherNode(this);
        }

        public BVHNode GetTheOtherNode(BVHNode separateNode)
        {
            if (this.leftNode == separateNode) return this.rightNode;
            if (this.rightNode == separateNode) return this.leftNode;
            return null;
        }

        public static BVHNode CombineNodes(BVHNode targetNode, BVHNode insertNode)
        {
            //这里插入/合并节点的思路就是把当前节点复制一份新的，把复制出来的节点和插入节点作为当前节点的子节点。
            //复制目标节点信息
            BVHNode newNode = new BVHNode(targetNode);
            //合并aabb
            targetNode.UnionAABB(insertNode.aabb);
            targetNode.AABBBroadCast();
            
            targetNode.SetLeafNode(newNode,insertNode);

            // //平衡操作
            // Balance(targetNode);
            
            return newNode;
        }
        

        public static BVHNode SeparateNodes(BVHNode separateNode)
        {
            BVHNode parent = separateNode.parentNode;
            if (parent == null || !parent.ContainChildNode(separateNode))
            {
                throw new Exception("节点分离失败，父节点为空/父节点不包含当前节点。");
            }
            BVHNode siblingNode = separateNode.GetSibling();
            //把兄弟节点的子节点丢给父节点
            parent.SetLeafNode(siblingNode.leftNode,siblingNode.rightNode);
            //绑定场景物体
            parent.BindSceneObject(siblingNode.sceneObject);
            //更新AABB,这里不能用UpdateAABB因为叶子节点没有left、right节点
            parent.aabb = siblingNode.aabb;
            //向上广播更新
            parent.AABBBroadCast();
            
            // Balance(parent);

            return parent;
        }

        public void UpdateAABB()
        {
            ResetAABB();

            if (leftNode != null)
            {
                UnionAABB(leftNode.aabb);
            }

            if (rightNode != null)
            {
                UnionAABB(rightNode.aabb);
            }
        }
        
        /// <summary>
        /// 通知所有父节点需要变更AABB
        /// </summary>
        public void AABBBroadCast()
        {
            if (parentNode != null)
            {
                parentNode.UpdateAABB();
                parentNode.AABBBroadCast();
            }
        }

        #endregion

        #region 平衡

        /// <summary>
        /// 平衡因子阈值
        /// </summary>
        private const int k_BalanceThreshold = 1;
        
        private static void Balance(BVHNode node)
        {
            int leftCount = CountNodes(node.leftNode);
            int rightCount = CountNodes(node.rightNode);

            if (Mathf.Abs(leftCount - rightCount) > k_BalanceThreshold)
            {
                Debug.LogWarning("BVH发现失衡，进行平衡处理");
                //左型
                if (leftCount > rightCount)
                {
                    //右旋
                    //旋转后的根节点
                    BVHNode left = node.leftNode;
                    node.leftNode = left.rightNode;
                    left.rightNode = node;
                    node.UpdateAABB();
                    left.UpdateAABB();
                }
                //右型
                else
                {
                    //左旋
                    //旋转后的根节点
                    BVHNode right = node.rightNode;
                    node.rightNode = right.leftNode;
                    right.leftNode = node;
                    node.UpdateAABB();
                    right.UpdateAABB();
                }
            }
        }

        private static int CountNodes(BVHNode node)
        {
            if (node == null)
                return 0;
            if (node.isLeaf)
                return 1;
            return CountNodes(node.leftNode) + CountNodes(node.rightNode);
        }

        #endregion
    }
}