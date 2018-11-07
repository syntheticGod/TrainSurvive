# 各种设施的Prefab

这里存放各种设施的Prefab，说明如下：

- 需要有SpriteRenderer， Collider2D组件
- 编写一个脚本继承自Facility类，实现抽象方法，并绑定在上面。
- Inspector中指定可以放置该设施的Layer以及Indicator。
- 将该物体Layer设置为Facility。
- Facility UI Prefab为查看界面UI，会在点击查看后自动附加到Canvas下。