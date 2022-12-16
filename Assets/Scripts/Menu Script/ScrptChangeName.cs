using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrptChangeName : MonoBehaviour
{
    [SerializeField] GameObject GetNameWindow;
    [SerializeField] GameObject MainWindow;

    public void CallWindow()
    {
        if (GetNameWindow != null)
        {
            GetNameWindow.transform.localPosition += Vector3.down * 500;
            if (MainWindow != null)
            {
                MainWindow.SetActive(false);
            }
        }
    }
    void Start()
    {
        CallWindow();
    }
}
