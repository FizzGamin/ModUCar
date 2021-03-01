public class GameManager : UnitySingleton<GameManager>
{
    private IPlayer player;
    private LootService lootService;
    private SpriteService spriteService;

    public static void SetPlayer(IPlayer player)
    {
        instance.player = player;
    }

    public static IPlayer GetPlayer()
    {
        return instance.player;
    }

    public static void SetLootService(LootService lootService)
    {
        instance.lootService = lootService;
    }

    public static LootService GetLootService()
    {
        return instance.lootService;
    }

    public static void SetSpriteService(SpriteService spriteService)
    {
        instance.spriteService = spriteService;
    }

    public static SpriteService GetSpriteService()
    {
        return instance.spriteService;
    }
}
