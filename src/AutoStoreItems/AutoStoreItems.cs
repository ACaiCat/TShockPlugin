﻿using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Plugin;

[ApiVersion(2, 1)]
public class AutoStoreItems : TerrariaPlugin
{

    #region 插件信息
    public override string Name => "自动存储";
    public override string Author => "羽学 cmgy雱";
    public override Version Version => new Version(1, 2, 5);
    public override string Description => "涡轮增压不蒸鸭";
    #endregion

    #region 注册与释放
    private GeneralHooks.ReloadEventD _reloadHandler;
    public AutoStoreItems(Main game) : base(game) { }
    public override void Initialize()
    {
        LoadConfig();
        this._reloadHandler = (_) => LoadConfig();
        GeneralHooks.ReloadEvent += this._reloadHandler;
        GetDataHandlers.PlayerUpdate.Register(this.PlayerUpdate);
        ServerApi.Hooks.GameUpdate.Register(this, this.OnGameUpdate);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= this._reloadHandler;
            GetDataHandlers.PlayerUpdate.UnRegister(this.PlayerUpdate);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnGameUpdate);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置重载读取与写入方法
    internal static Configuration Config = new();
    private static void LoadConfig()
    {
        Config = Configuration.Read();
        WriteName();
        Config.Write();
        TShock.Log.ConsoleInfo(GetString("[自动存储]重新加载配置完毕。"));
    }
    #endregion

