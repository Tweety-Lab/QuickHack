using AliLib.Core;
using ThunderRoad;

namespace QuickHack.Hacks.Default;

/// <summary>
/// Change enemy team.
/// </summary>
public class CyberpsychosisQuickHack : ComponentQuickHack<Creature>
{
    /// <inheritdoc/>
    public override string Name { get; } = "Cyberpsychosis";

    /// <inheritdoc/>
    public override string Icon { get; } = "QuickHack.Icons.Cyberpsychosis";

    /// <inheritdoc/>
    public override bool CanHack(Creature target) => !target.isPlayer;

    /// <inheritdoc/>
    public override void Hack(Creature target)
    {
        StatusDataElectrocute electrocuteData = Catalog.GetData<StatusDataElectrocute>("Electrocute");
        target.Inflict(electrocuteData, this, 0.5f);

        int originalFaction = target.factionId;
        target.SetFaction(Player.local.creature.factionId);

        CoroutineRunner.Instance.PlayAfterDelay(() =>
        {
            StatusDataElectrocute electrocuteData = Catalog.GetData<StatusDataElectrocute>("Electrocute");
            target.Inflict(electrocuteData, this, 0.5f);

            target.SetFaction(originalFaction);
        }, 30f);
    }
}
