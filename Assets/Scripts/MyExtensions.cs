namespace AFSInterview
{
    using UnityEngine;
    
    public static class Utils
    {
        public static bool IsLayerInMask(this int thisLayer, int layerMask) => (1 << thisLayer & layerMask) != 0;
        public static int GroundMask => LayerMask.GetMask("Ground");
    }
}
