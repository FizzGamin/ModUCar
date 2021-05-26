using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UIManager : UnitySingleton<UIManager>
{
    private InventoryUI inventoryUI;
    private PauseMenuUI pauseMenuUI;
    private DeathMenuUI deathMenuUI;
    private FinishMenuUI finishMenuUI;
    private InteractionHud interactionHud;
    private InteractionHud vehicleInteractionHud;
    private GenericBarUI healthBarUI;
    private GenericBarUI hungerBarUI;
    private GenericBarUI vehicleHealthBarUI;
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

    public static FinishMenuUI GetFinishMenuUI()
    {
        return instance.finishMenuUI;
    }

    public static void SetFinishMenuUI(FinishMenuUI finishMenuUI)
    {
        instance.finishMenuUI = finishMenuUI;
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

    public static GenericBarUI GetVehicleHealthBarUI()
    {
        return instance.vehicleHealthBarUI;
    }

    public static void SetVehicleHealthBarUI(GenericBarUI vehicleHealthBarUI)
    {
        instance.vehicleHealthBarUI = vehicleHealthBarUI;
    }
}
