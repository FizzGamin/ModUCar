using System;
using UnityEngine;

public class TestItem : IPickupable
{
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public override string GetName()
    {
        return "Test Object 1";
    }

    public override ItemQuality GetQuality()
    {
        return ItemQuality.D;
    }

    public override int GetWeight()
    {
        return 200;
    }

    public override string GetSpriteName()
    {
        return "TestSprite1";
    }
}
