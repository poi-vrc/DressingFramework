# DressingFramework - DK
A framework that assembles DressingTools and provides interfaces for third-party developers.

## API Stability

**Unstable** but most features are usable.

## Hooks

DressingFramework categories hooks into two levels and four types:

- Cabinet-level
    - Cabinet Hook
    - Cabinet Module
- Wearable-level
    - Wearable Hook
    - Wearable Module

### Difference of hooks and modules

**Hooks** do not have configurations stored in the wearable configuration. They process avatars and wearables using the two contexts given only.

```csharp
// cabinet hook
public abstract bool Invoke(ApplyCabinetContext cabCtx);
// wearable hook
public abstract bool Invoke(ApplyCabinetContext cabCtx, ApplyWearableContext wearCtx);
```

**Modules** are actually just hooks, but they have configurations stored in the wearable configuration and will be passed on apply.

```csharp
// cabinet module
public abstract bool Invoke(ApplyCabinetContext cabCtx, ReadOnlyCollection<CabinetModule> modules, bool isPreview);
// wearable module
public abstract bool Invoke(ApplyCabinetContext cabCtx, ApplyWearableContext wearCtx, ReadOnlyCollection<WearableModule> modules, bool isPreview);
```

### Cabinet-level

At cabinet-level, hooks and modules are **only executed once on stage start or end** (defined via `CabinetHookStageRunOrder`).

### Wearable-level

At wearable-level, hooks and modules will be **executed on each wearable once.**

## Execution Constraints

DressingFramework schedules hooks and modules by constructing a dependency graph using the execution constraints defined by hooks and modules.
The order of execution is obtained by performing a topological sort on the dependency graph.

### Stages

You should categorize your hooks and modules into these stages for better management with other hooks and modules.

    - Pre
        - Generic pre-stage. The earliest stage to execute hooks.
        - You are preferred to categorize your hook instead of using this generic stage.
    - Analyzing
        - Analyzing stage. Scanning, generation, cloning are done in this stage.
    - Transpose
        - Transpose stage. Mapping, animation etc. general stuff are done in this stage.
    - Integration
        - Integration stage. Integration-specific hooks and modules are invoked in this stage.
    - Optimization
        - Optimization stage
    - Post
        - Generic post-stage. The latest stage to execute hooks.
        - You are preferred to categorize your hook instead of using this generic stage.

### Cabinet hooks and modules

`CabinetHookBase` and `CabinetModuleProviderBase` consist of a short-hand function `ApplyAtStage` for creating constraints easily.

- `CabinetHookStageRunOrder`
    - Use `CabinetHookStageRunOrder.Before` to run your cabinet hooks and modules before the stage
    - Use `CabinetHookStageRunOrder.After` to run your cabinet hooks and modules after the stage

The identifier defaults to use the class fullname if you do not override the `Identifier` property.

```csharp
public override CabinetApplyConstraint Constraint =>
    ApplyAtStage(CabinetApplyStage.Transpose, CabinetHookStageRunOrder.Before)
        .BeforeCabinetHook(typeof(MyCabinetHook));
        .BeforeCabinetModule("com.example.modules.cabinet.my-some-module")
        .AfterCabinetHook("com.example.hooks.cabinet.my-some-hook-identifier");
        .AfterCabinetModule("Example.Hooks.MySomeCabinetHook");
        .Build();
```

### Wearable hooks and modules

`WearableHookBase` and `WearableModuleProviderBase` consist of a short-hand function `ApplyAtStage` for creating constraints easily.

The identifier defaults to use the class fullname if you do not override the `Identifier` property.

```csharp
public override WearableApplyConstraint Constraint =>
    ApplyAtStage(CabinetApplyStage.Optimization)
        .BeforeWearableHook(typeof(MyWearableHook));
        .BeforeWearableModule("com.example.modules.wearble.my-some-module")
        .AfterWearableHook("com.example.hooks.wearable.my-some-hook-identifier");
        .AfterWearableModule("Example.Hooks.MySomeWearableHook");
        .Build();
```

## Plugins

To register your hooks and modules, you need to define a plugin by inheriting the `PluginBase` base class.
The identifier defaults to use the class fullname if you do not override the `Identifier` property.

```csharp
internal class MyPlugin : PluginBase
{
    public override string FriendlyName => "My Great Plugin";
    public override ExecutionConstraint Constraint => ExecutionConstraint.Empty;

    public override void OnEnable()
    {
        // will be called on enable
        RegisterCabinetHook(new MyCabinetHook());
        RegisterWearableHook(new MyWearableHook());
        RegisterCabinetModuleProvider(new MyCabinetModuleProvider());
        RegisterWearableModuleProvider(new MyWearableModuleProvider());
    }

    public override void OnDisable()
    {
        // will be called on disable
    }
}
```

## Documentation and samples

More will be released soon. API reference: https://poi-vrc.github.io/DressingFramework

## License

This project is licensed under the LGPLv3 License. [tl;dr](https://tldrlegal.com/license/gnu-lesser-general-public-license-v3-(lgpl-3))
