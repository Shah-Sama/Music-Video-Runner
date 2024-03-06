using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public PlayerMove playerMove;

    void Update()
    {
        transform.Translate(Vector3.forward * playerMove.moveSpeed * Time.deltaTime, Space.World);
    }
}
