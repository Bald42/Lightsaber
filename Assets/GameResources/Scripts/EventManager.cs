using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Обработчик событий
/// </summary>
public static class EventManager
{
    public static event Action OnConnectedToServerEvent = null;

    /// <summary>
    /// Вызываем событие подключения к серверу
    /// </summary>
    public static void OnConnectedToServer ()
    {
        OnConnectedToServerEvent?.Invoke();
    }
}