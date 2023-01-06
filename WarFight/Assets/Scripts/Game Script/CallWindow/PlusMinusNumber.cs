using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlusMinusNumber : MonoBehaviour
{
    [SerializeField] GameObject InputNumber;
    public int i;
    public string str;

    public void plus()
    {
        str = InputNumber.GetComponent<TMP_InputField>().text;
        int.TryParse(str, out i);
        i += 1;
        str = i.ToString();
        InputNumber.GetComponent<TMP_InputField>().text = str;
    }
    public void minus()
    {
        str = InputNumber.GetComponent<TMP_InputField>().text;
        int.TryParse(str, out i);
        i -= 1;
        str = i.ToString();
        InputNumber.GetComponent<TMP_InputField>().text = str;
    }
}
