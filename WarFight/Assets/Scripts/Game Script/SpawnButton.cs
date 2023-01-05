using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnButton : MonoBehaviour
{
    [SerializeField] GameObject Button1;
    [SerializeField] GameObject Button2;
    [SerializeField] GameObject Button3;
    [SerializeField] GameObject Button4;
    //button的種類可變多,只要把下面的switch改多點case
    //但一次最多顯示四種button
    Vector3 v1 = new Vector3(-2, 0), v2 = new Vector3(0, 2);
    Vector3 v3 = new Vector3(2, 0), v4 = new Vector3(0, -2);

    public void Spawn(int i, int j, int k, int l)
    {
        GameObject btn1, btn2, btn3, btn4;
        switch (i)
        {
            case 1: btn1 = Instantiate(Button1, v1 , Quaternion.identity); break;
            //正式時，將v1和Quaternion改為parent的transform
            //ex:Instantiate(Button1, parent)?
            case 2:btn1 = Instantiate(Button2, v1, Quaternion.identity); break;
            case 3:btn1 = Instantiate(Button3, v1, Quaternion.identity); break;
            case 4: btn1 = Instantiate(Button4, v1, Quaternion.identity); break;
            default: break;
        }
        switch (j)
        {
            case 1: btn2 = Instantiate(Button1, v2, Quaternion.identity); break;
            case 2: btn2 = Instantiate(Button2, v2, Quaternion.identity); break;
            case 3: btn2 = Instantiate(Button3, v2, Quaternion.identity); break;
            case 4: btn2 = Instantiate(Button4, v2, Quaternion.identity); break;
            default: break;
        }
        switch (k)
        {
            case 1: btn3 = Instantiate(Button1, v3, Quaternion.identity); break;
            case 2: btn3 = Instantiate(Button2, v3, Quaternion.identity); break;
            case 3: btn3 = Instantiate(Button3, v3, Quaternion.identity); break;
            case 4: btn3 = Instantiate(Button4, v3, Quaternion.identity); break;
            default: break;
        }
        switch (l)
        {
            case 1: btn4 = Instantiate(Button1, v4, Quaternion.identity); break;
            case 2: btn4 = Instantiate(Button2, v4, Quaternion.identity); break;
            case 3: btn4 = Instantiate(Button3, v4, Quaternion.identity); break;
            case 4: btn4 = Instantiate(Button4, v4, Quaternion.identity); break;
            default: break;
        }
    }
    public void Start()
    {
        Spawn(4,1,2,3);//test
    }
}
