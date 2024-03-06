using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 3;
    public float leftRightSpeed = 4;

    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float deltaX = touch.deltaPosition.x / Screen.width;

                if (deltaX > 0 && transform.position.x < LevelBoundry.rightSide)
                {
                    transform.Translate(Vector3.right * deltaX * leftRightSpeed);
                }
                else if (deltaX < 0 && transform.position.x > LevelBoundry.leftSide)
                {
                    transform.Translate(Vector3.left * -deltaX * leftRightSpeed);
                }
            }
        }   
    }
}