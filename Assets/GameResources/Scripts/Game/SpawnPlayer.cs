using UnityEngine;
using Photon.Pun;

public class SpawnPlayer : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;
    [SerializeField] private Transform parent = null;

    private void Awake()
    {
        Spawn();
    }

    private void Spawn()
    {
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
        player.transform.SetParent(parent);
    }
}