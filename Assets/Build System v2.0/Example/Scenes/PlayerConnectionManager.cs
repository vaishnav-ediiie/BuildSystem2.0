using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerConnectionManager : MonoBehaviourPunCallbacks
{
    [Tooltip("The maximum number of players that can join a single room.")] [SerializeField]
    protected int m_MaxPlayerCount = 30;

    
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