using System;
using Photon.Pun;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomBuildSystem.PhotonIntegration
{
    [RequireComponent(typeof(PhotonView))]
    public class BuildSystemPhotonHandler : MonoBehaviour
    {
        internal BuildSystem buildSystem;
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

        private string GetNameFor(int plotID, string modifier) => $"{plotID}|{modifier}";

        private void OnEdgeStateChanged(BSS_PlacingEdge placingEdge, PlacingState newState)
        {
            if (newState != PlacingState.Placed) return;
            Vector3 position = buildSystem.gridCurrent.EdgeNumberToPosition(placingEdge.EdgeNumber);
            string itemID = GetNameFor(TempClass.GetActivePlotID(), placingEdge.EdgeNumber.ToString());

            photonView.RPC(nameof(RPC_OnItemSpawned), RpcTarget.OthersBuffered,
                placingEdge.Scriptable.ID, itemID, position, placingEdge.Rotation);
        }

        private void OnCellStateChanged(BSS_PlacingCell placingCell, PlacingState newState)
        {
            if (newState != PlacingState.Placed) return;
            Vector3 position = buildSystem.gridCurrent.CellNumberToPosition(placingCell.CellNumber);
            string itemID = GetNameFor(TempClass.GetActivePlotID(), placingCell.CellNumber.ToString());

            photonView.RPC(nameof(RPC_OnItemSpawned), RpcTarget.OthersBuffered,
                placingCell.Scriptable.ID, itemID, position, placingCell.Rotation);
        }

        private void OnItemDeleted(IMonoPlaceable deleting)
        {
            Type type = deleting.GetType();
            string itemID;
            if      (type == typeof(CellDecorator)) itemID = GetNameFor(TempClass.GetActivePlotID(), ((CellDecorator)deleting).Number.ToString());
            else if (type == typeof(EdgeDecorator)) itemID = GetNameFor(TempClass.GetActivePlotID(), ((EdgeDecorator)deleting).Number.ToString());
            else if (type == typeof(CellPlaceable)) itemID = GetNameFor(TempClass.GetActivePlotID(), ((CellPlaceable)deleting).Number.ToString());
            else if (type == typeof(EdgePlaceable)) itemID = GetNameFor(TempClass.GetActivePlotID(), ((EdgePlaceable)deleting).Number.ToString());
            else throw new NotImplementedException("The give type is not implemented");
            
            photonView.RPC(nameof(RPC_OnItemDeleted), RpcTarget.OthersBuffered, itemID);
        }


        [PunRPC]
        public void RPC_OnItemSpawned(int placeableID, string itemID, Vector3 position, int rotation)
        {
            PlaceableSOBase placeableSo = buildSystem.Brain.AllPlaceableData[placeableID];
            Quaternion rotQuat = Quaternion.Euler(0, rotation, 0);
            GameObject spawned = TempClass.InitGameObject(placeableSo.placed, position, rotQuat, transform, buildSystem.ProbsLayer);
            spawned.name = itemID;
        }

        [PunRPC]
        public void RPC_OnItemDeleted(string itemID)
        {
            Transform obj = transform.Find(itemID);
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