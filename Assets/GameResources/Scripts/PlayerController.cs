using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Управление игроком
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private PhotonView photonView = null;

    private void Update()
    {
        KeyboardControl();
    }


    private void KeyboardControl ()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(-Time.deltaTime * 5, 0, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Time.deltaTime * 5, 0, 0);
        }
    }
}