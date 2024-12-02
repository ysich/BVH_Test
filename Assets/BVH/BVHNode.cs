using TAABB;

namespace TBVH
{
    public class BVHNode
    {
        public BVHNode leftNode;
        public BVHNode rightNode;
        public BVHNode parentNode;

        public AABB aabb;
    }
}