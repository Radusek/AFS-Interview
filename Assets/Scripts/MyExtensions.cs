namespace AFSInterview
{
    using UnityEngine;
    
    public static class Utils
    {
        public static bool IsLayerInMask(this int thisLayer, int layerMask) => (1 << thisLayer & layerMask) != 0;
        public static readonly int GroundMask = LayerMask.GetMask("Ground");

        public static Vector3 ResetY(this Vector3 vector)
        {
            vector.y = 0;
            return vector;
        }
    }
}
