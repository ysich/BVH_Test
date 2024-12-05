
using System.Collections.Generic;
using TAABB;
using UnityEngine;

namespace TBVH
{
    public class DynamicBVHSpace
    {
        public BVHNode rootNode { get; private set; }

        private List<BVHNode> m_LeafNodes = new List<BVHNode>();
        private int generateCount = 0;

        public Dictionary<GameObject, BVHNode> gameObject2Node = new Dictionary<GameObject, BVHNode>();

        public void RecordGameObject(BVHNode node)
        {
            GameObject sceneObj = node.sceneObject;
            if(sceneObj == null)
                return;
            gameObject2Node[sceneObj] = node;
        }
        
        public BVHNode AddNode(GameObject gameObject)
        {
            BVHNode leaf = new BVHNode($"node_{generateCount}", gameObject);
            RecordGameObject(leaf);

            BuildBVH(leaf);

            generateCount++;
            return leaf;
        }

        public void BuildBVH(BVHNode leafNode)
        {
            //说明是第一次构建，则指定为Root节点
            if (rootNode == null)
            {
                rootNode = leafNode;
                m_LeafNodes.Add(leafNode);
                return;
            }
            //使用SAH的方式找到表面积差最小的节点进行插入
            BVHNode targetNode = SAH(leafNode);
            BVHNode newNode = BVHNode.CombineNodes(targetNode, leafNode);
            
            //因为在叶子节点插入了一个新节点，所以就不是叶子节点了
            m_LeafNodes.Remove(targetNode);
            m_LeafNodes.Add(leafNode);
            m_LeafNodes.Add(newNode);
            
            RecordGameObject(newNode);

            rootNode = newNode.rootNode;
        }

        private BVHNode SAH(BVHNode newNode)
        {
            float minCost = float.MaxValue;
            BVHNode minCostNode = null;

            //遍历所有叶子节点
            foreach (var leaf in m_LeafNodes)
            {
                AABB newBranchAABB = leaf.aabb.Union(newNode.aabb);
                float deltaCost = newBranchAABB.surfaceArea;
                float allCost = deltaCost;
                BVHNode parent = leaf.parentNode;
                while (parent != null)
                {
                    float parentSurfaceArea = parent.surfaceArea;
                    AABB unionAABB = parent.aabb.Union(newNode.aabb);
                    float unionSurfaceArea = unionAABB.surfaceArea;
                    deltaCost = unionSurfaceArea - parentSurfaceArea;
                    allCost += deltaCost;
                    parent = parent.parentNode;
                }

                if (allCost < minCost)
                {
                    minCostNode = leaf;
                    minCost = allCost;
                }
            }

            return minCostNode;
        }

        public void RemoveNode(GameObject gameObject)
        {
            if (gameObject2Node.TryGetValue(gameObject, out BVHNode node))
            {
                //移除叶子节点
                m_LeafNodes.Remove(node);
                //因为这里执行了删除操作，节点的兄弟节点也被平移了所以这里也在管理器里删除一下。
                m_LeafNodes.Remove(node.GetSibling());

                BVHNode subNode = BVHNode.SeparateNodes(node);
                if (subNode.isLeaf)
                {
                    m_LeafNodes.Add(subNode);
                    RecordGameObject(node);
                }
            }
        }
        
        
    }
}