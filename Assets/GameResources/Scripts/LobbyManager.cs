using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// Лобби менеджер
/// </summary>
public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private InputField nameInputField = null;

    private const string GAME_VERSION = "1";
    private const string NAME_SCENE_GAME = "Game";
    private const string NAME_SERVER = "Server";
    private const int MAX_PLAYER = 10;

    private void Awake()
    {
        Init();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log($"Connected to Master");
        EventManager.OnConnectedToServer();
    }

    /// <summary>
    /// Создаём сервер
    /// </summary>
    public void OnCreateRoom()
    {
        PhotonNetwork.NickName = NAME_SERVER;
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = MAX_PLAYER });
        Debug.Log($"OnCreateRoom");
        //GoToGame();
    }

    /// <summary>
    /// Подключаемся к серверу
    /// </summary>
    public void OnJoinRoom()
    {
        if (nameInputField.text != string.Empty)
        {
            PhotonNetwork.NickName = nameInputField.text;
            PhotonNetwork.JoinRandomRoom();
            Debug.Log($"OnJoinRoom");
        }
    }

    /// <summary>
    /// Колбэк подключения к комнате
    /// </summary>
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log($"OnJoinedRoom");
    }

    /// <summary>
    /// Перейти в сцену
    /// </summary>
    public void GoToGame()
    {
        PhotonNetwork.LoadLevel(NAME_SCENE_GAME);
    }

    private void Init()
    {
        //автоматически переключает сцены
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = GAME_VERSION;
        PhotonNetwork.ConnectUsingSettings();
    }
}