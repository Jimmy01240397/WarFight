using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnListButton : MonoBehaviour
{
    [SerializeField] GameObject Button1, Button2, Button3, Button4, Button5, Button6;
    [SerializeField] GameObject Button7, Button8;
    [SerializeField] GameObject Parent;
    //button�������i�ܦh,�u�n��U����switch��h�Icase
    //���@���̦h��ܥ|��button

    public void Spawn(int i)
    {
        GameObject btn1;
        switch (i)
        {
            case 1: btn1 = Instantiate(Button1, Parent.transform); break;
            //�����ɡA�Nv1�MQuaternion�אּparent��transform
            //ex:Instantiate(Button1, parent)?
            case 2: btn1 = Instantiate(Button2, Parent.transform); break;
            case 3: btn1 = Instantiate(Button3, Parent.transform); break;
            case 4: btn1 = Instantiate(Button4, Parent.transform); break;
            case 5: btn1 = Instantiate(Button5, Parent.transform); break;
            case 6: btn1 = Instantiate(Button6, Parent.transform); break;
            case 7: btn1 = Instantiate(Button7, Parent.transform); break;
            case 8: btn1 = Instantiate(Button8, Parent.transform); break;
            default: break;
        }
    }
    public void Start()
    {
        Spawn(4);//test
        Spawn(8);
        Spawn(2);
        Spawn(2);
        Spawn(3);
        Spawn(1);
        Spawn(7);
    }
}
