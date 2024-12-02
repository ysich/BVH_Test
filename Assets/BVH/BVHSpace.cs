/*---------------------------------------------------------------------------------------
-- 负责人: onemt
-- 创建时间: 2024-12-02 10:18:00
-- 概述:
---------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using TAABB;
using UnityEngine;

namespace TBVH
{
    public class BVHSpace
    {
        public BVHNode root;

        public void BuildBVH(List<GameObject> sceneObjects, int depth)
        {
            root = new BVHNode("root", null);
            foreach (var obj in sceneObjects)
            {
                AABB aabb = BVHNode.ComputeWordAABB(obj);
                root.UnionAABB(aabb);
            }
            BinaryPartition(root,sceneObjects,0,sceneObjects.Count,depth);
        }
        /// <summary>
        /// 二分查找法进行划分
        /// </summary>
        /// <param name="node"></param>
        /// <param name="objects"></param>
        /// <param name="startIndex"></param>
        /// <param name="depth"></param>
        public void BinaryPartition(BVHNode node,List<GameObject> objects,int startIndex,int endIndex,int depth)
        {
            if(depth <= 0) return;
            
            //计算二分下表
            int halfIndex = (endIndex + startIndex) / 2;
            var leftNode = new BVHNode(node.name + "_left_" + depth.ToString(), null);
            
            //计算左节点AABB
            for (int i = startIndex; i < halfIndex; i++)
            {
                var obj = objects[i];
                var aabb = BVHNode.ComputeWordAABB(obj);
                leftNode.UnionAABB(aabb);
            }
            
            var rightNode = new BVHNode(node.name + "_right_" + depth.ToString(), null);
            //计算右节点AABB
            for (int i = halfIndex; i < endIndex; i++)
            {
                var obj = objects[i];
                var aabb = BVHNode.ComputeWordAABB(obj);
                rightNode.UnionAABB(aabb);
            }
            
            node.SetLeafNode(leftNode,rightNode);
            
            BinaryPartition(leftNode,objects,startIndex,halfIndex,depth - 1);
            BinaryPartition(rightNode,objects,halfIndex,endIndex,depth - 1);
        }
    }
}