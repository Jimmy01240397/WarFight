using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] int moveSpeed = 1;
    Transform t;
    private void Start()
    {
         t = GetComponent<Transform>();
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            t.position += moveSpeed *Vector3.up* Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            t.position += moveSpeed * Vector3.down * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            t.position += moveSpeed * Vector3.left * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            t.position += moveSpeed * Vector3.right * Time.deltaTime;
        }
    }
}
