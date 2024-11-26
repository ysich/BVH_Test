using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Octree
{
    public class OctreeManager : MonoBehaviour
    {
        [Tooltip("创建数量")] public int genCount;

        [Tooltip("创建尺寸")] public float genSize;

        [Tooltip("范围")] public float range;

        [Tooltip("八叉树深度")] public int treeDepth;


        private OctreeNode rootNode;
        private List<GameObject> sceneGameObjects;

        public void Start()
        {
            GenerateSceneGameObjects();
            OctreePartion();
        }

        private void GenerateSceneGameObjects()
        {
            sceneGameObjects = new List<GameObject>(genCount);
            float halfRange = range * 0.5f;
            for (int i = 0; i < genCount; i++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.SetParent(this.transform);
                cube.transform.position = new Vector3(Random.Range(-halfRange, halfRange),
                    Random.Range(-halfRange, halfRange),
                    Random.Range(-halfRange, halfRange));
                sceneGameObjects.Add(cube);
            }
        }

        /// <summary>
        /// 八叉树划分
        /// </summary>
        private void OctreePartion()
        {
            rootNode = new OctreeNode(Vector3.zero, range);
            rootNode.areaObjects = sceneGameObjects;
            GenerateOctree(rootNode, range, treeDepth);
        }

        private void GenerateOctree(OctreeNode rootNode, float range, float depth)
        {
            if (depth <= 0)
                return;
            float halfRange = range * 0.5f;
            float rootOffset = halfRange * 0.5f;
            Vector3 rootCenter = rootNode.center;

            rootNode.top1 = new OctreeNode(rootCenter + new Vector3(-1, 1, -1) * rootOffset, halfRange);
            rootNode.top2 = new OctreeNode(rootCenter + new Vector3(-1, 1, 1) * rootOffset, halfRange);
            rootNode.top3 = new OctreeNode(rootCenter + new Vector3(1, 1, -1) * rootOffset, halfRange);
            rootNode.top4 = new OctreeNode(rootCenter + new Vector3(1, 1, 1) * rootOffset, halfRange);
            rootNode.bottom1 = new OctreeNode(rootCenter + new Vector3(-1, -1, -1) * rootOffset, halfRange);
            rootNode.bottom2 = new OctreeNode(rootCenter + new Vector3(-1, -1, 1) * rootOffset, halfRange);
            rootNode.bottom3 = new OctreeNode(rootCenter + new Vector3(1, -1, -1) * rootOffset, halfRange);
            rootNode.bottom4 = new OctreeNode(rootCenter + new Vector3(1, -1, 1) * rootOffset, halfRange);

            PartionGameObjects(rootNode, rootNode.areaObjects);
            //一个节点超过两个对象则继续划分节点
            if (rootNode.top1.areaObjectCount >= 2)
            {
                GenerateOctree(rootNode.top1, halfRange, depth - 1);
            }
            if (rootNode.top2.areaObjectCount >= 2)
            {
                GenerateOctree(rootNode.top2, halfRange, depth - 1);
            }
            if (rootNode.top3.areaObjectCount >= 2)
            {
                GenerateOctree(rootNode.top3, halfRange, depth - 1);
            }
            if (rootNode.top4.areaObjectCount >= 2)
            {
                GenerateOctree(rootNode.top4, halfRange, depth - 1);
            }
            if (rootNode.bottom1.areaObjectCount >= 2)
            {
                GenerateOctree(rootNode.bottom1, halfRange, depth - 1);
            }
            if (rootNode.bottom2.areaObjectCount >= 2)
            {
                GenerateOctree(rootNode.bottom2, halfRange, depth - 1);
            }
            if (rootNode.bottom3.areaObjectCount >= 2)
            {
                GenerateOctree(rootNode.bottom3, halfRange, depth - 1);
            }
            if (rootNode.bottom4.areaObjectCount >= 2)
            {
                GenerateOctree(rootNode.bottom4, halfRange, depth - 1);
            }
            
        }

        private void PartionGameObjects(OctreeNode octreeNode,List<GameObject> gameObjects)
        {
            foreach (var go in gameObjects)
            {
                Vector3 pos = go.transform.position;
                if(octreeNode.top1.Contains(pos))
                    octreeNode.top1.AddAreaGameObject(go);
                if(octreeNode.top2.Contains(pos))
                    octreeNode.top2.AddAreaGameObject(go);
                if(octreeNode.top3.Contains(pos))
                    octreeNode.top3.AddAreaGameObject(go);
                if(octreeNode.top4.Contains(pos))
                    octreeNode.top4.AddAreaGameObject(go);
                
                if(octreeNode.bottom1.Contains(pos))
                    octreeNode.bottom1.AddAreaGameObject(go);
                if(octreeNode.bottom2.Contains(pos))
                    octreeNode.bottom2.AddAreaGameObject(go);
                if(octreeNode.bottom3.Contains(pos))
                    octreeNode.bottom3.AddAreaGameObject(go);
                if(octreeNode.bottom4.Contains(pos))
                    octreeNode.bottom4.AddAreaGameObject(go);
            }
        }

        private void OnDrawGizmos()
        {
            if(rootNode == null)
                return;
            rootNode.DrawGizmos();
        }
    }
}