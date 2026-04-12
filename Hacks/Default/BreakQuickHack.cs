using ThunderRoad;
using UnityEngine;

namespace QuickHack.Hacks.Default;

/// <summary>
/// Destroy a breakable object.
/// </summary>
public class BreakQuickHack : ComponentQuickHack<Item>
{
    /// <inheritdoc/>
    public override string Name { get; } = "Break";

    /// <inheritdoc/>
    public override bool CanHack(Item target) => target.breakable != null && target.breakable.CanBeBroken() && !target.breakable.IsBroken;

    /// <inheritdoc/>
    public override void Hack(Item target) => target.breakable.Explode(target.totalCombinedMass, target.Center, 1f, 0f, ForceMode.Impulse);
}
