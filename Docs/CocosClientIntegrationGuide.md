# Cocos Creator 客户端对接说明

这份文档给 Cocos Creator（TypeScript）客户端使用。把本文交给实现客户端的 AI 时，按本文的字节布局写编解码，不要改成 Protobuf，也不要手写一套“看起来像 ET”的帧。服务端外网 WebSocket 的消息体是 **MemoryPack**，不是 Protobuf wire，也不是 JSON。

仓库里的 `.proto` 只负责字段名、字段顺序和 Opcode。真正发出去的字节按本文第 2 节编码。

已用 `ET.Slot.IntegrationTests.OuterFrameTests` 对 `C2G_EnterSlot { RpcId = 7 }` 做了往返断言。完整帧（十六进制）是：

```text
07 00 E5 2E 01 07 00 00 00
```

含义：

| 字节 | 值 | 含义 |
| --- | --- | --- |
| 0–1 | `07 00` | 小端 uint16，后续 body 长度 = 7 |
| 2–3 | `E5 2E` | 小端 uint16，Opcode = 12005（`C2G_EnterSlot`） |
| 4 | `01` | MemoryPack 对象头，成员个数 = 1 |
| 5–8 | `07 00 00 00` | 小端 int32，`RpcId = 7` |

---

## 1. 本地启动

工作目录是仓库根目录（`ET.sln` 所在目录）。命令使用 PowerShell 7（`pwsh`）。

### 1.1 PostgreSQL

连接串优先级：

1. 环境变量 `SLOT_POSTGRES_CONNECTION_STRING`（Npgsql 关键字形式）
2. 环境变量 `DATABASE_URL`（支持 `postgres://` 与 `postgresql://`，启动时会转成 Npgsql 关键字）
3. `Config/postgres.json` 的 `ConnectionString`

默认内容：

```text
Host=127.0.0.1;Port=5432;Username=slot;Password=slot;Database=slot
```

一键容器（只包含 PostgreSQL 16，首次初始化会执行 schema）：

```powershell
docker compose up -d
```

`docker-compose.yml` 把 `Config/Sql/slot_postgres_schema.sql` 挂到 `docker-entrypoint-initdb.d`。数据卷 `slot_pg` 已存在时，入口脚本不会再次执行；服务端启动 Slot 场景时会再执行一次同一份幂等 SQL（全部是 `CREATE TABLE IF NOT EXISTS`）。

不用 Docker 时，本机 PostgreSQL 16 执行：

```sql
CREATE ROLE slot LOGIN PASSWORD 'slot' SUPERUSER;
CREATE DATABASE slot OWNER slot;
```

然后：

```powershell
$env:PGPASSWORD = "slot"
psql -h 127.0.0.1 -U slot -d slot -f Config/Sql/slot_postgres_schema.sql
```

Slot 进程启动时调用 `SlotPostgresDatabase.EnsureSchema()`，重复执行安全。

### 1.2 配置与协议导出

改过 `Packages/cn.etetet.slotcore/Luban`、`Packages/cn.etetet.slotops/Luban` 或 `Packages/cn.etetet.startconfig/Luban` 之后：

```powershell
dotnet ./Bin/ET.ExcelExporter.dll
```

成功标志是控制台出现 `excelexporter ok!`。这个导出只生成服务端 C#，**没有客户端 JSON**。Cocos 不要读服务端配置表来驱动 UI 数值；房间、押注档、任务、商品、邮件都从下面的查询协议拿。

改过 `.proto` 之后按仓库的 Proto 导出流程重新生成 `Packages/cn.etetet.proto/CodeMode/Model/Server`。客户端只使用第 4 节列出的外网 Opcode。内网消息（`Slot_S_22000.proto`、`Login_S_20000.proto`）不要实现。

### 1.3 启动服务端

```powershell
dotnet build ET.sln -c Debug
dotnet ./Bin/ET.App.dll --Console=1
```

工作目录必须是仓库根，不是 `Bin`。

`Config/websocket.json`：

| 字段 | 默认 | 含义 |
| --- | --- | --- |
| Enable | true | Realm 与 Gate 都挂外网 WebSocket |
| Path | `/ws` | URL 路径 |
| PortOffset | 1000 | WebSocket 端口 = 场景外网 UDP 端口 + 1000 |
| UseTls | false | 明文 `ws://`。改成 true 才是 `wss://`，并要填证书 |

Localhost 场景端口（`StartSceneConfig`）：

| 场景 | UDP 端口 | WebSocket |
| --- | --- | --- |
| Realm | 30101 | `ws://127.0.0.1:31101/ws` |
| Gate1 | 30102 | `ws://127.0.0.1:31102/ws` |
| Gate2 | 30103 | `ws://127.0.0.1:31103/ws` |
| Slot | 0 | 无外网端口。客户端不直连 Slot |

子协议：WebSocket **Binary**。不要发 Text 帧。一条 WebSocket 消息里可以粘多条 ET 包，接收端必须按长度循环拆包。

---

## 2. 二进制帧与 MemoryPack

### 2.1 帧

