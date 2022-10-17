using CustomBuildSystem.Placed;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomBuildSystem
{
    public class BSS_Deleting : BuiltSystemState
    {
        public OccupantBaseMono Target { get; private set; }
        private static readonly float MaxRaycastDistance = 500f;
        private GameObject currentSpawned;
        private Ray currentRay;

        public override void OnUpdate()
        {
            Ray ray = BuildSystem.playerCamera.ScreenPointToRay(BuildSystem.Brain.GetMousePosition);
            if (ray.origin != currentRay.origin || ray.direction != currentRay.direction)
            {
                RedoRaycast(ray);
            }

            if (Target != null && BuildSystem.Brain.ShouldDeleteObject(Target))
            {
                ConfirmDelete();
            }
        }

        public override void OnExit()
        {
            EmptyFocus();
        }

        void RedoRaycast(Ray ray)
        {
            if (Physics.Raycast(
                    ray: ray,
                    hitInfo: out RaycastHit hitInfo,
                    maxDistance: MaxRaycastDistance,
                    layerMask: BuildSystem.ProbsLayer))
            {
                OccupantBaseMono comp = hitInfo.collider.GetComponentInParent<OccupantBaseMono>();
                if (comp != null && comp != Target && comp.FloorNumber == BuildSystem.CurrentFloor)
                {
                    SwitchActive(comp);
                }
            }
            else
            {
                EmptyFocus();
            }
        }

        void SwitchActive(OccupantBaseMono newOne)
        {
            SetCurrentRendererActive(true);
            Target = newOne;
            SetCurrentRendererActive(false);

            if (currentSpawned) Object.Destroy(currentSpawned);
            currentSpawned = Object.Instantiate(newOne.GetDeletePrefab());
            currentSpawned.transform.CopyFrom(Target.transform);
        }

        void EmptyFocus()
        {
            SetCurrentRendererActive(true);
            Target = null;
            if (currentSpawned) Object.Destroy(currentSpawned);
        }

        void SetCurrentRendererActive(bool value)
        {
            if (Target == null) return;
            foreach (Renderer renderer in Target.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = value;
            }
        }
        
        private void ConfirmDelete()
        {
            foreach (OccupantBaseMono monoPlaceable in Target.Children)
            {
                BuildEvents.Call_OnItemDeleted(monoPlaceable);
            }
            
            BuildEvents.Call_OnItemDeleted(Target);
            Target.UnOccupy(BuildSystem);
            Object.Destroy(Target.gameObject);
            Object.Destroy(currentSpawned);
        }
    }
}