    #region 获取物品ID的中文名
    private static void WriteName()
    {
        foreach (var item in Config.Items)
        {
            var Names = new HashSet<string>(item.Name.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries));
            foreach (var id in item.ID)
            {
                string ItemName;
                ItemName = (string) Lang.GetItemName(id);
                if (!Names.Contains(ItemName))
                {
                    Names.Add(ItemName);
                }
            }
            item.Name = string.Join(", ", Names);
        }
    }
    #endregion

    #region 检测背包持有物品方法
    public static long Timer = 0L;
    private void OnGameUpdate(EventArgs args)
    {
        Timer++;

        foreach (var plr in TShock.Players.Where(plr => plr != null && plr.Active && plr.IsLoggedIn && Config.Enable))
        {
            foreach (var item in Config.HoldItems)
            {
                var Stored = false;
                for (var i = 0; i < plr.TPlayer.inventory.Length; i++)
                {
                    var inv = plr.TPlayer.inventory[i];

                    if (Timer % Config.ItemTime == 0)
                    {
                        if ((Config.Hand ? inv.type == plr.TPlayer.inventory[plr.TPlayer.selectedItem].type : inv.type == item) &&
                            (Config.bank1 && !Stored && (Stored = AutoStoredItem(plr, plr.TPlayer.bank.item, PlayerItemSlotID.Bank1_0, "存钱罐")) ||
                             Config.bank2 && !Stored && (Stored = AutoStoredItem(plr, plr.TPlayer.bank2.item, PlayerItemSlotID.Bank2_0, "保险箱")) ||
                             Config.bank3 && !Stored && (Stored = AutoStoredItem(plr, plr.TPlayer.bank3.item, PlayerItemSlotID.Bank3_0, "护卫熔炉")) ||
                             Config.bank4 && !Stored && (Stored = AutoStoredItem(plr, plr.TPlayer.bank4.item, PlayerItemSlotID.Bank4_0, "虚空袋"))))
                        {
                            break;
                        }
                    }

                    else if (Timer % Config.CoinTime == 0)
                    {
                        if (Config.Hand ? inv.type == plr.TPlayer.inventory[plr.TPlayer.selectedItem].type : inv.type == item)
                        {
                            CoinToBank(plr, 71);
                            CoinToBank(plr, 72);
                            CoinToBank(plr, 73);
                            CoinToBank(plr, 74);
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region 检测装备物品方法
    private void PlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        var plr = e.Player;
        if (plr == null || !Config.Enable2)
        {
            return;
        }

        var stored = false;
        foreach (var item in Config.ArmorItem)
        {
            var armor = plr.TPlayer.armor.Take(10).Where(x => x.netID == item).ToList();
            var hasArmor = armor.Any();

            if (hasArmor)
            {
                for (var i = 71; i <= 74; i++)
                {
                    CoinToBank(plr, i);
                }

                stored |= AutoStoredItem(plr, plr.TPlayer.bank.item, PlayerItemSlotID.Bank1_0, GetString("存钱罐")) && Config.bank1;
                stored |= AutoStoredItem(plr, plr.TPlayer.bank2.item, PlayerItemSlotID.Bank2_0, GetString("保险箱")) && Config.bank2;
                stored |= AutoStoredItem(plr, plr.TPlayer.bank3.item, PlayerItemSlotID.Bank3_0, GetString("护卫熔炉")) && Config.bank3;
                stored |= AutoStoredItem(plr, plr.TPlayer.bank4.item, PlayerItemSlotID.Bank4_0, GetString("虚空袋")) && Config.bank4;

                if (stored)
                {
                    break;
                }
            }
        }
    }
    #endregion

    #region 自动储存物品方法
    public static bool AutoStoredItem(TSPlayer tplr, Item[] bankItems, int bankSlot, string bankName)
    {
        if (!tplr.IsLoggedIn || tplr == null)
        {
            return false;
        }

        var plr = tplr.TPlayer;
        var itemID = new HashSet<int>(Config.Items.SelectMany(x => x.ID));

        foreach (var bank in bankItems)
        {
            for (var i = 0; i < plr.inventory.Length; i++)
            {
                var inv = plr.inventory[i];
                var items = Config.Items.FirstOrDefault(x => x.ID.Contains(inv.type));

                if (items != null
                    && inv.stack >= items.Stack
                    && itemID.Contains(inv.type)
                    && inv.type == bank.type
                    && inv.type != plr.inventory[plr.selectedItem].type)
                {

                    bank.stack += inv.stack;
                    inv.TurnToAir();

                    if (bank.stack >= Item.CommonMaxStack)
                    {
                        bank.stack = Item.CommonMaxStack;
                    }

                    tplr.SendData(PacketTypes.PlayerSlot, null, tplr.Index, PlayerItemSlotID.Inventory0 + i);
                    tplr.SendData(PacketTypes.PlayerSlot, null, tplr.Index, bankSlot + Array.IndexOf(bankItems, bank));

                    if (Config.Mess)
                    {
                        tplr.SendMessage(GetString($"【自动储存】已将'[c/92C5EC:{bank.Name}]'存入您的{bankName} 当前数量: {bank.stack}"), 255, 246, 158);
                    }

                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    #region 自动存钱到存钱罐方法
    private static void CoinToBank(TSPlayer tplr, int coin)
    {
        var plr = tplr.TPlayer;
        var bankItem = new Item();
        var bankSolt = -1;

        for (var i2 = 0; i2 < 40; i2++)
        {
            bankItem = plr.bank.item[i2];
            if (bankItem.IsAir || bankItem.netID == coin)
            {
                bankSolt = i2;
                break;
            }
        }

        if (bankSolt != -1)
        {
            for (var i = 50; i < 54; i++)
            {
                var invItem = plr.inventory[i];
                if (invItem.netID == coin)
                {
                    invItem.netID = 0;
                    tplr.SendData(PacketTypes.PlayerSlot, "", tplr.Index, i);

                    bankItem.netID = coin;
                    bankItem.type = invItem.type;
                    bankItem.stack += invItem.stack;

                    if (bankItem.stack >= 100 && coin != 74)
                    {
                        bankItem.stack %= 100;
                        tplr.GiveItem(coin + 1, 1);
                    }

                    else if (bankItem.stack >= Item.CommonMaxStack && coin == 74)
                    {
                        bankItem.stack = Item.CommonMaxStack;
                    }

                    tplr.SendData(PacketTypes.PlayerSlot, "", tplr.Index, PlayerItemSlotID.Bank1_0 + bankSolt);
                    break;
                }
            }
        }
    }
    #endregion

}