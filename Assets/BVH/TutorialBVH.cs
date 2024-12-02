using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TBVH
{
    public class TutorialBVH:MonoBehaviour
    {
        private BVHSpace m_BvhSpace;
        
        [Range(1, 100)]
        public int generateCount = 10;
        private List<GameObject> m_SeneObjects;

        [Range(0, 10)]
        public int partionDepth = 4;
        [Range(0, 10)]
        public int displayDepth;

        public enum GenerateType
        {
            Ordered,
            Random
        }

        public GenerateType generateType;

        public BVHBuildType buildType;

        private void Awake()
        {
            m_SeneObjects = new List<GameObject>();
            m_BvhSpace = new BVHSpace();
            CreateScene();
        }

        private void CreateScene()
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
                    go.transform.position = new Vector3(positionX, randomPos.y, randomPos.z);
                }
                else
                {
                    go.transform.position = randomPos;
                }
                go.name = "Sphere_" + i.ToString();

                m_SeneObjects.Add(go);
            }

            m_BvhSpace.BuildBVH(m_SeneObjects, partionDepth,buildType);
        }

        private void OnDrawGizmos()
        {
            m_BvhSpace?.root?.DrawTargetDepth(displayDepth);
        }
    }
}