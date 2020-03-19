using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

/// <summary>
/// Игровой менеджер
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks
{
    private const string NAME_SCENE_MENU = "Menu";

    private const string NAME_PREFAB_PLAYER = "Player";

    private void Start()
    {
        SpawnPlayer();
    }


    private void SpawnPlayer ()
    {
        PhotonNetwork.Instantiate(NAME_PREFAB_PLAYER, Vector3.zero, Quaternion.identity);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log($"Left room");
        SceneManager.LoadScene(NAME_SCENE_MENU);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log($"Player {newPlayer.NickName} entered room");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log($"Player {otherPlayer.NickName} left room");
    }

    /// <summary>
    /// Покинуть комнату
    /// </summary>
    public void OnLeave ()
    {
        PhotonNetwork.LeaveRoom();
    }
}