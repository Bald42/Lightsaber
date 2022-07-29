using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text logText = null;

    private void Awake()
    {
        InitLobby();
    }

    private void InitLobby()
    {
        logText.text = string.Empty;
        PhotonNetwork.NickName = "Player_" + Random.Range(0, 1001);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();

        Log($"Player`s name is {PhotonNetwork.NickName}");
    }

    public override void OnConnectedToMaster()
    {
        Log($"On connected to master");
    }

    public void OnCreateRoom()
    {
        PhotonNetwork.CreateRoom(string.Empty, new Photon.Realtime.RoomOptions());
    }

    public void OnJoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Log($"Joined the room");
        PhotonNetwork.LoadLevel("GameScene");
    }

    private void Log(string message)
    {
        Debug.Log(message);
        logText.text += $"{message}\n";
    }
}