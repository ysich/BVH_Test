using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TBVH
{
    public class TutorialDynamicBVH:MonoBehaviour
    {
        private DynamicBVHSpace m_BvhSpace = new DynamicBVHSpace();
        private List<GameObject> m_SeneObjects = new List<GameObject>();

        [Range(0, 50)]
        public int displayDepth;

        public GameObject deleteObj;
        
        public enum GenerateType
        {
            Ordered,
            Random
        }
        
        public GenerateType generateType;

        public int generateCount;
        
        private void Awake()
        {
            var halfCount = generateCount * 0.5f;
            for (int i = 0; i < generateCount; i++)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.SetParent(transform);
                var positionX = i - halfCount;
                var randomPos = Random.insideUnitSphere * 10;
                if (generateType == GenerateType.Ordered)
                {
                    go.transform.position = new Vector3(positionX, 0, 0);
                }
                else
                {
                    go.transform.position = randomPos;
                }
                go.name = "Sphere_" + i.ToString();

                m_SeneObjects.Add(go);
                m_BvhSpace.AddNode(go);
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.SetParent(transform);
                var randomPos = Random.insideUnitSphere * 10;
                go.transform.position = randomPos;
                
                go.name = "Sphere_" + m_SeneObjects.Count;
                m_SeneObjects.Add(go);
                m_BvhSpace.AddNode(go);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (deleteObj != null)
                {
                    m_BvhSpace.RemoveNode(deleteObj);
                    m_SeneObjects.Remove(deleteObj);
                    DestroyImmediate(deleteObj);
                }   
            }
        }

        private void OnDrawGizmos()
        {
            m_BvhSpace.rootNode?.DrawDepth(1000);
        }
    }
}