
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Octree
{
    public class OctreeGizmos:MonoBehaviour
    {
        public OctreeManager octreeManager;
        public List<Color> gizmosColors = new List<Color>();

        public GameObject sphere;
        public Vector3 lastSpherePos;
        private void Start()
        {
            octreeManager = this.GetComponent<OctreeManager>();
            
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.SetParent(this.transform);
            Vector3 pos = Vector3.zero;
            sphere.transform.position = pos;
            lastSpherePos = pos;
            
            octreeManager.rootNode.AddAreaGameObject(sphere);
            octreeManager.GenerateOctree(octreeManager.rootNode, octreeManager.range, octreeManager.treeDepth);
        }

        private void LateUpdate()
        {
            Vector3 spherePos = sphere.transform.position;
            if (spherePos != lastSpherePos)
            {
                Debug.Log("change");
                lastSpherePos = spherePos;
                octreeManager.GenerateOctree(octreeManager.rootNode, octreeManager.range, octreeManager.treeDepth);
            }
        }

        private void OnDrawGizmos()
        {
            if(octreeManager == null || octreeManager.rootNode == null)
                return;
            DrawOctreeDepthGizmos(octreeManager.rootNode, octreeManager.treeDepth);
        }

        private void SetGizmosColorByDepth(int depth)
        {
            Color color;
            if (depth >= gizmosColors.Count || depth < 0)
            {
                color = Color.gray;
            }
            else
            {
                color = gizmosColors[depth];
            }
            color.a = 0.2f;
            Gizmos.color = color;
        }

        private void DrawOctreeDepthGizmos(OctreeNode node,int depth)
        {
            if (depth <= 0)
            {
                DrawOctreeNode(node,0);
                return;
            }
            DrawOctreeNode(node,depth);
            
            if(node.top1 != null)
                DrawOctreeDepthGizmos(node.top1,depth -1);
            if(node.top2 != null)
                DrawOctreeDepthGizmos(node.top2,depth -1);
            if(node.top3 != null)
                DrawOctreeDepthGizmos(node.top3,depth -1);
            if(node.top4 != null)
                DrawOctreeDepthGizmos(node.top4,depth -1);
            
            if(node.bottom1 != null)
                DrawOctreeDepthGizmos(node.bottom1,depth -1);
            if(node.bottom2 != null)
                DrawOctreeDepthGizmos(node.bottom2,depth -1);
            if(node.bottom3 != null)
                DrawOctreeDepthGizmos(node.bottom3,depth -1);
            if(node.bottom4 != null)
                DrawOctreeDepthGizmos(node.bottom4,depth -1);
        }

        private void DrawOctreeNode(OctreeNode node,int depth)
        {
            SetGizmosColorByDepth(depth);
            Gizmos.DrawWireCube(node.center,Vector3.one * node.size);
        }
    }
}