using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : UnitySingleton<UIManager>
{
    private InventoryUI inventoryUI;
    private PauseMenuUI pauseMenuUI;
    private InteractionHud interactionHud;

    public static InventoryUI GetInventoryUI()
    {
        return instance.inventoryUI;
    }

    public static void SetInventoryUI(InventoryUI inventoryUI)
    {
        instance.inventoryUI = inventoryUI;
    }

    public static PauseMenuUI GetPauseMenuUI()
    {
        return instance.pauseMenuUI;
    }

    public static void SetPauseMenuUI(PauseMenuUI pauseMenuUI)
    {
        instance.pauseMenuUI = pauseMenuUI;
    }

    public static InteractionHud GetInteractionHud()
    {
        return instance.interactionHud;
    }

    public static void SetInteractionHud(InteractionHud interactionHud)
    {
        instance.interactionHud = interactionHud;
    }
}
