using System;
using CustomGridSystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomBuildSystem
{
    public class BSS_Deleting : BuiltSystemState
    {
        private static readonly float MaxRaycastDistance = 500f;
        private IMonoPlaceable currentFocused;
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

            if (currentFocused != null && BuildSystem.Brain.ShouldDeleteObject(currentFocused))
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
                if (comp != null && comp != currentFocused)
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
            currentFocused = newOne;
            SetCurrentRendererActive(false);

            if (currentSpawned) Object.Destroy(currentSpawned);
            currentSpawned = Object.Instantiate(newOne.GetDeletePrefab());
            currentSpawned.transform.CopyFrom(currentFocused.transform);
        }

        void EmptyFocus()
        {
            SetCurrentRendererActive(true);
            currentFocused = null;
            if (currentSpawned) Object.Destroy(currentSpawned);
        }

        void SetCurrentRendererActive(bool value)
        {
            if (currentFocused == null) return;
            foreach (Renderer renderer in currentFocused.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = value;
            }
        }
        
        private void ConfirmDelete()
        {
            currentFocused.UnOccupy(BuildSystem);
            Object.Destroy(currentFocused.gameObject);
            Object.Destroy(currentSpawned);
        }
    }
}