using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Вращение
/// </summary>
public class Rotate : MonoBehaviour
{
    [SerializeField]
    private Vector3 angleRotate = Vector3.zero;

    [SerializeField]
    private float speed = 0f;

    private void Update()
    {
        DoRotate();
    }

    private void DoRotate ()
    {
        transform.Rotate(angleRotate * speed);
    }
}