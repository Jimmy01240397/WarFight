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
            GetNameWindow.SetActive(true);
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