外网（`WChannel.Send` + `PacketParser`，`OuterPacketSizeLength = 2`，`OpcodeLength = 2`）：

```text
uint16 LE bodyLength
uint16 LE opcode
byte[] memoryPackBody          // 长度 = bodyLength - 2
```

`bodyLength` 含 Opcode 的 2 字节，不含自己。最小 body 是 2（`Packet.MinPacketSize`）。客户端发出的 Opcode 必须落在 **10000 到 20000（含两端）**，否则 Gate/Realm 直接丢弃。

服务端运行在小端 x64。Cocos 无论跑在什么 CPU 上，都按小端读写，不要用本机大端。

内网包（4 字节长度 + FiberInstanceId）只存在于服务器进程之间。客户端永远不要发那种包。

### 2.2 为什么不是 Protobuf

`.proto` 的 `syntax = "proto3"` 只是 ET 的消息描述。`MessageSerializeHelper` 对消息体调用 MemoryPack。字段编号不出现在字节里，也没有 protobuf 的 tag/varint。嵌套 `message` 各有一个 Opcode 常量，那只是生成器给每个类型编了号；**嵌套对象写在父消息的 MemoryPack 成员里，不能单独封一帧发出去。**

成员顺序等于 `[MemoryPackOrder]`，也就是 `.proto` 里字段的出现顺序（字段号 1 是 order 0）。漏字段、换顺序、多写一个成员，后面的整包都会错位。

### 2.3 标量

| 类型 | 字节 |
| --- | --- |
| 对象头 | 1 字节 = 成员个数。`255` 表示 null 对象 |
| int32 | 4 字节小端 |
| int64 | 8 字节小端 |
| bool | 1 字节，0 或 1 |
| List / repeated | int32 小端个数，然后连续元素。`-1`（`FF FF FF FF`）表示 null。个数 0 是 `00 00 00 00`，后面没有元素 |
| string | 见下 |

服务端字符串默认 UTF-8（`MemoryPackSerializerOptions.Default`）：

| 字符串 | 字节 |
| --- | --- |
| null | `FF FF FF FF` |
| 空串 `""` | `00 00 00 00` |
| 非空 | int32 按位取反的 UTF-8 字节数，再 int32 UTF-16 字符数，再 UTF-8 字节 |

例子：`"ab"`

```text
FD FF FF FF    // ~2 = -3
02 00 00 00    // UTF-16 长度 2
61 62          // utf-8
```

中文按 UTF-8 字节数取反，UTF-16 长度按 C# `string.Length`（一个 BMP 汉字长度为 1）。

`List<消息>` 的每个元素自己再带 1 字节对象头。例如 `SlotGrantProto` 有 2 个成员，元素以 `02` 开头，然后 int32 `ItemId`、int64 `Count`。

服务端发出的 `repeated` 字段在生成代码里初始化成空列表，所以个数是 0，不是 null。客户端解析时两种都要接受。客户端请求消息没有 `repeated` 字段。

### 2.4 RPC

带 `// ResponseType` 的请求是 RPC：

- 请求、响应都有 `int32 RpcId`，而且都是第一个成员。
- 客户端从 1 递增，跳过 0。
- 收到响应用 `RpcId` 配对，不要靠“下一包一定是响应”。主动推送没有 `RpcId`，会插在任意位置，包括插在请求和响应之间。
- 建议客户端超时 8 秒，超时后删掉待配对项并提示重试。旋转重试必须复用同一个 `ClientSpinId`。
- `Error == 0` 才是成功。`Message` 是服务端文本，成功时通常为空串。

响应类的前三个成员固定是：

1. `int32 RpcId`
2. `int32 Error`
3. `string Message`

主动推送是 `IMessage`，没有这三个字段。

### 2.5 心跳、断线、顶号

会话空闲检查每 2 秒一次。Debug 构建超时 400 秒，非 Debug 超时 40 秒。发送或接收任一方向超过这个时间，服务器关掉会话。

客户端每 10 秒发一次 `C2G_Ping`（Opcode 10003），并用 `G2C_Ping.Time` 对时。Realm 连接在拿到 Gate 地址后可以关掉；Gate 连接必须保持心跳。

断线后重新走第 3 节：新的 Realm 登录、新的 Gate Key、再次 `C2G_EnterSlot`。不要复用旧 Key。持久身份是登录时的 **账号字符串**，不是 `PlayerId`。`PlayerId` 是当前 Gate 进程里的 id，重启后会变。

顶号：同一账号在新的 Gate 会话上再次 `EnterSlot` 时，旧会话先收到：

- `G2C_PushKick`
- `Reason = 1`
- `Message = "账号在其他地方登录"`

收到后立刻关 WebSocket，停止重连，直到玩家确认是自己再次登录。

### 2.6 TypeScript 核心模块

下面的代码按第 2.1–2.3 节实现，`encodeFrame(12005, enter)` 在 `RpcId = 7` 时必须得到 `07 00 E5 2E 01 07 00 00 00`。

