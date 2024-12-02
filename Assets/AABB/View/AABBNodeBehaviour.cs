using System;
using UnityEngine;

namespace TAABB
{
    public class AABBNodeBehaviour:MonoBehaviour
    {
        public AABB aabb = new AABB(Vector3.zero,Vector3.zero);

        public Vector3 minCorner;
        public Vector3 maxCorner;

        private Vector3 m_LastPos;
        
        private Color m_GizmosColor = Color.white;
        public void Awake()
        {
            m_LastPos = transform.position;
            aabb = new AABB(m_LastPos + minCorner,m_LastPos + maxCorner);
        }

        private void LateUpdate()
        {
            Vector3 pos = transform.position;
            if (m_LastPos != pos)
            {
                m_LastPos = pos;
                aabb.Reset(m_LastPos + minCorner, m_LastPos + maxCorner);
            }
        }
        
        public void SetGizmosColor(Color color)
        {
            m_GizmosColor = color;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = m_GizmosColor;
            Gizmos.DrawWireCube(aabb.center,aabb.size);
        }
    }
}