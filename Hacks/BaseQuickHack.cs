using UnityEngine;

namespace QuickHack.Hacks;


/// <summary>
/// Base class for a Quick Hack.
/// </summary>
public abstract class BaseQuickHack
{
    // We could probably move this stuff to a QuickHackData json

    /// <summary> The name of this Quick Hack that appears in UI. </summary>
    public abstract string Name { get; }

    /// <summary> Determine if this Quick Hack can be applied to a target. </summary>
    public abstract bool CanHack(GameObject target);

    /// <summary> Apply the Quick Hack to a target. </summary>
    public abstract void Hack(GameObject target);
}
