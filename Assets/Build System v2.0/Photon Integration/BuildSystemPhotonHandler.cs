using Photon.Pun;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomBuildSystem.PhotonIntegration
{
    [RequireComponent(typeof(PhotonView))]
    public class BuildSystemPhotonHandler : MonoBehaviour
    {
        [SerializeField] private BuildSystem buildSystem;

        private PhotonView photonView;

        void OnEnable()
        {
            photonView = GetComponent<PhotonView>();
            buildSystem.Brain.OnCellStateChanged += OnCellStateChanged;
            buildSystem.Brain.OnEdgeStateChanged += OnEdgeStateChanged;
        }

        void OnDisable()
        {
            buildSystem.Brain.OnCellStateChanged -= OnCellStateChanged;
            buildSystem.Brain.OnEdgeStateChanged -= OnEdgeStateChanged;
            buildSystem.Brain.OnItemDeleted -= OnItemDeleted;
        }

        private string GetNameFor(int placeableID, int actorNumber) => $"{actorNumber}|{placeableID}";

        private void OnEdgeStateChanged(BSS_PlacingEdge placingEdge, PlacingState newState)
        {
            if (newState != PlacingState.Placed) return;
            Vector3 position = buildSystem.gridCurrent.EdgeNumberToPosition(placingEdge.EdgeNumber);
            Debug.Log($"Photon: {photonView}");
            photonView.RPC(nameof(RPC_OnItemSpawned), RpcTarget.OthersBuffered,
                placingEdge.PlaceableSo.ID, TempClass.GetActivePlotID(), position, placingEdge.Rotation);
        }

        private void OnCellStateChanged(BSS_PlacingCell placingCell, PlacingState newState)
        {
            if (newState != PlacingState.Placed) return;
            Vector3 position = buildSystem.gridCurrent.CellNumberToPosition(placingCell.CellNumber);
            Debug.Log($"Photon: {photonView}");
            photonView.RPC(nameof(RPC_OnItemSpawned), RpcTarget.OthersBuffered,
                placingCell.PlaceableSo.ID, TempClass.GetActivePlotID(), position, placingCell.Rotation);
        }

        private void OnItemDeleted(BSS_Deleting deleting)
        {
            photonView.RPC(nameof(RPC_OnItemDeleted), RpcTarget.OthersBuffered,
                deleting.Target.GetScriptableID(), TempClass.GetActivePlotID());
        }


        [PunRPC]
        public void RPC_OnItemSpawned(int placeableID, int actorNumber, Vector3 position, int rotation)
        {
            PlaceableSOBase placeableSo = buildSystem.Brain.AllPlaceableData[placeableID];
            Quaternion rotQuat = Quaternion.Euler(0, rotation, 0);
            GameObject spawned = TempClass.InitGameObject(placeableSo.placed, position, rotQuat, transform, buildSystem.ProbsLayer);
            spawned.name = GetNameFor(actorNumber, placeableID);
        }

        [PunRPC]
        public void RPC_OnItemDeleted(int placeableID, int actorNumber)
        {
            Transform obj = transform.Find(GetNameFor(placeableID, actorNumber));
            if (obj != null)
            {
                Destroy(obj.gameObject);
            }
        }
    }


    public static class TempClass
    {
        public static GameObject InitGameObject(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent, int layer)
        {
            GameObject go = Object.Instantiate(gameObject, position, rotation, parent);
            go.SetLayerRecursive(layer);
            return go;
        }
        
        public static int GetActivePlotID()
        {
            return 0;
        } 
    }
}