using UnityEngine;
using Photon.Pun;
using Loader;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        ScenesLoader.LoadScene("Lobby");
    }
}