using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrptCancel : MonoBehaviour
{
    [SerializeField] GameObject GetNameWindow;
    [SerializeField] GameObject MainWindow;
    public void CancelTheWindow()
    {
        if (GetNameWindow != null)
        {
            GetNameWindow.SetActive(false);
            if (MainWindow != null)
            {
                MainWindow.SetActive(true);
            }
        }
    }
}
