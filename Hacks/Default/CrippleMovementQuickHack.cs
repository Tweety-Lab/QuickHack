
using AliLib.Core;
using ThunderRoad;

namespace QuickHack.Hacks.Default;

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
