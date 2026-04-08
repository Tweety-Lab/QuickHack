using ThunderRoad;
using UnityEngine;

namespace QuickHack.Hacks;

internal class DebugQuickHack : BaseQuickHack
{
    /// <inheritdoc/>
    public override string Name { get; } = "Debug";

    /// <inheritdoc/>
    public override bool CanHack(GameObject target) => target.GetComponentInParent<Creature>() != null;

    /// <inheritdoc/>
    public override void Hack(GameObject target) => target.GetComponentInParent<Creature>().Kill();
}
