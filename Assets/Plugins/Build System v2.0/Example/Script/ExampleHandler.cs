using UnityEngine;

namespace CustomBuildSystem.Example
{
    public class ExampleHandler : MonoBehaviour
    {
        [SerializeField] protected BuildSystem buildSystem;
        [SerializeField] internal EdgePlaceableSO edgePlaceable0;
        [SerializeField] internal EdgePlaceableSO edgePlaceable1;
        [SerializeField] internal EdgePlaceableSO edgePlaceable2;
        [SerializeField] internal CellPlaceableSO cell1x1;
        [SerializeField] internal CellPlaceableSO cell2x1_00;
        [SerializeField] internal CellPlaceableSO cell2x1_10;
        [SerializeField] internal CellPlaceableSO cell3x3_11;

        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                buildSystem.StartBuild(cell1x1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                buildSystem.StartBuild(cell2x1_00);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                 buildSystem.StartBuild(cell2x1_10);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            {
                buildSystem.StartBuild(cell3x3_11);
            }

            
            
            /*if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
            {
                buildSystem.StartBuild(edgePlaceable0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
            {
                buildSystem.StartBuild(edgePlaceable1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
            {
                buildSystem.StartBuild(edgePlaceable2);
            }*/
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                buildSystem.CancelBuild();
            }
        }
    }
}