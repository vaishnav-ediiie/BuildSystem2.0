using System.Collections.Generic;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public abstract class PlaceableSOBase : ScriptableObject
    {
        [SerializeField] GameObject placingOkay;
        [SerializeField] GameObject placingError;

        protected GameObject currentSpawned;
        protected BuildSystem buildSystem;
        public bool canPlace { get; protected set; }

        protected virtual T ReplaceActiveMovel<T>(T toPlace, bool nullCurrentSpawned)
            where T : Component
        {
            Transform cpTrans = currentSpawned.transform;
            T spawned = Instantiate(toPlace, cpTrans.position, cpTrans.rotation, cpTrans.parent);
            spawned.transform.localScale = cpTrans.localScale;
            Destroy(currentSpawned);
            if (!nullCurrentSpawned) currentSpawned = spawned.gameObject;
            return spawned;
        }

        protected virtual void MarkOkay()
        {
            ReplaceActiveMovel(placingOkay.transform, nullCurrentSpawned: false);
            canPlace = true;
        }

        protected virtual void MarkError()
        {
            ReplaceActiveMovel(placingError.transform, nullCurrentSpawned: false);
            canPlace = false;
        }

        public void Mark(bool cp)
        {
            if (cp == canPlace) return;
            
            if (cp) MarkOkay();
            else MarkError();
        }
        
        public void InitPlacing(BuildSystem system)
        {
            this.buildSystem = system;
            this.currentSpawned = Instantiate(placingError);
            canPlace = false;
        }

        public void MoveTo(Vector3 position, IGridNumber gridNumber)
        {
            currentSpawned.transform.position = position;
        }

        public void RotateTo(float xAngle, float yAngle, float zAngle)
        {
            currentSpawned.transform.rotation = Quaternion.Euler(xAngle, yAngle, zAngle );
        }
        
        public void RotateBy(float xAngle, float yAngle, float zAngle)
        {
            currentSpawned.transform.Rotate(xAngle, yAngle, zAngle);
        }
        
        public void EndPlacing()
        {
            this.buildSystem = null;
            if (currentSpawned) Destroy(this.currentSpawned.gameObject);
            canPlace = false;
        }

        public abstract IPlaceable Place();
    }
}