```typescript
export class ByteWriter {
    private bytes: number[] = [];
    u8(v: number) { this.bytes.push(v & 0xff); }
    u16(v: number) { this.u8(v); this.u8(v >>> 8); }
    i32(v: number) {
        const n = v | 0;
        this.u8(n); this.u8(n >>> 8); this.u8(n >>> 16); this.u8(n >>> 24);
    }
    i64(v: bigint) {
        let n = BigInt.asIntN(64, v);
        for (let i = 0; i < 8; i++) {
            this.u8(Number(n & 0xffn));
            n >>= 8n;
        }
    }
    object(memberCount: number) { this.u8(memberCount); }
    str(value: string | null) {
        if (value == null) { this.i32(-1); return; }
        if (value.length === 0) { this.i32(0); return; }
        const utf8 = new TextEncoder().encode(value);
        this.i32(~utf8.length);
        this.i32(value.length);
        for (const b of utf8) this.u8(b);
    }
    toArray() { return new Uint8Array(this.bytes); }
}

export class ByteReader {
    constructor(private buf: Uint8Array, public pos = 0) {}
    get remain() { return this.buf.length - this.pos; }
    u8() { return this.buf[this.pos++]; }
    u16() { return this.u8() | (this.u8() << 8); }
    i32() {
        const b = this.u8() | (this.u8() << 8) | (this.u8() << 16) | (this.u8() << 24);
        return b | 0;
    }
    i64() {
        let n = 0n;
        for (let i = 0; i < 8; i++) n |= BigInt(this.u8()) << BigInt(i * 8);
        return BigInt.asIntN(64, n);
    }
    str(): string | null {
        const mark = this.i32();
        if (mark === -1) return null;
        if (mark === 0) return "";
        const utf8Length = ~mark;
        const utf16Length = this.i32();
        const bytes = this.buf.slice(this.pos, this.pos + utf8Length);
        this.pos += utf8Length;
        const text = new TextDecoder().decode(bytes);
        if (text.length !== utf16Length) throw new Error("string length mismatch");
        return text;
    }
}

export function encodeFrame(opcode: number, payload: Uint8Array): Uint8Array {
    const bodyLen = 2 + payload.length;
    const out = new Uint8Array(2 + bodyLen);
    out[0] = bodyLen & 0xff;
    out[1] = (bodyLen >>> 8) & 0xff;
    out[2] = opcode & 0xff;
    out[3] = (opcode >>> 8) & 0xff;
    out.set(payload, 4);
    return out;
}

export interface Frame { opcode: number; payload: Uint8Array; }

export function decodeFrames(buffer: Uint8Array): { frames: Frame[]; rest: Uint8Array } {
    const frames: Frame[] = [];
    let offset = 0;
    while (buffer.length - offset >= 2) {
        const bodyLen = buffer[offset] | (buffer[offset + 1] << 8);
        if (buffer.length - offset < 2 + bodyLen) break;
        const opcode = buffer[offset + 2] | (buffer[offset + 3] << 8);
        const payload = buffer.slice(offset + 4, offset + 2 + bodyLen);
        frames.push({ opcode, payload });
        offset += 2 + bodyLen;
    }
    return { frames, rest: buffer.slice(offset) };
}

export function encodeEnterSlot(rpcId: number): Uint8Array {
    const w = new ByteWriter();
    w.object(1);
    w.i32(rpcId);
    return encodeFrame(12005, w.toArray());
}
```

`C2G_Spin` 有 5 个成员，对象头是 `05`，顺序：`RpcId`、`RoomId`、`StakeId`、`LineCount`、`ClientSpinId`。

待发 RPC 用 `Map<number, { opcode: number; resolve; reject; timer }>`。收到包时：

1. 若 Opcode 在推送表（12018–12022），交给推送回调，不消耗 RpcId。
2. 否则按该 Opcode 的响应布局读出 `RpcId`，命中 Map 就 resolve。
3. 未知 Opcode 打日志并跳过该帧，不要把后续字节错位。

---

## 3. 业务时序

必须按这个顺序。`EnterRoom` 不会创建 Slot 玩家；没发过 `EnterSlot` 时房间请求会失败。

```text
ws://127.0.0.1:31101/ws
  -> C2R_Login            Account=本地保存的账号（游客用稳定的本地 uuid）
                          Password 可为空串，服务端不校验密码
  <- R2C_Login            Address=Gate 的 KCP 地址，例如 127.0.0.1:30102
                          Key, GateId
关掉 Realm。

把 Address 的端口 + 1000，路径 /ws：
ws://127.0.0.1:31102/ws
  -> C2G_LoginGate        Key, GateId
  <- G2C_LoginGate        PlayerId（仅本进程有效）
  -> C2G_EnterSlot
  <- G2C_EnterSlot        Wallet
  -> C2G_GetProfile
  <- G2C_GetProfile
  -> C2G_EnterRoom        RoomId=101
  <- G2C_EnterRoom        关卡、Boss、押注档、三个奖池
  -> C2G_Spin             见第 4 节
  <- G2C_PushWallet       可能先于响应到达
  <- G2C_PushProgress
  <- G2C_Spin
```

