using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Plugin;

[ApiVersion(2, 1)]
public class EndureBoost : TerrariaPlugin
{
    public static Configuration Config;

    public override string Author => "肝帝熙恩";

    public override string Description => "一定数量后长时间buff";

    public override string Name => "EndureBoost";

    public override Version Version => new Version(1, 0, 1);

    public EndureBoost(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        GeneralHooks.ReloadEvent += ReloadConfig;
        ServerApi.Hooks.ServerJoin.Register(this, this.OnServerJoin);
        GetDataHandlers.PlayerSpawn += this.Rebirth;
        Commands.ChatCommands.Add(new Command("EndureBoost", this.SetPlayerBuffcmd, "ebbuff", "ldbuff", "loadbuff"));
        LoadConfig();
    }

    private void SetPlayerBuffcmd(CommandArgs args)
    {
        var player = args.Player;
        this.SetPlayerBuff(player);
    }

    private static void LoadConfig()
    {
        Config = Configuration.Read(Configuration.FilePath);
        Config.Write(Configuration.FilePath);
    }

    private static void ReloadConfig(ReloadEventArgs args)
    {
        LoadConfig();
        args.Player.SendSuccessMessage("[{0}] 重新加载配置完毕。", typeof(EndureBoost).Name);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.SetPlayerBuffcmd);
            GeneralHooks.ReloadEvent -= ReloadConfig;
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnServerJoin);
            GetDataHandlers.PlayerSpawn -= this.Rebirth;
        }
        base.Dispose(disposing);
    }

    private void OnServerJoin(JoinEventArgs args)
    {
        var playerBuff = TShock.Players[args.Who];
        if (playerBuff == null)
        {
            return;
        }
        this.SetPlayerBuff(playerBuff);
    }

    private void Rebirth(object o, GetDataHandlers.SpawnEventArgs args)
    {
        if (args.Player == null)
        {
            return;
        }
        var player = args.Player;
        this.SetPlayerBuff(player);
    }

    private void SetPlayerBuff(TSPlayer player)
    {

        // 处理 potions
        foreach (var potion in Config.Potions)
        {
            foreach (var itemId in potion.ItemID)
            {
                var itemCount = 0;

                // 检查背包中的物品
                for (var i = 0; i < 58; i++)
                {
                    if (player.TPlayer.inventory[i].type == itemId)
                    {
                        itemCount += player.TPlayer.inventory[i].stack;
                    }
                }

                // 检查不同存储区中的物品
                this.CheckBanksForItem(player, itemId, ref itemCount);

                if (itemCount >= potion.RequiredStack)
                {
                    var buffType = this.GetBuffIDByItemID(itemId); // 获取物品的 buff 类型
                    if (buffType != 0)
                    {
                        player.SetBuff(buffType, Config.duration * 60);
                    }
                }
            }
        }

        // 处理 stations
        foreach (var station in Config.Stations)
        {
            foreach (var itemId in station.Type)
            {
                var itemCount = 0;

                // 检查背包中的物品
                for (var i = 0; i < 58; i++)
                {
                    if (player.TPlayer.inventory[i].type == itemId)
                    {
                        itemCount += player.TPlayer.inventory[i].stack;
                    }
                }

                // 检查不同存储区中的物品
                this.CheckBanksForItem(player, itemId, ref itemCount);

                if (itemCount >= station.RequiredStack)
                {
                    player.SetBuff(station.BuffType, Config.duration * 60);
                }
            }
        }
    }

    private void CheckBanksForItem(TSPlayer player, int itemId, ref int itemCount)
    {
        for (var j = 0; j < 40; j++)
        {
            if (player.TPlayer.bank.item[j].type == itemId && Config.bank)// 检查猪猪储钱罐
            {
                itemCount += player.TPlayer.bank.item[j].stack;
            }
            if (player.TPlayer.bank2.item[j].type == itemId && Config.bank2)// 检查保险箱
            {
                itemCount += player.TPlayer.bank2.item[j].stack;
            }
            if (player.TPlayer.bank3.item[j].type == itemId && Config.bank3)// 检查护卫熔炉
            {
                itemCount += player.TPlayer.bank3.item[j].stack;
            }
            if (player.TPlayer.bank4.item[j].type == itemId && Config.bank4)// 检查虚空宝藏袋
            {
                itemCount += player.TPlayer.bank4.item[j].stack;
            }
        }
    }

    private int GetBuffIDByItemID(int itemId)
    {
        var item = new Item();
        item.SetDefaults(itemId);
        return item.buffType;
    }
}