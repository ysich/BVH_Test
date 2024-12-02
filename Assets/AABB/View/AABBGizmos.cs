using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TAABB
{
    public class AABBGizmos:MonoBehaviour
    {
        public AABBNodeBehaviour aabbNode1;
        public AABBNodeBehaviour aabbNode2;

        public GameObject point;
        
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            AABB union = aabbNode1.aabb.Union(aabbNode2.aabb);
            //绘制并集
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(union.center,union.size);
            if (aabbNode1.aabb.Intersects(aabbNode2.aabb))
            {
                aabbNode1.SetGizmosColor(Color.red);
                aabbNode2.SetGizmosColor(Color.red);
            }
            else
            {
                aabbNode1.SetGizmosColor(Color.white);
                aabbNode2.SetGizmosColor(Color.white);
            }

            Vector3 pointPos = point.transform.position;
            if (aabbNode1.aabb.ContainPoint(pointPos) ||
                aabbNode2.aabb.ContainPoint(pointPos))
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(pointPos,1);
            }
        }
    }
}