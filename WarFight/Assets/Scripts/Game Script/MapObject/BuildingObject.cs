using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingObject : MapObject
{
    // Start is called before the first frame update

    protected override void Awake()
    {
        Vector2 spritesize = GetComponent<SpriteRenderer>().sprite.rect.size;
        MaxLife = (float)GameManager.Instance.Builds[type]["life"];
        transform.localScale = new Vector3(TilemapSetting.unit * ((int)GameManager.Instance.Builds[type]["width"] + 1) / spritesize.x, TilemapSetting.unit * ((int)GameManager.Instance.Builds[type]["height"] + 1) / spritesize.y, 1);
        //MaxLife = (float)GameManager.Instance.Builds[type]["life"];

        base.Awake();
    }

    protected override void Start()
    {

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {

        base.Update();
    }
}
