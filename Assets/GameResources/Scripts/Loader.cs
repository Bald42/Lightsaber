using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Заглушка загрузки или ожидания
/// </summary>
public class Loader : MonoBehaviour
{
    public static Loader Instance = null;

    [SerializeField]
    private GameObject loaderBack = null;

    #region Subscribes / UnSubscribes
    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        UnSubscribe();
    }

    /// <summary>Подписки</summary>
    private void Subscribe()
    {
        EventManager.OnConnectedToServerEvent += OnConnectedToServer;
    }

    /// <summary>Отписки</summary>
    private void UnSubscribe()
    {
        EventManager.OnConnectedToServerEvent -= OnConnectedToServer;
    }

    private void OnConnectedToServer ()
    {
        Active(false);
    }
    #endregion

    private void Awake()
    {
        Init();
    }

    private void Init ()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Active(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Active (bool isActive)
    {
        loaderBack.SetActive(isActive);
    }
}