# RabbitContainer

这个插件是一个插槽容器，为新的进程外扩展提供命令放置的 group。

## 占用的位置

| 位置 | Group 名称 | VS 原生菜单 ID |
|------|-----------|---------------|
| 编辑器右键菜单 | EditorContextMenuGroup | IDM_VS_CTXT_CODEWIN |
| 文件右键菜单 | FileItemContextMenuGroup | IDM_VS_CTXT_ITEMNODE |
| 文件夹右键菜单 | FolderContextMenuGroup | IDM_VS_CTXT_FOLDERNODE |
| 项目右键菜单 | ProjectContextMenuGroup | IDM_VS_CTXT_PROJNODE |
| 解决方案右键菜单 | SolutionContextMenuGroup | IDM_VS_CTXT_SOLNNODE |

## 每个位置的变量

| Group 名称 | GUID | ID 值 |
|-----------|------|------|
| EditorContextMenuGroup | `e85d182a-f11b-4b4f-8fdc-cdfafe7b0d25` | `0x0001` |
| FileItemContextMenuGroup | `e85d182a-f11b-4b4f-8fdc-cdfafe7b0d25` | `0x0002` |
| FolderContextMenuGroup | `e85d182a-f11b-4b4f-8fdc-cdfafe7b0d25` | `0x0003` |
| ProjectContextMenuGroup | `e85d182a-f11b-4b4f-8fdc-cdfafe7b0d25` | `0x0004` |
| SolutionContextMenuGroup | `e85d182a-f11b-4b4f-8fdc-cdfafe7b0d25` | `0x0005` |

## 新插件怎么使用

在你的进程外扩展中使用 `CommandPlacement.VsctParent()` 方法来放置命令：

```csharp
// 放置到编辑器右键菜单
CommandPlacement.VsctParent(
    new Guid("e85d182a-f11b-4b4f-8fdc-cdfafe7b0d25"),
    0x0001,
    0  // priority
);

// 放置到文件右键菜单
CommandPlacement.VsctParent(
    new Guid("e85d182a-f11b-4b4f-8fdc-cdfafe7b0d25"),
    0x0002,
    0
);

// 放置到文件夹右键菜单
CommandPlacement.VsctParent(
    new Guid("e85d182a-f11b-4b4f-8fdc-cdfafe7b0d25"),
    0x0003,
    0
);

// 放置到项目右键菜单
CommandPlacement.VsctParent(
    new Guid("e85d182a-f11b-4b4f-8fdc-cdfafe7b0d25"),
    0x0004,
    0
);

// 放置到解决方案右键菜单
CommandPlacement.VsctParent(
    new Guid("e85d182a-f11b-4b4f-8fdc-cdfafe7b0d25"),
    0x0005,
    0
);
```

## 说明

- 所有 group 的 priority 都设置为 `0x0000`，会尽量显示在菜单顶部
- GUID 值是固定的：`e85d182a-f11b-4b4f-8fdc-cdfafe7b0d25`
- ID 值从 `0x0001` 到 `0x0005`，对应不同的菜单位置
