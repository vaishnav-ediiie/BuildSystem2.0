using UnityEngine;

namespace CustomBuildSystem
{
    public static class Extensions
    {
        public static bool IsSingleLayer(this LayerMask layerMask)
        {
            int lm = layerMask.value;
            return (lm != 0) && ((lm & (lm - 1)) == 0);
        }

        public static int GetLayer(this LayerMask layerMask)
        {
            int layerNumber = 0;
            int layer = layerMask.value;
            while (layer > 0)
            {
                layer >>= 1;
                layerNumber++;
            }

            return layerNumber - 1;
        }

        public static void SetLayerRecursive(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform go in gameObject.transform)
            {
                SetLayerRecursive(go.gameObject, layer);
            }
        }


        public static void CopyFrom(this Transform getter, Transform source)
        {
            getter.position = source.position;
            getter.rotation = source.rotation;
            getter.localScale = source.localScale;
        }

        public static void CopyTo(this Transform source, Transform getter)
        {
            getter.position = source.position;
            getter.rotation = source.rotation;
            getter.localScale = source.localScale;
        }


        public static void CopyFrom(this Transform getter, Transform source, bool pos = true, bool rot = true, bool sca = true)
        {
            if (pos) getter.position = source.position;
            if (rot) getter.rotation = source.rotation;
            if (sca) getter.localScale = source.localScale;
        }

        public static void CopyTo(this Transform source, Transform getter, bool pos = true, bool rot = true, bool sca = true)
        {
            if (pos) getter.position = source.position;
            if (rot) getter.rotation = source.rotation;
            if (sca) getter.localScale = source.localScale;
        }
    }
}