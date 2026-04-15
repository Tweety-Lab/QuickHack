# Quick Hack
A Blade & Sorcery mod that adds Quick Hacking from Cyberpunk 2077.

## Making your own Quick Hack
This mod is designed to support programming your own custom hacks. A hack looks a little bit like this:
```csharp
/// <summary>
/// Disable movement.
/// </summary>
public class CrippleMovementQuickHack : ComponentQuickHack<Creature>
{
    /// <inheritdoc/>
    public override string Name { get; } = "Cripple Movement";

    /// <inheritdoc/>
    public override string Icon { get; } = "QuickHack.Icons.CrippleMovement";

    /// <inheritdoc/>
    public override bool CanHack(Creature target) => !target.isPlayer;

    /// <inheritdoc/>
    public override void Hack(Creature target)
    {
        target.locomotion.enabled = false;
        CoroutineRunner.Instance.PlayAfterDelay(() => target.locomotion.enabled = true, 5f);
    }
}
```

You can then register that Quick Hack in a Thunderscript like so:
```csharp
public class QuickHackThunderScript : ThunderScript
{
    /// <inheritdoc/>
    public override void ScriptLoaded(ModManager.ModData modData)
    {
        base.ScriptLoaded(modData);

        QuickHackLogicAbility.RegisteredQuickHackTypes.Add(typeof(MyAwesomeQuickHack));
    }
}
```
