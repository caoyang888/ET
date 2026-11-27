# cn.etetet.slotcore

老虎机权威结算。数学引擎是纯函数，玩家是 Slot 场景上的 Location Actor，房间账本在 `SlotWorldComponent`。

- 只依赖 ET 基础包和 `login`，不依赖 `item`、`unit`、`map`、`numeric`。
- `slotops` 可以依赖本包，本包不能反向依赖 `slotops`。
- 控盘数字来自 Luban，不写死在结算代码里。
