using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomBuildSystem
{
    public class BSS_Deleting : BuiltSystemState
    {
        public IMonoPlaceable Target { get; private set; }
        private static readonly float MaxRaycastDistance = 500f;
        private GameObject currentSpawned;
        private Ray currentRay;
        private Vector2 center;

        public override void OnEnter()
        {
            // buildSystem.DeleteIcon
            center = new Vector2(Screen.width, Screen.height) / 2f;
            Debug.Log(center);
        }

        public override void OnUpdate()
        {
            Ray ray = BuildSystem.playerCamera.ScreenPointToRay(center);
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
                IMonoPlaceable comp = hitInfo.collider.GetComponentInParent<IMonoPlaceable>();
                if (comp != null && comp != Target)
                {
                    SwitchActive(comp);
                }
            }
            else
            {
                EmptyFocus();
            }
        }

        void SwitchActive(IMonoPlaceable newOne)
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
            foreach (IMonoPlaceable monoPlaceable in Target.Children)
            {
                BuildSystem.Brain.Call_OnItemDeleted(monoPlaceable);
            }
            
            BuildSystem.Brain.Call_OnItemDeleted(Target);
            Target.UnOccupy(BuildSystem);
            Object.Destroy(Target.gameObject);
            Object.Destroy(currentSpawned);
        }
    }
}