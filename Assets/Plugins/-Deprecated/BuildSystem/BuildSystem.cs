using UnityEngine;
using KSRecs.Deprecated.GridSystemSpace;

namespace KSRecs.Deprecated.BuildSystemSpace
{
    public class BuildSystem : MonoBehaviour
    {
        public static BuildSystem Instance { get; private set; }
        [SerializeField] internal string probsLayerName;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private PlayerCompForBuildSystem player;
        private Vector3 mousePosCenter;
        
        private BuildProbSO selectedBuildProbSo;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                mousePosCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            }
            else
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void LateUpdate()
        {
            GridSystem.Instance.GridYPos = player.GetFloorHeight();
            
            if (selectedBuildProbSo == null)
                return;

            if (Input.GetKeyUp(KeyCode.Q))
            {
                selectedBuildProbSo.CancelBuild();
                this.DeselectProb();
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                selectedBuildProbSo.ConfirmBuild(player.GetFloorNumber());
                this.DeselectProb();
                return;
            }
            
            Vector3 lookAtPoint = GetPointPlayerIsLookingAt(playerCamera.ScreenPointToRay(mousePosCenter));
            CellNumber lookAtCellNum = GridSystem.ValidateCellNumber(GridSystem.Instance.GetCellNumber(lookAtPoint));
            CellNumber currentCellNum = GridSystem.Instance.GetCellNumber(player.transform.position);

            if (currentCellNum == lookAtCellNum)
                selectedBuildProbSo.MovePreviewTo(player.transform.position, lookAtPoint, player);
            else
                selectedBuildProbSo.MovePreviewTo(currentCellNum, lookAtCellNum, player);
        }

        private Vector3 GetPointPlayerIsLookingAt(Ray ray)
        {
            float h = player.GetFloorHeight();
            Vector3 orig = ray.origin;
            Vector3 dire = orig + ray.direction;
            float fact = (h - dire.y) / (dire.y - orig.y);
            float x = fact * (dire.x - orig.x);
            float z = fact * (dire.z - orig.z);
            return new Vector3(x + dire.x, h, z + dire.z);
        }

        public void StartProbSetup(BuildProbSO prob)
        {
            if (selectedBuildProbSo)
                selectedBuildProbSo.CancelBuild();

            prob.InitNew();
            selectedBuildProbSo = prob;
        }

        
        
        void DeselectProb()
        {
            selectedBuildProbSo = null;
        }

        public void SetupCall(LayerMask theProbsLayer, Camera thePlayerCamera, PlayerCompForBuildSystem thePlayerComp)
        {
            // this.probsLayer = theProbsLayer;
            this.playerCamera = thePlayerCamera;
            this.player = thePlayerComp;
            selectedBuildProbSo = null;
        }
        
        public void SetupCall(Camera thePlayerCamera, PlayerCompForBuildSystem thePlayerComp)
        {
            this.playerCamera = thePlayerCamera;
            this.player = thePlayerComp;
            selectedBuildProbSo = null;
        }
    }
}