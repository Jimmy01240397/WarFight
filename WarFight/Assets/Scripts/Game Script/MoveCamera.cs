using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public int moveSpeed = 10;
    public int scaleSpeed = 50;

    private void Start()
    {
    }
    void Update()
    {
        float fov = Camera.main.orthographicSize;
        fov += Input.GetAxis("Mouse ScrollWheel") * scaleSpeed;
        fov = Math.Abs(fov);
        Camera.main.orthographicSize = fov;
        float m_speed = moveSpeed * (Camera.main.orthographicSize / 5);

        Vector3 way = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            way = Vector3.up;
        else if (Input.GetKey(KeyCode.S))
            way = Vector3.down;

        if (Input.GetKey(KeyCode.A))
            way = Vector3.left;
        else if (Input.GetKey(KeyCode.D))
            way = Vector3.right;
        transform.Translate(m_speed * way * Time.deltaTime);
    }
}
