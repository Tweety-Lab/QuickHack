using AliLib.Core;
using ThunderRoad;
using UnityEngine;

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

        CoroutineRunner.Instance.PlaySmooth((t) =>
        {
            target.physicBody.AddForce((Player.local.creature.ragdoll.GetPart(RagdollPart.Type.Neck).transform.position - target.transform.position).normalized * target.physicBody.mass * 0.01f * t, UnityEngine.ForceMode.Impulse);
        }, 3f, curve: new AnimationCurve(new Keyframe(0f, 0f, 0f, 40f), new Keyframe(1f, 1f, 0.5f, 0f)));
    }
}