之后可以穿插：任务、聚宝、骰子、王座、储蓄罐、破产补助、商城、订单、邮件、排行。这些都走同一条 Gate WebSocket。

`R2C_Login.Address` 是 UDP/KCP 地址，不是 WebSocket URL。客户端自己加 `PortOffset`（默认 1000）和路径 `/ws`。

账号字符串是 PostgreSQL 主键。游客把 uuid 存在 `sys.localStorage`，下次用同一个字符串登录，重启服务器后档案还在。

### 3.1 旋转和运营联动

`C2G_Spin` 成功结算后，服务端在同一次处理里：

- 按 Boss 伤害增加任务条件 1 和条件 8 的进度
- 按本次押注计提储蓄罐（`SlotRule.PiggyMilli = 5`，即押注的千分之 5，受 VIP 储蓄罐上限约束）。领取 `C2G_ClaimPiggy` 只把已计提的余额发到钱包
- 把玩家累计伤害写入排行（周期 1）并作为王座竞逐积分（`C2G_GetKing.Damage`）
- 写入 PostgreSQL：账号、进度、房间三账本、旋转流水（账号 + `ClientSpinId` 唯一）
- 向当前 Gate 会话推送 `G2C_PushWallet` 与 `G2C_PushProgress`

同一 `ClientSpinId` 再发一次：服务端直接返回第一次的结果，不再扣钱、不再加进度、不再计提。这个幂等在进程重启后仍然有效。

免费旋转期间 `StakeId` 与 `LineCount` 必须和触发那一次相同，否则 `Error = 200061006`。免费旋转不扣金币。

消除房（房间 301）`LineCount` 传 0。连线房 `LineCount` 为 1 到该房间 `MaxLines`。连线房实际扣费 = `BetGold * LineCount`。消除房扣费 = `BetGold`。金币道具 id 是 2。新号只有玉（道具 id 1，数量 15000），没有金币。联调先用 `C2G_DevSendMail` 发金币再领取。

### 3.2 盘面

`G2C_Spin.Steps[].Symbols` 是行主序：下标 `row * Columns + column`，行、列都从 0 开始。

| 房间 | 机台 | 玩法 | 行 × 列 | 进度组 | 最大线数 |
| --- | --- | --- | --- | --- | --- |
| 101 | 1 | 连线 | 3 × 5 | 1 | 9 |
| 201 / 202 / 203 | 2 | 连线 | 3 × 5 | 2（三间房共用进度） | 9 / 15 / 15 |
| 301 | 3 | 消除 | 5 × 6 | 3 | 0 |

`SlotWinProto.Cells` 给出中奖坐标。符号种类（配置表，客户端只在需要画特殊图标时使用）：1 普通、2 免费旋转、3 连线特征、4 彩金计数、5 消除倍数、6 彩金触发。

钱包 `SlotGrantProto`：`ItemId = 1` 玉，`ItemId = 2` 金币。列表顺序不稳定，按 ItemId 查找。

### 3.3 推送

| Opcode | 名称 | 何时 |
| --- | --- | --- |
| 12018 | `G2C_PushWallet` | 旋转结算后，以及会改钱包的运营操作之后 |
| 12019 | `G2C_PushProgress` | 旋转结算后。`TotalDamage` 为玩家累计伤害 |
| 12020 | `G2C_PushMail` | `C2G_DevSendMail` 写入新邮件后。只有 `MailId` 和 `Title`，正文用 `C2G_ListMails` |
| 12021 | `G2C_PushNotice` | 维护开关。`Kind = 2`，标题 `维护`，正文 `服务器维护中` 或 `维护结束` |
| 12022 | `G2C_PushKick` | `Reason = 1` 表示顶号 |

维护中 `C2G_EnterSlot` 与 `C2G_Spin` 返回 `200061007`。`G2C_GetProfile.Maintenance` 为 1 或 0。

---

## 4. 外网协议清单

类型写法：`i32` `i64` `str` `bool` 不在外网消息里单独出现；列表写成 `T[]`。成员顺序就是编码顺序。对象头字节等于下表“成员数”。

嵌套类型（有 Opcode，禁止作为顶层帧）：

| Opcode | 名称 | 成员 |
| --- | --- | --- |
| 12001 | `SlotGrantProto` | `i32 ItemId`, `i64 Count` |
| 12002 | `SlotCellProto` | `i32 Row`, `i32 Column`, `i32 SymbolId` |
| 12003 | `SlotWinProto` | `i32 SymbolId`, `i32 Count`, `i64 Amount`, `SlotCellProto[] Cells` |
| 12004 | `SlotStepProto` | `i32 Index`, `i32[] Symbols`, `SlotWinProto[] Wins` |
| 12009 | `SlotStakeProto` | `i32 StakeId`, `i64 BetGold`, `i32 VipLevel`, `i64 TotalCost`。`TotalCost = -1` 表示不限累计消耗 |
| 13031 | `SlotTaskProto` | `i32 TaskId`, `i32 TaskType`, `i32 Condition`, `i64 Progress`, `i64 Target`, `i32 Claimed`（0/1） |
| 13044 | `SlotProductProto` | `i32 ProductId`, `i32 Category`, `i64 PriceCents`, `i32 LimitCount`, `i32 Bought` |
| 13049 | `SlotOrderProto` | `str OrderId`, `i32 ProductId`, `i32 Paid`（0/1） |
| 13052 | `SlotMailProto` | `i64 MailId`, `str Title`, `str Body`, `i32 Read`, `i32 Claimed`, `SlotGrantProto[] Rewards` |
| 13063 | `SlotRankProto` | `i32 Place`, `i64 PlayerId`, `str Nickname`, `i64 Damage`, `i32 Rival`。玩家榜 `Rival = 0` |

