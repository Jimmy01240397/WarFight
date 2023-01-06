using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnCloseWindow : MonoBehaviour
{
    public Transform ThisTransform;
    public GameObject ThisWindow;
    private void Start()
    {
        ThisTransform= gameObject.GetComponent<Transform>();
        ThisWindow = ThisTransform.parent.gameObject;
    }
    public void CloseWindow()
    {
        ThisWindow.SetActive(false);
    }
}
