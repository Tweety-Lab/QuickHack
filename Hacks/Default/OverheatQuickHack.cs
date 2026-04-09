using System;
using System.Collections.Generic;
using System.Text;
using ThunderRoad;

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
        Burning burning = new Burning();

        target.Statuses.Add(burning);
        burning.Ignite();
    }
}
