using System;
using UnityEngine;

public class TestItem2 : IPickupable
{
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            Debug.Log("Script is active");
        }
    }

    public override string GetName()
    {
        return "Test Object 2: Electric Boogaloo";
    }

    public override ItemQuality GetQuality()
    {
        return ItemQuality.B;
    }

    public override int GetWeight()
    {
        return 100;
    }

    public override string GetSpriteName()
    {
        return "TestSprite2";
    }
}