### 4.1 登录与心跳

这些消息在 `Packages/cn.etetet.login/Proto/Login_C_10000.proto`。`Main2NetClient_Login`（10001/10002）是 Unity 进程内部消息，Cocos 不要发。`C2G_EnterMap` 是地图玩法，老虎机不要发。

| Opcode | 名称 | 方向 | 成员 |
| --- | --- | --- | --- |
| 10003 | `C2G_Ping` | C→S | `i32 RpcId` |
| 10004 | `G2C_Ping` | S→C | `i32 RpcId`, `i32 Error`, `str Message`, `i64 Time` 服务器毫秒 |
| 10005 | `C2R_Login` | C→Realm | `i32 RpcId`, `str Account`, `str Password` |
| 10006 | `R2C_Login` | Realm→C | `i32 RpcId`, `i32 Error`, `str Message`, `str Address`, `i64 Key`, `i64 GateId` |
| 10007 | `C2G_LoginGate` | C→Gate | `i32 RpcId`, `i64 Key`, `i64 GateId` |
| 10008 | `G2C_LoginGate` | Gate→C | `i32 RpcId`, `i32 Error`, `str Message`, `i64 PlayerId` |
| 10009 | `C2G_Logout` | C→Gate | `i32 RpcId` |
| 10010 | `G2C_Logout` | Gate→C | `i32 RpcId`, `i32 Error`, `str Message` |

`C2R_Login` 示例（`RpcId=1`，账号 `"g"`，密码空串）的 MemoryPack 体：

```text
03                // 3 个成员
01 00 00 00       // RpcId = 1
FE FF FF FF       // ~1，UTF-8 长度 1
01 00 00 00       // UTF-16 长度 1
67                // "g"
00 00 00 00       // Password = ""
```

Key 错误时 `G2C_LoginGate.Error = 100009001`（`ERR_ConnectGateKeyError`）。

### 4.2 档案、房间、旋转、推送

定义在 `Packages/cn.etetet.slotcore/Proto/Slot_C_12000.proto`。

| Opcode | 名称 | 成员 |
| --- | --- | --- |
| 12005 | `C2G_EnterSlot` | `i32 RpcId` |
| 12006 | `G2C_EnterSlot` | `i32 RpcId`, `i32 Error`, `str Message`, `i64 PlayerId`, `SlotGrantProto[] Wallet` |
| 12007 | `C2G_Spin` | `i32 RpcId`, `i32 RoomId`, `i32 StakeId`, `i32 LineCount`, `str ClientSpinId` |
| 12008 | `G2C_Spin` | `i32 RpcId`, `i32 Error`, `str Message`, `i64 Payout`, `i64 Damage`, `i32 StageId`, `i64 BossHp`, `i32 FreeSpins`, `i64 Seed`, `SlotStepProto[] Steps`, `SlotGrantProto[] Wallet`, `SlotGrantProto[] Grants` |
| 12010 | `C2G_GetProfile` | `i32 RpcId` |
| 12011 | `G2C_GetProfile` | `i32 RpcId`, `i32 Error`, `str Message`, `i64 PlayerId`, `str Account`, `str Nickname`, `str Avatar`, `i32 VipExp`, `i32 VipLevel`, `i32 RealNameVerified`, `i32 PhoneBound`, `i32 UnreadMail`, `i32 ClaimableTask`, `i32 Maintenance`, `SlotGrantProto[] Wallet` |
| 12012 | `C2G_Rename` | `i32 RpcId`, `str Nickname` |
| 12013 | `G2C_Rename` | `i32 RpcId`, `i32 Error`, `str Message`, `str Nickname` |
| 12014 | `C2G_EnterRoom` | `i32 RpcId`, `i32 RoomId` |
| 12015 | `G2C_EnterRoom` | `i32 RpcId`, `i32 Error`, `str Message`, `i32 RoomId`, `i32 StageId`, `i64 BossHp`, `i64 BossHpMax`, `i32 FreeSpins`, `i64 PublicStock`, `i64 RankPool`, `i64 JackpotPool`, `SlotStakeProto[] Stakes` |
| 12016 | `C2G_GetRoomSnapshot` | 与 `C2G_EnterRoom` 相同 |
| 12017 | `G2C_GetRoomSnapshot` | 与 `G2C_EnterRoom` 相同 |
| 12018 | `G2C_PushWallet` | `SlotGrantProto[] Wallet`。没有 RpcId |
| 12019 | `G2C_PushProgress` | `i32 RoomId`, `i32 StageId`, `i64 BossHp`, `i64 BossHpMax`, `i32 FreeSpins`, `i64 TotalDamage` |
| 12020 | `G2C_PushMail` | `i64 MailId`, `str Title` |
| 12021 | `G2C_PushNotice` | `i32 Kind`, `str Title`, `str Body` |
| 12022 | `G2C_PushKick` | `i32 Reason`, `str Message` |

