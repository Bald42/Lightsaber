using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour, IPunObservable
{
    [SerializeField] private float speed = 50f;
    private PhotonView photonView = null;

    private bool IsMine
    {
        get
        {
            return photonView == null ? false : photonView.IsMine;
        }
    }
    private void Awake()
    {
        Cache();
    }

    private void Cache()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (!IsMine)
        {
            return;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-Time.deltaTime * speed, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Time.deltaTime * speed, 0, 0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, 0, Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(0, 0, -Time.deltaTime * speed);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}