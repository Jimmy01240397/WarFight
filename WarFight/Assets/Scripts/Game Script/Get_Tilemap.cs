using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Chuon;
using PacketType;
using UnityEngine.SceneManagement;

public class Get_Tilemap : MonoBehaviour
{
    public Tile[] tiles;
    private Tilemap tilemap;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        Manager.Instance.ClientForGame.ClientLinker.Ask((byte)GameServerAndClientRequestType.GetMap, null, (responsedata) =>
        {
            switch ((GameServerAndClientResponseType)responsedata.Code)
            {
                case GameServerAndClientResponseType.Login:
                    {
                        if (!Convert.ToBoolean(responsedata.ReturnCode))
                        {
                            Manager.Instance.ClientForGame.ClientLinker.Disconnect();
                            Manager.Instance.ClientForLogin.ClientLinker.Ask((byte)LoginServerAndClientRequestType.Logout, null);
                            Manager.Instance.Username = null;
                            Manager.Instance.RoomNum = null;
                            Manager.Instance.RoomPlayer = null;
                            SceneManager.LoadScene("MenuScene");
                        }
                    }
                    break;
                case GameServerAndClientResponseType.GetMap:
                    {
                        Dictionary<string, object> mapdata = (Dictionary<string, object>)responsedata.Parameters;
                        GameManager.Instance.MapObjects = (IDictionary[])mapdata["MapObjects"];
                        byte[][] tileData = (byte[][])mapdata["TileMap"];
                        for (int i = 0; i < tileData.Length; i++)
                        {
                            for (int j = 0; j < tileData[i].Length; j++)
                            {
                                tilemap.SetTile(new Vector3Int(j, i), tiles[tileData[i][j]]);
                                //tilemap.SetTiles(((List<Vector3Int>)ONLINE.MapData[0]).ToArray(), ((List<Tile>)ONLINE.MapData[1]).ToArray());
                            }
                        }
                    }
                    break;
            }
        }, 1000);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TilemapToChuon()
    {
        byte[][] tileData = new byte[60][];
        for (int i = 0; i < 60; i++)
        {
            tileData[i] = new byte[60];
            for (int j = 0; j < 60; j++)
            {
                tileData[i][j] = (byte)Array.IndexOf(tiles, tilemap.GetTile(new Vector3Int(j, i)));
            }
        }
        ChuonString chuonString = new ChuonString(tileData);
        Debug.Log(chuonString.ToString());
    }
}
