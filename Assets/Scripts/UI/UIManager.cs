using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UIManager : UnitySingleton<UIManager>
{
    private InventoryUI inventoryUI;
    private PauseMenuUI pauseMenuUI;
    private DeathMenuUI deathMenuUI;
    private InteractionHud interactionHud;
    private InteractionHud vehicleInteractionHud;
    private GenericBarUI healthBarUI;
    private GenericBarUI hungerBarUI;
    private GenericBarUI fuelBarUI;

    public static GameObject GetUIBase()
    {
        return instance.gameObject;
    }

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

    public static DeathMenuUI GetDeathMenuUI()
    {
        return instance.deathMenuUI;
    }

    public static void SetDeathMenuUI(DeathMenuUI deathMenuUI)
    {
        instance.deathMenuUI = deathMenuUI;
    }

    public static InteractionHud GetInteractionHud()
    {
        return instance.interactionHud;
    }

    public static void SetInteractionHud(InteractionHud interactionHud)
    {
        instance.interactionHud = interactionHud;
    }

    public static InteractionHud GetVehicleInteractionHud()
    {
        return instance.vehicleInteractionHud;
    }

    public static void SetVehicleInteractionHud(VehicleInteractionHud vehicleInteractionHud)
    {
        instance.vehicleInteractionHud = vehicleInteractionHud;
    }

    public static GenericBarUI GetFuelBarUI()
    {
        return instance.fuelBarUI;
    }

    public static void SetFuelBarUI(GenericBarUI fuelBarUI)
    {
        instance.fuelBarUI = fuelBarUI;
    }

    public static GenericBarUI GetHealthBarUI()
    {
        return instance.healthBarUI;
    }

    public static void SetHealthBarUI(GenericBarUI healthBarUI)
    {
        instance.healthBarUI = healthBarUI;
    }

    public static GenericBarUI GetHungerBarUI()
    {
        return instance.hungerBarUI;
    }

    public static void SetHungerBarUI(GenericBarUI hungerBarUI)
    {
        instance.hungerBarUI = hungerBarUI;
    }
}
