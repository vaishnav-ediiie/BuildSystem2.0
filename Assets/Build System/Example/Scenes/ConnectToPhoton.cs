using UnityEngine;
using Photon.Pun;

namespace CustomBuildSystem
{
    public class ConnectToPhoton : MonoBehaviourPunCallbacks
    {
        public void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected");
            PhotonNetwork.LoadLevel(1);
        }
    }
}