`G2C_EnterRoom` 与 `G2C_GetRoomSnapshot` 字段相同。进房会在该房间所属进度组上创建进度（取该组第一关）。快照只读当前进度。`Stakes` 只含这个 `RoomId` 的档位。档位是否可押由服务端在 `C2G_Spin` 时按 VIP 与累计消耗判断；快照里的 `VipLevel`、`TotalCost` 给客户端做置灰。

`C2G_Spin` 示例：`RpcId=2, RoomId=101, StakeId=10101, LineCount=1, ClientSpinId="s1"`。对象头 `05`，然后四个 int32 与字符串 `"s1"`。

房间 101 最低档 `StakeId=10101`，`BetGold=300`。1 线时扣 300 金币。

改名需要当前 VIP 允许改名（`G2C_GetVipInfo.CanRename == 1`），否则 `200062003`。

### 4.3 运营

定义在 `Packages/cn.etetet.slotops/Proto/SlotOps_C_13000.proto`。全部是 Gate 上的会话 RPC。

| Opcode | 名称 | 请求额外字段 | 响应额外字段 |
| --- | --- | --- | --- |
| 13001/13002 | `C2G_ClaimTask` / `G2C_ClaimTask` | `i32 TaskId` | `SlotGrantProto[] Wallet` |
| 13003/13004 | `C2G_CreateOrder` / `G2C_CreateOrder` | `i32 ProductId`, `str OrderId` | `str OrderId` |
| 13005/13006 | `C2G_DevPay` / `G2C_DevPay` | `str OrderId` | `i32 VipLevel`, `SlotGrantProto[] Wallet` |
| 13007/13008 | `C2G_ClaimJubao` / `G2C_ClaimJubao` | 无 | `SlotGrantProto[] Wallet` |
| 13009/13010 | `C2G_RollDice` / `G2C_RollDice` | `i32 Steps`, `i32 Lucky`（非 0 表示幸运） | `i32 Cell`, `i32 Laps`, `SlotGrantProto[] Wallet` |
| 13011/13012 | `C2G_ClaimLap` / `G2C_ClaimLap` | `i32 Laps` | `SlotGrantProto[] Wallet` |
| 13013/13014 | `C2G_ClaimKing` / `G2C_ClaimKing` | `i32 TierId` | `SlotGrantProto[] Wallet` |
| 13015/13016 | `C2G_ClaimMail` / `G2C_ClaimMail` | `i64 MailId` | `SlotGrantProto[] Wallet` |
| 13017/13018 | `C2G_RedeemCode` / `G2C_RedeemCode` | `str Code` | `SlotGrantProto[] Wallet` |
| 13019/13020 | `C2G_ClaimPiggy` / `G2C_ClaimPiggy` | 无 | `SlotGrantProto[] Wallet` |
| 13021/13022 | `C2G_Bankruptcy` / `G2C_Bankruptcy` | 无 | `SlotGrantProto[] Wallet` |
| 13023/13024 | `C2G_SubmitIdentity` / `G2C_SubmitIdentity` | `str Name`, `str Number` | 无。开发桩，成功后 `RealNameVerified=1` |
| 13025/13026 | `C2G_BindPhone` / `G2C_BindPhone` | `str Number`, `str Code` | 无。开发桩，成功后 `PhoneBound=1` |
| 13027/13028 | `C2G_QueryRival` / `G2C_QueryRival` | `i32 Period`, `i32 Place` | `i64 RivalDamage` |
| 13029/13030 | `C2G_SetMaintenance` / `G2C_SetMaintenance` | `i32 Enabled`（0/1） | 无。开发桩，并给在线玩家推 `G2C_PushNotice` |
| 13032/13033 | `C2G_ListTasks` / `G2C_ListTasks` | 无 | `SlotTaskProto[] Tasks` |
| 13034/13035 | `C2G_GetJubao` / `G2C_GetJubao` | 无 | `i32 Level`, `i64 ReadyUnix`, `i64 Gold` |
| 13036/13037 | `C2G_GetDiceBoard` / `G2C_GetDiceBoard` | 无 | `i32 Cell`, `i32 Laps`, `i32 CellCount` |
| 13038/13039 | `C2G_GetKing` / `G2C_GetKing` | 无 | `i64 Damage` 累计伤害，即王座积分 |
| 13040/13041 | `C2G_GetPiggy` / `G2C_GetPiggy` | 无 | `i64 Piggy` 已计提、尚未领取 |
| 13042/13043 | `C2G_GetBankruptcy` / `G2C_GetBankruptcy` | 无 | `i32 Claims`, `i32 Limit`, `i64 Gold` |
| 13045/13046 | `C2G_ListProducts` / `G2C_ListProducts` | 无 | `SlotProductProto[] Products` |
| 13047/13048 | `C2G_GetVipInfo` / `G2C_GetVipInfo` | 无 | `i32 Level`, `i32 Exp`, `i32 CanRename`, `i32 CanIdle` |
| 13050/13051 | `C2G_ListOrders` / `G2C_ListOrders` | 无 | `SlotOrderProto[] Orders` |
| 13053/13054 | `C2G_ListMails` / `G2C_ListMails` | 无 | `SlotMailProto[] Mails`（已删除的不返回） |
| 13055/13056 | `C2G_ReadMail` / `G2C_ReadMail` | `i64 MailId` | 无 |
| 13057/13058 | `C2G_ClaimAllMails` / `G2C_ClaimAllMails` | 无 | `SlotGrantProto[] Wallet` |
| 13059/13060 | `C2G_DeleteMail` / `G2C_DeleteMail` | `i64 MailId` | 无 |
| 13061/13062 | `C2G_DevSendMail` / `G2C_DevSendMail` | `str Title`, `str Body`, `i32 ItemId`, `i64 Count` | `i64 MailId`。同时推 `G2C_PushMail` |
| 13064/13065 | `C2G_ListRank` / `G2C_ListRank` | `i32 Period` | `SlotRankProto[] Rows` |

