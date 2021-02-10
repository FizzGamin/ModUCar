using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class GameManager : UnitySingleton<GameManager>
{
    private IPlayer player;
    private LootService lootService;

    public void SetPlayer(IPlayer player)
    {
        this.player = player;
    }

    public IPlayer GetPlayer()
    {
        return player;
    }

    public void SetLootService(LootService lootService)
    {
        this.lootService = lootService;
    }

    public LootService GetLootService()
    {
        return lootService;
    }
}
