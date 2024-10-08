# VeinMiner

- Authors: Megghy|YSpoof|Maxthegreat99|肝帝熙恩|Cai
- Source: [github](https://github.com/Maxthegreat99/TSHockVeinMiner)
- To quickly mine veins of ore
  
> [!IMPORTANT]
> To enable vein mining, you need the `veinminer` permission.
> Authorization command: `/group addperm default veinminer` (default is the default group, you can replace it with the group you need)

## Commands

| Command        | Permission |                 Details                 |
|----------------|:----------:|:---------------------------------------:|
| /vm            | veinminer  |           Toggle vein mining            |
| /vm [Any agrs] | veinminer  | Toggle vein mining notification message |

## Config
> Configuration file location：tshock/VeinMiner.json
```json
{
  "启用": true, //Enable
  "广播": true, //Broadcast
  "放入背包": true, //Put ores into player's inventory
  "矿石类型": [  //TileID which will be mined by VeinMiner
    7,
    166,
    6,
    167,
    9,
    168,
    8
  ],
  "兑换规则": [ //Exchange rules
    {
      "仅给予物品": false, //Only give item
      "最小尺寸": 0, //Min size
      "类型": 0, //Tile ID
      "物品": null //Item
    }
  ]
}
```
### Example
```json
{
  "启用": true, //Enable
  "广播": true, //Broadcast
  "放入背包": true, //Put ores into player's inventory
  "矿石类型": [ //TileID which will be mined by VeinMiner
    7,
    166,
    6,
    167,
    9,
    168
  ],
  "兑换规则": [ //Exchange rules
    {
      "仅给予物品": true, //Item
      "最小尺寸": 10,  //Min size
      "类型": 168, //Tile ID
      "物品": {
        "666": 1, //"ItemID": stack
        "669": 1
      }
    },
    {
      "仅给予物品": true, 
      "最小尺寸": 10,
      "类型": 8,
      "物品": {
        "662": 5,
        "219": 1
      }
    }
  ]
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
