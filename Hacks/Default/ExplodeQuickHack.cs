using ThunderRoad;
using UnityEngine;

namespace QuickHack.Hacks.Default;

/// <summary>
/// Explode a breakable object.
/// </summary>
public class ExplodeQuickHack : ComponentQuickHack<Item>
{
    /// <inheritdoc/>
    public override string Name { get; } = "Explode";

    /// <inheritdoc/>
    public override bool CanHack(Item target) => target.breakable != null && target.breakable.canBreak;

    /// <inheritdoc/>
    public override void Hack(Item target)
    {
        target.breakable.Explode(target.totalCombinedMass * 4f, target.Center, 5f, 0f, ForceMode.Impulse);
        EffectInstance? effect = Catalog.GetData<EffectData>("MeteorExplosion").Spawn(target.Center, Quaternion.identity, target.transform, null, false);
        effect?.Play();
    }
}