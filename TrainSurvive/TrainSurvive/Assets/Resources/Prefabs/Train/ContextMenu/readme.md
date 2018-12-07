# 上下文菜单Prefabs

该目录下有两个Prefab，一个名为Canvas，一个名为Button。详细实现见`02.Scripts/Train/ContextMenu`。

## Canvas

这个是右键菜单的底部面板，有以下几个注意事项：

- 务必绑上ContextMenuClose脚本以实现自动关闭功能。
- Canvas Render Mode需要是World Space的。
- Rect Transform Pivot 需要是 (0, 1)。
- 长宽无所谓，均由Button决定。

Canvas子物体的面板有几个注意事项：

- 自动适配不会更改Panel的长宽，所以可以的话尽量让Panel的长宽随Canvas变化，如将RectTransform设为Stretch。
- 需要绑定Vertical Layout Group组件，Padding和Spacing随意设置。Child Controls Size和Child Force Expand全勾上。

## Button

按钮样式，注意事项：

- Button的长宽决定了菜单的大小。