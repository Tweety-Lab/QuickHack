using ThunderRoad;

namespace QuickHack.Hacks.Default;

public class ShortCircuitQuickHack : ComponentQuickHack<Creature>
{
    /// <inheritdoc/>
    public override string Name { get; } = "Short Circuit";

    /// <inheritdoc/>
    public override string Icon { get; } = "QuickHack.Icons.ShortCircuit";

    /// <inheritdoc/>
    public override bool CanHack(Creature target) => !target.isPlayer;

    /// <inheritdoc/>
    public override void Hack(Creature target)
    {
        StatusDataElectrocute electrocuteData = Catalog.GetData<StatusDataElectrocute>("Electrocute");
        target.Inflict(electrocuteData, this, 3f);
    }
}

