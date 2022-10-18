using System;
using CustomBuildSystem.Placed;
using CustomBuildSystem.Placing;
using Photon.Pun;
using UnityEngine;

namespace CustomBuildSystem.PhotonIntegration
{
    [RequireComponent(typeof(PhotonView))]
    public class BuildSystemPhotonHandler : MonoBehaviour
    {
        private LayerMask probsLayer;
        private BuildSystem _buildSystem;
        private PhotonView _photonView;

        #region Unity Callbacks
        void OnEnable()
        {
            _photonView = GetComponent<PhotonView>();
            BuildEvents.OnBuildSystemCreated += OnBuildSystemCreated;
            BuildEvents.OnCellStateChanged += OnCellStateChanged;
            BuildEvents.OnEdgeStateChanged += OnEdgeStateChanged;
            BuildEvents.OnItemDeleted += OnItemDeleted;
        }

        void OnDisable()
        {
            BuildEvents.OnBuildSystemCreated -= OnBuildSystemCreated;
            BuildEvents.OnCellStateChanged -= OnCellStateChanged;
            BuildEvents.OnEdgeStateChanged -= OnEdgeStateChanged;
            BuildEvents.OnItemDeleted -= OnItemDeleted;
        }
        #endregion

        // Change the implementation of Communicator Functions according to game need 
        #region Communicator Functions
        public static GameObject InitGameObject(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent, int layer)
        {
            GameObject go = Instantiate(gameObject, position, rotation, parent);
            go.SetLayerRecursive(layer);
            return go;
        }

        public static string GetPlotID(Vector3 position)
        {
            return "0";
        }

        private string GetNameFor(Vector3 position, string placement)
        {
            return $"{GetPlotID(position)}|{placement}";
        }
        #endregion

        #region Build System Event Callbacks
        private void OnBuildSystemCreated(BuildSystem system)
        {
            this._buildSystem = system;
        }

        private void OnEdgeStateChanged(BSS_PlacingEdge placingEdge, PlacingState newState)
        {
            if (_buildSystem == null || newState != PlacingState.Placed) return;
            Vector3 position = _buildSystem.gridCurrent.EdgeNumberToPosition(placingEdge.EdgeNumber);
            string placedItemID = GetNameFor(position, placingEdge.EdgeNumber.ToString());

            _photonView.RPC(nameof(RPC_OnItemSpawned), RpcTarget.Others,
                placingEdge.Current.ID, placedItemID, position, placingEdge.Rotation);
        }

        private void OnCellStateChanged(BSS_PlacingCell placingCell, PlacingState newState)
        {
            if (_buildSystem == null || newState != PlacingState.Placed) return;
            Vector3 position = _buildSystem.gridCurrent.CellNumberToPosition(placingCell.CellNumber);
            string placedItemID = GetNameFor(position, placingCell.CellNumber.ToString());

            _photonView.RPC(nameof(RPC_OnItemSpawned), RpcTarget.Others,
                placingCell.Current.ID, placedItemID, position, placingCell.Rotation);
        }

        private void OnItemDeleted(OccupantBaseMono deleting)
        {
            Type type = deleting.GetType();
            string itemID;
            if (type == typeof(CellDecorator)) itemID = GetNameFor(deleting.transform.position, ((CellDecorator)deleting).Number.ToString());
            else if (type == typeof(EdgeDecorator)) itemID = GetNameFor(deleting.transform.position, ((EdgeDecorator)deleting).Number.ToString());
            else if (type == typeof(CellOccupantMono)) itemID = GetNameFor(deleting.transform.position, ((CellOccupantMono)deleting).Number.ToString());
            else if (type == typeof(EdgeOccupantMono)) itemID = GetNameFor(deleting.transform.position, ((EdgeOccupantMono)deleting).Number.ToString());
            else throw new NotImplementedException($"The give type ({type}) is not implemented");

            _photonView.RPC(nameof(RPC_OnItemDeleted), RpcTarget.Others, itemID);
        }
        #endregion

        #region RPCs
        [PunRPC]
        public void RPC_OnItemSpawned(int placeableID, string itemID, Vector3 position, int rotation)
        {
            // Note: Do not use _buildSystem inside RPC (Because this is being executed on the receiver's end where build system might not be present or worst be completely different from sender's system)
            PlaceableMonoBase placeableSo = BuildSystem.AllPlaceableData[placeableID];
            Quaternion rotQuat = Quaternion.Euler(0, rotation, 0);
            GameObject spawned = InitGameObject(placeableSo.placed, position, rotQuat, transform, probsLayer);
            spawned.name = itemID;
        }

        [PunRPC]
        public void RPC_OnItemDeleted(string itemID)
        {
            // Note: Do not use _buildSystem inside RPC (Because this is being executed on the receiver's end where build system might not be present or worst be completely different from sender's system)
            Transform obj = transform.Find(itemID);
            if (obj != null)
            {
                Destroy(obj.gameObject);
            }
        }
        #endregion
    }
}