每个请求的第一个成员都是 `RpcId`。每个响应的前三个成员都是 `RpcId`、`Error`、`Message`，然后再写“响应额外字段”。

商城联调：`C2G_ListProducts` 取 `ProductId` → `C2G_CreateOrder`（`OrderId` 用客户端 uuid）→ `C2G_DevPay`。支付是开发桩，不会访问真实渠道。`G2C_DevPay.VipLevel` 是支付后的等级。

聚宝领取要求累计充值（分）达到该档门槛，且 `ReadyUnix` 已到。门槛来自服务端配置，客户端用 `C2G_GetJubao` 的 `Level` / `ReadyUnix` / `Gold` 展示，失败看错误码。

骰子棋盘格数以 `G2C_GetDiceBoard.CellCount` 为准（配置为 32 格时该值为 32）。`Cell` 是当前格。圈数奖励用 `C2G_ClaimLap`。

排行 `C2G_ListRank` 的 `Period` 传 1，得到已落库的玩家伤害榜，最多 50 条。对手名次不是这份列表。对手伤害只通过 `C2G_QueryRival(Period, Place)` 拿到一个 `RivalDamage` 数字。对手策略表不会下发。

邮件：`C2G_ClaimMail` 领取单封附件；`C2G_ClaimAllMails` 领取所有未领且未删的邮件；已领再领返回“已领取”。删除是软删。

### 4.4 错误码

`Error = 0` 成功。

| 值 | 名称 | 含义 |
| --- | --- | --- |
| 100009001 | `ERR_ConnectGateKeyError` | Gate Key 无效或过期 |
| 200061001 | `ERR_SlotRoomMissing` | 房间不存在 |
| 200061002 | `ERR_SlotStakeMissing` | 押注档不存在或不属于该房间 |
| 200061003 | `ERR_SlotLineCount` | 连线数超出 1..MaxLines |
| 200061004 | `ERR_SlotStageCleared` | 当前进度组没有下一关 |
| 200061005 | `ERR_SlotNotEnoughGold` | 金币不足 |
| 200061006 | `ERR_SlotFreeLocked` | 免费旋转中更换了档位或线数 |
| 200061007 | `ERR_SlotMaintenance` | 维护中 |
| 200061008 | `ERR_SlotPlayerMissing` | 还没有 `EnterSlot`，或玩家未创建 |
| 200062001 | `ERR_OpsMissing` | 目标不存在（任务、邮件、商品、兑换码、档位） |
| 200062002 | `ERR_OpsAlready` | 已经领取或已经购买到上限 |
| 200062003 | `ERR_OpsNotReady` | 条件未满足（VIP、充值、冷却、进度） |
| 200062004 | `ERR_OpsNotEnough` | 货币或次数不足 |
| 200062005 | `ERR_OpsClosed` | 活动关闭 |
| 200062006 | `ERR_OpsQuota` | 王座名额已满 |
| 200062007 | `ERR_OpsLimit` | 次数或上限 |

位置与 RPC 基础设施还可能返回 `100001003`（消息超时）等核心错误。客户端把非 0 都当失败展示，上面这张表做文案。

---

## 5. Luban 与客户端

服务端导出命令只有：

```powershell
dotnet ./Bin/ET.ExcelExporter.dll
```

没有面向 Cocos 的 JSON 导出。客户端需要的运行时数据全部来自协议：

- 押注档、Boss、奖池：`G2C_EnterRoom`
- 任务：`G2C_ListTasks`
- 商品与 VIP：`G2C_ListProducts`、`G2C_GetVipInfo`
- 活动状态：聚宝、骰子、王座、储蓄罐、破产补助的 Get 消息

