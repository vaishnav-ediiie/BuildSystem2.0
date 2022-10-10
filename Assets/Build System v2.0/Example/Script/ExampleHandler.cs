using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomBuildSystem.Example
{
    public class ExampleHandler : MonoBehaviour
    {
        [SerializeField] protected BuildSystem buildSystem;
        [SerializeField] protected Canvas canvas;
        [SerializeField] private KeyCode quitKeyCode;
        [SerializeField] protected SelectItemPopup selectionPopup;
        [SerializeField] internal PlaceableSOBase[] allPlaceable;

        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            ExampleBrain exampleBrain = new ExampleBrain
            {
                AllPlaceableData = new Dictionary<int, PlaceableSOBase>()
            };
            
            foreach (PlaceableSOBase soBase in allPlaceable)
            {
                if (exampleBrain.AllPlaceableData.ContainsKey(soBase.ID)) Debug.LogError($"Multiple placeable with same ID: {soBase.name} & {exampleBrain.AllPlaceableData[soBase.ID].name}");
                else exampleBrain.AllPlaceableData.Add(soBase.ID, soBase);
            }
            buildSystem.UseBrain(exampleBrain);
            
            buildSystem.Brain.OnCellStateChanged += OnCellStateChanged;
            buildSystem.Brain.OnEdgeStateChanged += OnEdgeStateChanged;
        }

        private void OnEdgeStateChanged(BSS_PlacingEdge placeable, PlacingState newState)
        {
            if (newState == PlacingState.Placed) buildSystem.StartBuild(placeable.PlaceableSo);
        }

        private void OnCellStateChanged(BSS_PlacingCell placeable, PlacingState newState)
        {
            if (newState == PlacingState.Placed) buildSystem.StartBuild(placeable.PlaceableSo);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // placingEdge = !placingEdge;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Instantiate(selectionPopup, canvas.transform)
                    .Init(allPlaceable, UpdatePlacingState);
            }

            if (Input.GetKey(KeyCode.Tab)) return;

            if (Input.GetKeyDown(quitKeyCode))
            {
                buildSystem.CancelBuild(switchToIdle: true);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                string data = buildSystem.Serialize();
                PlayerPrefs.SetString("___Data___", data);
                Debug.Log(data);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                buildSystem.Deserialize(PlayerPrefs.GetString("___Data___"));
            }
        }

        private void UpdatePlacingState(PlaceableSOBase placeable)
        {
            if (placeable != null)
            {
                Type pt = placeable.GetType();
                if (pt == typeof(EdgePlaceableSO))      buildSystem.StartBuild((EdgePlaceableSO)placeable);
                else if (pt == typeof(CellPlaceableSO)) buildSystem.StartBuild((CellPlaceableSO)placeable);
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}