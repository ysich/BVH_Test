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

        public void BuildBVH(List<GameObject> sceneObjects, int depth,BVHBuildType bvhBuildType = BVHBuildType.BinaryPartition)
        {
            root = new BVHNode("root", null);
            foreach (var obj in sceneObjects)
            {
                AABB aabb = BVHNode.ComputeWordAABB(obj);
                root.UnionAABB(aabb);
            }

            if (bvhBuildType == BVHBuildType.BinaryPartition)
            {
                BinaryPartition(root,sceneObjects,0,sceneObjects.Count,depth);
            }
            else
            {
                AxisPartition(root, sceneObjects, depth);
            }
        }

        /// <summary>
        /// 轴划分，kd树思维。选定方差最大的轴进行划分
        /// </summary>
        /// <param name="node"></param>
        /// <param name="sceneObjects"></param>
        /// <param name="depth"></param>
        public void AxisPartition(BVHNode node,List<GameObject> sceneObjects,int depth)
        {
            if(depth <= 0) return;
            
            var leftNode = new BVHNode(node.name + "_left_" + depth, null);
            var rightNode = new BVHNode(node.name + "_right_" + depth, null);
            node.SetLeafNode(leftNode,rightNode);
            leftNode.parentNode = node;
            rightNode.parentNode = node;

            List<GameObject> leftNodeObjects = new List<GameObject>();
            List<GameObject> rightNodeObjects = new List<GameObject>();

            BVHAxisVarianceType bvhAxisVarianceType = PickAxisVariance(sceneObjects);
            switch (bvhAxisVarianceType)
            {
                case BVHAxisVarianceType.XAxis:
                    float midX = node.aabb.center.x;
                    foreach (var obj in sceneObjects)
                    {
                        if (obj.transform.position.x <= midX)
                        {
                            leftNodeObjects.Add(obj);
                            AABB aabb = BVHNode.ComputeWordAABB(obj);
                            leftNode.UnionAABB(aabb);
                        }
                        else
                        {
                            rightNodeObjects.Add(obj);
                            AABB aabb = BVHNode.ComputeWordAABB(obj);
                            rightNode.UnionAABB(aabb);
                        }
                    }
                    break;
                case BVHAxisVarianceType.YAxis:
                    float midY = node.aabb.center.y;
                    foreach (var obj in sceneObjects)
                    {
                        if (obj.transform.position.y <= midY)
                        {
                            leftNodeObjects.Add(obj);
                            AABB aabb = BVHNode.ComputeWordAABB(obj);
                            leftNode.UnionAABB(aabb);
                        }
                        else
                        {
                            rightNodeObjects.Add(obj);
                            AABB aabb = BVHNode.ComputeWordAABB(obj);
                            rightNode.UnionAABB(aabb);
                        }
                    }
                    break;
                case BVHAxisVarianceType.ZAxis:
                    float midZ = node.aabb.center.z;
                    foreach (var obj in sceneObjects)
                    {
                        if (obj.transform.position.z <= midZ)
                        {
                            leftNodeObjects.Add(obj);
                            AABB aabb = BVHNode.ComputeWordAABB(obj);
                            leftNode.UnionAABB(aabb);
                        }
                        else
                        {
                            rightNodeObjects.Add(obj);
                            AABB aabb = BVHNode.ComputeWordAABB(obj);
                            rightNode.UnionAABB(aabb);
                        }
                    }
                    break;
            }
            
            AxisPartition(leftNode,leftNodeObjects,depth - 1);
            AxisPartition(rightNode,rightNodeObjects,depth - 1);
        }

        private BVHAxisVarianceType PickAxisVariance(List<GameObject> objects)
        {
            float mean_x = 0f;
            float mean_y = 0f;
            float mean_z = 0f;

            foreach (var obj in objects)
            {
                Vector3 pos = obj.transform.position;
                mean_x += pos.x;
                mean_y += pos.y;
                mean_z += pos.z;
            }

            float objCount = objects.Count;
            mean_x /= objCount;
            mean_y /= objCount;
            mean_z /= objCount;

            float variance_x = 0f;
            float variance_y = 0f;
            float variance_z = 0f;
            
            //统计方差
            foreach (var obj in objects)
            {
                Vector3 pos = obj.transform.position;
                variance_x += Mathf.Pow(pos.x - mean_x, 2);
                variance_y += Mathf.Pow(pos.y - mean_y, 2);
                variance_z += Mathf.Pow(pos.z - mean_z, 2);
            }

            variance_x /= objCount;
            variance_y /= objCount;
            variance_z /= objCount;

            if (variance_x > variance_y && variance_x > variance_z)
            {
                return BVHAxisVarianceType.XAxis;
            }
            
            if (variance_y > variance_x && variance_y > variance_z)
            {
                return BVHAxisVarianceType.YAxis;
            }
            
            if (variance_z > variance_x && variance_z > variance_y)
            {
                return BVHAxisVarianceType.ZAxis;
            }

            return BVHAxisVarianceType.XAxis;
        }
        
        /// <summary>
        /// 二分查找法进行划分
        /// </summary>
        /// <param name="node"></param>
        /// <param name="objects"></param>
        /// <param name="startIndex"></param>
        /// <param name="depth"></param>
        private void BinaryPartition(BVHNode node,List<GameObject> objects,int startIndex,int endIndex,int depth)
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