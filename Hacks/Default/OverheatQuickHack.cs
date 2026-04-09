using ThunderRoad;
using ThunderRoad.Skill.Spell;
using UnityEngine;

namespace QuickHack.Hacks.Default;


public class OverheatQuickHack : ComponentQuickHack<Creature>
{
    /// <inheritdoc/>
    public override string Name { get; } = "Overheat";

    /// <inheritdoc/>
    public override bool CanHack(Creature target) => !target.isPlayer;

    /// <inheritdoc/>
    public override void Hack(Creature target)
    {
        StatusDataBurning burningData = Catalog.GetData<StatusDataBurning>("Burning");
        target.Inflict(burningData, this, float.PositiveInfinity, burningData.maxHeat);
    }
}
