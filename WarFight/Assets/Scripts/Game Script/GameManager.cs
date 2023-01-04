using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public IDictionary[] MapObjects { get; set; }

    public static GameManager Instance
    {
        get
        {
            return GameObject.Find("GameManager").GetComponent<GameManager>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