表文件在 `Packages/cn.etetet.slotcore/Luban/Config/Datas` 与 `Packages/cn.etetet.slotops/Luban/Config/Datas`，仅服务端加载。

**只允许服务端读取、禁止放进客户端包的表：`OpsRival.xlsx`（`OpsRivalConfig`）。** 它描述对手排行的步长和封顶。客户端只能调用 `C2G_QueryRival`。其余 `Ops*` 与 `Slot*` 表同样不要打进客户端；列出来是为了让实现者知道不要再导一份。

服务端表名：

- slotcore：`SlotFace` `SlotFreeSpin` `SlotHit` `SlotJackpot` `SlotMachine` `SlotPattern` `SlotPay` `SlotPayline` `SlotPolicy` `SlotRoom` `SlotRule` `SlotStage` `SlotStake` `SlotSymbol`
- slotops：`OpsActivity` `OpsDice` `OpsJubao` `OpsKing` `OpsLap` `OpsProduct` `OpsRank` `OpsRedeem` `OpsRelief` `OpsReward` `OpsRival` `OpsTask` `OpsVip`

进度按 **进度组** 落库（`slot_progress` 主键是账号 + `progress_group`），不是按房间。201/202/203 共用进度组 2。主题 3（房间 301）从关卡 3001 起，不与主题 1 共关键卡。

---

## 6. 联调验收

前置：PostgreSQL 已监听 `127.0.0.1:5432`，库 `slot` 可用。服务端 `dotnet ./Bin/ET.App.dll --Console=1`。

1. 连接 `ws://127.0.0.1:31101/ws`，发 `C2R_Login`，账号用一个新的 uuid。确认 `R2C_Login.Error = 0`，`Address` 端口是 30102 或 30103。
2. 连接对应的 `ws://127.0.0.1:31102/ws`（端口 + 1000），`C2G_LoginGate` 后 `C2G_EnterSlot`。`Wallet` 含 `ItemId=1, Count=15000`。
3. `C2G_GetProfile`：`Account` 等于登录字符串，`UnreadMail=0`，`Maintenance=0`。
4. `C2G_EnterRoom RoomId=101`：`StageId` 有值，`BossHpMax > 0`，`Stakes` 含 `StakeId=10101`。
5. `C2G_DevSendMail`，`Title="gold"`，`ItemId=2`，`Count=100000`。先收到 `G2C_PushMail`，响应里有 `MailId`。`C2G_ClaimMail` 后钱包出现金币。
6. `C2G_Spin`，`RoomId=101`，`StakeId=10101`，`LineCount=1`，`ClientSpinId` 用新 uuid。`Error=0`。在响应之前或同一连接上收到 `G2C_PushWallet` 与 `G2C_PushProgress`。
7. 立刻用同一个 `ClientSpinId` 再转一次。`Payout`、`Damage`、`Seed` 与第一次相同，金币不再减少。
8. `C2G_GetPiggy` 的 `Piggy` 大于 0（押注 300 的千分之 5）。`C2G_ListTasks` 里条件 1 或 8 的 `Progress` 增加了本次 `Damage`（伤害为 0 的局不增加）。`C2G_GetKing.Damage` 等于累计伤害。`C2G_ListRank Period=1` 能看到该账号。
9. `C2G_QueryRival Period=1 Place=1` 返回 `RivalDamage > 0`。客户端包内不存在 `OpsRival` 表。
10. 停掉 `ET.App`，再启动。用同一个账号从第 1 步重登。`GetProfile` 的昵称、钱包、VIP、邮件与停机前一致。`EnterRoom` 的 `StageId` 与 `BossHp` 与停机前一致。再用第 6 步的 `ClientSpinId` 旋转，结果仍与第一次相同，金币不变。
11. 数据库核对：

```powershell
$env:PGPASSWORD = "slot"
psql -h 127.0.0.1 -U slot -d slot -c "SELECT account, nickname FROM slot_account"
psql -h 127.0.0.1 -U slot -d slot -c "SELECT account, client_spin_id, bet, win, damage FROM slot_spin"
```

`slot_spin` 里该 `client_spin_id` 只有一行。

自动化覆盖（不替代上面的手工联调，但证明落库和帧字节）：

```powershell
dotnet test ET.sln -c Debug
```

其中 `ET.Slot.IntegrationTests` 会连本机 PostgreSQL：建表、写入账号/进度/账本/旋转、用新的连接再读出来，并断言 `C2G_EnterSlot` 的帧字节。

---

## 7. 实现时不要做的事

- 不要用 protobuf.js 解析外网 body。`.proto` 只提供字段顺序。
- 不要连接 Slot 场景，也不要发 Opcode ≥ 20000 的内网消息。
- 不要把 `PlayerId` 当存档键。
- 不要在免费旋转中改 `StakeId` 或 `LineCount`。
- 不要把 `OpsRival` 或任何 Luban xlsx 打进 Cocos 资源。
- 旋转重试不要更换 `ClientSpinId`。
