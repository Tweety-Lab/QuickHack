using ThunderRoad;

namespace QuickHack.Hacks.Default;

/// <summary>
/// Sends an item flying towards the player.
/// </summary>
public class MagneticAttractionQuickHack : ComponentQuickHack<Item>
{
    /// <inheritdoc/>
    public override string Name { get; } = "Magnetic Attraction";

    /// <inheritdoc/>
    public override string Icon { get; } = "QuickHack.Icons.MagneticAttraction";

    /// <inheritdoc/>
    public override void Hack(Item target)
    {
        if (target.IsHeld())
            target.ForceUngrabAll();

        target.physicBody.AddForce((Player.local.creature.ragdoll.GetPart(RagdollPart.Type.Neck).transform.position - target.transform.position).normalized * 20f, UnityEngine.ForceMode.Impulse);
    }
}