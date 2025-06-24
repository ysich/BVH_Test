/*---------------------------------------------------------------------------------------
-- 负责人: onemt
-- 创建时间: 2024-12-05 11:27:23
-- 概述:
---------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using TAABB;
using UnityEngine;

namespace TBVH
{
    public class BVHTree
    {
        private List<BVHNode> m_LeafNodes = new List<BVHNode>();

        private BVHNode m_RootNode;

        public void AddNode(GameObject gameObject)
        {
            BVHNode bvhNode = new BVHNode(gameObject.name, gameObject);
            AddNode(bvhNode);
        }
        
        private void AddNode(BVHNode addNode)
        {
            if (m_RootNode == null)
                m_RootNode = addNode;
            BVHNode targetNode = SAH(addNode);
        }

        private void InsertNodeRecursive(BVHNode insertNode)
        {
            
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
        
        private void RemoveNode(GameObject gameObject)
        {
            
        }

        public void RemoveNode(BVHNode node)
        {
            
        }
    }
}