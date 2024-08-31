# Economics.RPG 升级职业插件

- 作者: 少司命
- 出处: 无
- RPG 升级插件  

> [!NOTE]  
> 需要安装前置插件：EconomicsAPI (本仓库)  

## 更新日志

```
V1.0.0.2
- 添加权限economics.rpg.chat，拥有此权限不会改变玩家聊天格式。

V1.0.0.1
- 增加显示信息
- 添加/level reset指令
- 添加自定义消息玩家组
- 添加RPG聊天渐变色
```

## 指令

| 语法           |        权限         |   说明   |
| -------------- | :-----------------: | :------: |
| /rank [职业名] | economics.rpg.rank  |   升级   |
| /重置等级      | economics.rpg.reset | 重置职业 |
| /level reset   | economics.rpg.admin | 重置     |

## 配置
> 配置文件位置：tshock/Economics/RPG.json
```json
{
  "RPG信息": {
    "战士": {
      "聊天前缀": "[战士]",
      "聊天颜色": [0, 238, 0],
      "聊天后缀": "",
      "聊天格式": "{0}{1}{2}: {3}",
      "升级广播": "恭喜玩家{0}升级至{1}!",
      "进度限制": [],
      "升级指令": [],
      "附加权限": [],
      "升级奖励": [],
      "升级消耗": 1000,
      "父等级": "萌新"
    },
    "战士2": {
      "聊天前缀": "[战士2]",
      "聊天颜色": [0, 238, 0],
      "聊天后缀": "",
      "聊天格式": "{0}{1}{2}: {3}",
      "升级广播": "恭喜玩家{0}升级至{1}!",
      "进度限制": [],
      "升级指令": [],
      "附加权限": [],
      "升级奖励": [],
      "升级消耗": 1000,
      "父等级": "战士"
    },
    "重置职业执行命令": [],
    "重置职业广播": "玩家{0}重新选择了职业",
    "重置后踢出": false,
    "默认等级": "萌新"
  }
}
```
## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love