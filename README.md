# ET11

## 开源包

| 包名 | 详细内容 |
|------|------|
| cn.etetet.harness | ai各种skills跟分析器，包括et-code, et-async, et-excel, et-luban, et-build, et-test, et-tdd等skills，还有包分析器, await分析器等等分析器, 主要用于严格限制ai行为 |
| cn.etetet.unitybridge | 该包是Unity Skills, 实现了各种Unity操作，并且极其方便扩展，提出需求让ai实现即可。目前注册的68个指令全部包括：状态与生命周期 Ping、HostState、Compile、Refresh、RegenProject、EnterPlay、ExitPlay、Reload、EditorGetStateRequest、EditorPauseRequest、EditorUndoRequest、EditorRedoRequest；资源 AssetSearchRequest、AssetFindRequest、AssetGetPathRequest、AssetLoadRequest、AssetImportRequest、AssetRefreshRequest、AssetReadTextRequest；场景 SceneGetHierarchyRequest、SceneGetActiveRequest、SceneLoadRequest、SceneSaveRequest、SceneNewRequest；选择集 SelectionGetRequest、SelectionSetRequest、SelectionAddRequest、SelectionRemoveRequest、SelectionClearRequest；GameObject GameObjectCreateRequest、GameObjectDestroyRequest、GameObjectDuplicateRequest、GameObjectFindRequest、GameObjectGetInfoRequest、GameObjectRenameRequest、GameObjectSetActiveRequest；Transform TransformGetRequest、TransformSetPositionRequest、TransformSetRotationRequest、TransformSetScaleRequest、TransformSetParentRequest、TransformLookAtRequest、TransformResetRequest、TransformSetSiblingIndexRequest；Inspector InspectorGetComponentsRequest、InspectorGetPropertiesRequest、InspectorGetPropertyRequest、InspectorFindPropertyRequest、InspectorSetPropertyRequest、InspectorSetPropertiesRequest、InspectorAddComponentRequest、InspectorRemoveComponentRequest；Prefab PrefabInstantiateRequest、PrefabSaveRequest、PrefabUnpackRequest、PrefabGetInfoRequest、PrefabGetHierarchyRequest、PrefabApplyRequest；GameView GameViewGetResolutionRequest、GameViewListResolutionsRequest、GameViewSetResolutionRequest；其它 ConsoleGetLogsRequest、EditorLogRequest、ScreenshotCaptureRequest、UnityTestRunRequest、MenuItemExecuteRequest、BatchExecuteRequest、TestEcho |
| cn.etetet.test | 测试框架，可以模拟整个游戏环境，实现整条客户端跟服务器交互。比如组队功能，可以写一个test，里面创建5个客户端机器人，然后让机器人发送消息实现组队，从而可以测试整个客户端跟服务端的组队逻辑。有了test框架，ai的工作检查变得极其轻松，只需要让ai写test，然后 review自己的test，确保test执行通过即可。不仅成实现逻辑层Test，UI Test也完全可以实现 |
| cn.etetet.behaviortree | 行为树包，提供BTNode、Sequence、Selector、Not、Condition、Action等行为树节点定义与运行时支持，并带有Unity编辑器可视化编辑、节点参数配置、运行路径调试和配置代码导出能力，适合实现怪物AI、任务流程、技能释放、复杂条件判断等可配置决策逻辑。 |
| cn.etetet.servicediscovery | 服务发现包，提供服务注册、注销、查询、订阅、心跳、租约和变更通知能力，进程级ServiceDiscoveryAgent会把请求转发到当前服务发现主节点，并支持主节点切换、故障恢复和多客户端并发访问，适合多进程多服务器架构下动态发现服务 |
| cn.etetet.conditionexpr | ConditionExpr包，可以把Excel/Luban中填写的条件表达式编译成行为树运行，支持数值比较、&&、\|\|、!、括号、错误码绑定、多owner key和专用条件节点，适合副本进入条件、奖励领取条件、任务接取条件、功能开启条件等策划可配置条件判断 |
| cn.etetet.spell | Spell包，提供技能与Buff系统，包含Spell/Buff配置模型、运行时施法逻辑、客户端表现组件和相关Unity编辑器工具，并复用行为树配置导出链路，适合实现技能释放、Buff结算、技能表现和复杂战斗流程配置。Unity菜单 ET->Spell->Spell Editor提供集成化的编辑器。甚至直接让ai自己配置即可 |
| cn.etetet.aspire | Aspire包，整合ET框架与.NET Aspire，会根据StartConfig为每个ET进程和副本创建Aspire服务，自动传递SceneName、StartConfig、内外网端口等启动参数，并接入OTLP/OpenTelemetry，适合分布式应用编排、运行观测和监控 |

