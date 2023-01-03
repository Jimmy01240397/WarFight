using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrptConfirm : MonoBehaviour
{
    [SerializeField] GameObject GetNameWindow;
    [SerializeField] GameObject NewName;
    [SerializeField] GameObject ShowName;
    [SerializeField] GameObject MainWindow;

    public void SetName()
    {
        if (NewName != null && ShowName != null)
        {
            string str = NewName.GetComponent<TMPro.TextMeshProUGUI>().text;
            ShowName.GetComponent<TMPro.TextMeshProUGUI>().text = "Nickname: " + str;

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
}
