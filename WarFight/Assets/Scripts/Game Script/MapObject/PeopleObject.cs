using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleObject : MapObject
{
    public Animator animator;
    // Start is called before the first frame update

    public override void SetData(IDictionary data)
    {
        base.SetData(data);

        GetComponent<Animator>().SetBool("walking", ((Dictionary<string, bool>)data["animation"])["walking"]);
        GetComponent<Animator>().SetBool("special_moving", ((Dictionary<string, bool>)data["animation"])["special_moving"]);
    }

    public override int GetHashCode()
    {
        return PacketType.Tools.HashCombine(base.GetHashCode(), PacketType.Tools.HashCombine(GetComponent<Animator>().GetBool("walking").GetHashCode(), GetComponent<Animator>().GetBool("special_moving").GetHashCode()));
    }

    protected override void Awake()
    {

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Start();
    }
}
