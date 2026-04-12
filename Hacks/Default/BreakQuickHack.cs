using ThunderRoad;

namespace QuickHack.Hacks.Default;

/// <summary>
/// Destroy a breakable object.
/// </summary>
public class BreakQuickHack : ComponentQuickHack<Item>
{
    /// <inheritdoc/>
    public override string Name { get; } = "Break";

    /// <inheritdoc/>
    public override bool CanHack(Item target) => target.breakable != null && target.breakable.CanBeBroken();

    /// <inheritdoc/>
    public override void Hack(Item target) => target.breakable.Break();
}
