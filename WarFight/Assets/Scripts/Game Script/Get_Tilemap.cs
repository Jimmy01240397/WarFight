using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Get_Tilemap : MonoBehaviour
{
    byte[][] tileData = new byte[60][];
    public Tile[] tiles;
    private Tilemap tilemap;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        for(int i=0;i<60;i++)
        {
            tileData[i] = new byte[60];
            for(int j=0;j<60;j++)
            {
                tileData[i][j] = (byte)Array.IndexOf(tiles, tilemap.GetTile(new Vector3Int(j, i)));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
