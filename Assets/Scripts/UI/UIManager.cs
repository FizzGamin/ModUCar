public class UIManager : UnitySingleton<UIManager>
{
    private InventoryUI inventoryUI;
    private PauseMenuUI pauseMenuUI;
    private InteractionHud interactionHud;
    private InteractionHud vehicleInteractionHud;

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

    public static InteractionHud GetVehicleInteractionHud()
    {
        return instance.vehicleInteractionHud;
    }

    public static void SetVehicleInteractionHud(VehicleInteractionHud vehicleInteractionHud)
    {
        instance.vehicleInteractionHud = vehicleInteractionHud;
    }
}