支持 game develop AI harness。客户端与服务端使用同一套 C#，可以直接编写双端 test，打通前后端逻辑，由测试完成联调。逻辑开发剩余的主要工作是 UI。

使用请参考 [Book/1.1Running Guide.md](Book/1.1Running%20Guide.md)

# 文档

- [运行指南（英文）](Book/1.1Running%20Guide.md)
- [运行指南](./Book/1.1运行指南.md)
- [ET Package 制作指南](./Book/8.1ET%20Package制作指南.md)
- [ET Package 目录](./Book/8.2ET%20Package目录.md)
- [包更新](./Book/8.3包更新.md)
- [ET Packages](https://github.com/orgs/ET-Packages/packages)
- [分析器说明](https://www.yuque.com/u28961999/yms0nt/)

# Benchmark

100W Ping Pong 平均耗时4秒左右，平均每秒收发20W的消息。这个网络性能远远超过主线程的需求，大家可以自己测试一下，测试方法：
Unity Menu->ServerTools select Benchmark, Start Watcher。然后在Logs目录，打开Debug日志等一会所有连接完成就能看到下面的日志了。  
2022-12-02 22:19:48.9837 (C2G_BenchmarkHandler.cs:13) benchmark count: 1000001  
2022-12-02 22:19:53.4621 (C2G_BenchmarkHandler.cs:13) benchmark count: 2000001  
2022-12-02 22:19:57.0416 (C2G_BenchmarkHandler.cs:13) benchmark count: 3000001  
2022-12-02 22:20:00.6186 (C2G_BenchmarkHandler.cs:13) benchmark count: 4000001  
2022-12-02 22:20:04.1384 (C2G_BenchmarkHandler.cs:13) benchmark count: 5000001  
2022-12-02 22:20:08.2236 (C2G_BenchmarkHandler.cs:13) benchmark count: 6000001  
2022-12-02 22:20:12.2842 (C2G_BenchmarkHandler.cs:13) benchmark count: 7000001  
2022-12-02 22:20:15.8544 (C2G_BenchmarkHandler.cs:13) benchmark count: 8000001  
2022-12-02 22:20:19.4085 (C2G_BenchmarkHandler.cs:13) benchmark count: 9000001  
2022-12-02 22:20:24.2969 (C2G_BenchmarkHandler.cs:13) benchmark count: 10000001  
2022-12-02 22:20:41.1448 (C2G_BenchmarkHandler.cs:13) benchmark count: 11000001  
2022-12-02 22:20:44.7174 (C2G_BenchmarkHandler.cs:13) benchmark count: 12000001  
2022-12-02 22:20:48.3188 (C2G_BenchmarkHandler.cs:13) benchmark count: 13000001  
2022-12-02 22:20:51.7793 (C2G_BenchmarkHandler.cs:13) benchmark count: 14000001  
2022-12-02 22:20:55.3379 (C2G_BenchmarkHandler.cs:13) benchmark count: 15000001  
2022-12-02 22:20:58.8810 (C2G_BenchmarkHandler.cs:13) benchmark count: 16000001  
2022-12-02 22:21:02.5156 (C2G_BenchmarkHandler.cs:13) benchmark count: 17000001  
2022-12-02 22:21:06.0132 (C2G_BenchmarkHandler.cs:13) benchmark count: 18000001  
2022-12-02 22:21:09.5320 (C2G_BenchmarkHandler.cs:13) benchmark count: 19000001  

# ET的介绍：

ET是一个开源的游戏客户端（基于unity3d）服务端双端框架，服务端是使用C# .net core开发的分布式游戏服务端，其特点是开发效率高，性能强，双端共享逻辑代码，客户端服务端热更机制完善，同时支持可靠udp tcp websocket协议，支持服务端3D recast寻路等等  

# ET的功能：

### 1.可用VS单步调试的分布式服务端，N变1  

一般来说，分布式服务端要启动很多进程，一旦进程多了，单步调试就变得非常困难，导致服务端开发基本上靠打log来查找问题。平常开发游戏逻辑也得开启一大堆进程，不仅启动慢，而且查找问题及其不方便，要在一堆堆日志里面查问题，这感觉非常糟糕，这么多年也没人解决这个问题。ET框架使用了类似守望先锋的组件设计，所有服务端内容都拆成了一个个组件，启动时根据服务器类型挂载自己所需要的组件。这有点类似电脑，电脑都模块化的拆成了内存，CPU，主板等等零件，搭配不同的零件就能组装成一台不同的电脑，例如家用台式机需要内存，CPU，主板，显卡，显示器，硬盘。而公司用的服务器却不需要显示器和显卡，网吧的电脑可能不需要硬盘等。正因为这样的设计，ET框架可以将所有的服务器组件都挂在一个服务器进程上，那么这个服务器进程就有了所有服务器的功能，一个进程就可以作为整组分布式服务器使用。这也类似电脑，台式机有所有的电脑组件，那它也完全可以当作公司服务器使用，也可以当作网吧电脑。  

### 2.随意可拆分功能的分布式服务端，1变N  

分布式服务端要开发多种类型的服务器进程，比如Login server，gate server，battle server，chat server friend server等等一大堆各种server，传统开发方式需要预先知道当前的功能要放在哪个服务器上，当功能越来越多的时候，比如聊天功能之前在一个中心服务器上，之后需要拆出来单独做成一个服务器，这时会牵扯到大量迁移代码的工作，烦不胜烦。ET框架在平常开发的时候根本不太需要关心当前开发的这个功能会放在什么server上，只用一个进程进行开发，功能开发成组件的形式。发布的时候使用一份多进程的配置即可发布成多进程的形式，是不是很方便呢？随便你怎么拆分服务器。只需要修改极少的代码就可以进行拆分。不同的server挂上不同的组件就行了嘛！  

### 3.跨平台的分布式服务端  

ET框架使用C#做服务端，现在C#是完全可以跨平台的，在linux上安装.netcore，即可，不需要修改任何代码，就能跑起来。性能方面，现在.netcore的性能非常强，比lua，python，js什么快的多了。做游戏服务端完全不在话下。平常我们开发的时候用VS在windows上开发调试，发布的时候发布到linux上即可。ET框架还提供了一键同步工具，打开unity->tools->rsync同步，即可同步代码到linux上  

```bash
./Run.sh Config/StartConfig/192.168.12.188.txt 
```

即可编译启动服务器。  

### 4.提供协程支持  

C#天生支持异步变同步语法 async和await，比lua，python的协程强大的多，新版python以及javascript语言甚至照搬了C#的协程语法。分布式服务端大量服务器之间的远程调用，没有异步语法的支持，开发将非常麻烦。所以java没有异步语法，做单服还行，不适合做大型分布式游戏服务端。例如：  

```c#
// 发送C2R_Ping并且等待响应消息R2C_Ping
R2C_Ping pong = await session.Call(new C2R_Ping()) as R2C_Ping;
Log.Debug("收到R2C_Ping");

// 向mongodb查询一个id为1的Player，并且等待返回
Player player = await Game.Scene.GetComponent<DBProxyComponent>().Query<Player>(1);
Log.Debug($"打印player name: {player.Name}")
```

可以看出，有了async await，所有的服务器间的异步操作将变得非常连贯，不用再拆成多段逻辑。大大简化了分布式服务器开发  

### 5.提供类似erlang的actor消息机制  

erlang语言一大优势就是位置透明的消息机制，用户完全不用关心对象在哪个进程，拿到id就可以对对象发送消息。ET框架也提供了actor消息机制，实体对象只需要挂上MailBoxComponent组件，这个实体对象就成了一个Actor，任何服务器只需要知道这个实体对象的id就可以向其发送消息，完全不用关心这个实体对象在哪个server，在哪台物理机器上。其实现原理也很简单，ET框架提供了一个位置服务器，所有挂载MailBoxComponent的实体对象都会将自己的id跟位置注册到这个位置服务器，其它服务器向这个实体对象发送消息的时候如果不知道这个实体对象的位置，会先去位置服务器查询，查询到位置再进行发送。

### 6.提供服务器不停服动态更新逻辑功能  

热更是游戏服务器不可缺少的功能，ET框架使用的组件设计，可以做成守望先锋的设计，组件只有成员，无方法，将所有方法做成扩展方法放到热更dll中，运行时重新加载dll即可热更所有逻辑。

### 7.客户端使用C#热更新，热更新一键切换  

可以使用csharp.lua或者ILRuntime稍加改造即可做客户端热更。再也不用使用狗屎lua了，客户端可以实现所有逻辑热更新，包括协议，config，ui等等。  

### 8.客户端热重载  

开发不用重启客户端即可修改客户端逻辑代码，开发极其方便  

### 9.客户端服务端用同一种语言，并且共享代码  

下载ET框架，打开服务端工程，可以看到服务端引用了客户端很多代码，通过引用客户端代码的方式实现了双端共享代码。例如客户端服务端之间的网络消息两边完全共用一个文件即可，添加一个消息只需要修改一遍。  

### 10.KCP ENET TCP Websocket协议无缝切换  

ET框架不但支持TCP，而且支持可靠的UDP协议（ENET跟KCP），ENet是英雄联盟所使用的网络库，其特点是快速，并且网络丢包的情况下性能也非常好，这个我们做过测试TCP在丢包5%的情况下，moba游戏就卡的不行了，但是使用ENet，丢包20%仍然不会感到卡。非常强大。框架还支持使用KCP协议，KCP也是可靠UDP协议，据说比ENET性能更好，使用kcp请注意，需要自己加心跳机制，否则20秒没收到包，服务端将断开连接。协议可以无缝切换。  

### 11. 3D Recast寻路功能  

可以Unity导出场景数据，给服务端做recast寻路。做MMO非常方便，demo演示了服务端3d寻路功能  

### 12. 服务端支持repl，也可以动态执行一段新代码  

这样就可以打印出进程中任何数据，大大简化了服务端查找问题的难度，开启repl方法，直接在console中输入repl回车即可进入repl模式  

### 13.提供客户端机器人框架支持  

几行代码即可创建机器人登录游戏。机器人压测轻而易举，机器人跟正常的玩家完全一样，上线前用机器人做好压测，大大降低上线崩溃几率   

### 14.AI框架  

ET的AI框架让AI编写比UI还简单     

### 15.测试用例框架  

跟市面上的测试用例不同，ET的测试用例都是一个完整的游戏环境，针对协议级别，不需要搞各种接口去mock。写起来非常快速    

### 16.还有很多很多功能，我就不详细介绍了  

a.及其方便检查CPU占用和内存泄漏检查，vs自带分析工具，不用再为性能和内存泄漏检查而烦恼  
b.使用NLog库，打log及其方便，平常开发时，可以将所有服务器log打到一个文件中，再也不用一个个文件搜索log了  
c.统一使用Mongodb的bson做序列化，消息和配置文件全部都是bson或者json，并且以后使用mongodb做数据库，再也不用做格式转换了。  
d.提供一个同步工具  

ET框架是一个强大灵活的分布式服务端架构，完全可以满足绝大部分大型游戏需求。使用这套框架，客户端开发者就可以自己完成双端开发，节省大量人力物力，节省大量沟通时间。  
