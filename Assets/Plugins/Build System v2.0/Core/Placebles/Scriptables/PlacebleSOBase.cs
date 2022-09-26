using UnityEngine;

namespace CustomBuildSystem
{
    public abstract class PlacebleSOBase : ScriptableObject
    {
        [SerializeField] GameObject placingOkay;
        [SerializeField] GameObject placingError;

        protected GameObject currentSpawned;
        public bool canPlace { get; protected set; }

        protected virtual T ReplaceActiveMovel<T>(T toPlace, bool nullCurrentSpawned)
            where T: Component
        {
            Transform cpTrans = currentSpawned.transform;
            T spawned = Instantiate(toPlace, cpTrans.position, cpTrans.rotation, cpTrans.parent);
            spawned.transform.localScale = cpTrans.localScale;
            if (!nullCurrentSpawned)
            {
                currentSpawned = spawned.gameObject;
            }
            Destroy(currentSpawned);
            return spawned;
        }

        public virtual void MarkOkay()
        {
            ReplaceActiveMovel(placingOkay.transform, nullCurrentSpawned: false);
            canPlace = true;
        }

        public virtual void MarkError()
        {
            ReplaceActiveMovel(placingError.transform, nullCurrentSpawned: false);
            canPlace = false;
        }

        public abstract IPlaceble Place();
    }
}