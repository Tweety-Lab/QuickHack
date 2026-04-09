using System;
using ThunderRoad;
using UnityEngine;

namespace QuickHack.Hacks;

/// <summary>
/// Explode a breakable object.
/// </summary>
public class BreakQuickHack : BaseQuickHack
{
    /// <inheritdoc/>
    public override string Name { get; } = "Break";

    /// <inheritdoc/>
    public override bool CanHack(GameObject target)
    {
        Item? item = target.GetComponentInParent<Item>();
        return item != null && item.breakable != null && item.breakable.canBreak;
    }

    /// <inheritdoc/>
    public override void Hack(GameObject target)
    {
        Item? item = target.GetComponentInParent<Item>();
        item?.breakable.Explode(item.totalCombinedMass, item.Center, 1f, 0f, ForceMode.Impulse);
    }
}
