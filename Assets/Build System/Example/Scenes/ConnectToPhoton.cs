using UnityEngine;
using Photon.Pun;

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