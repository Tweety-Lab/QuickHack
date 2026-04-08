using AliLib.Core.Abilities;
using UnityEngine;

namespace QuickHack.Abilities;

/// <summary>
/// Handles the VFX of the Quick Hack spell.
/// </summary>
public class QuickHackEffectsAbility : Ability
{
    /// <inheritdoc/>
    public QuickHackEffectsAbility(AbilitySpell spell) : base(spell) { }

    /// <inheritdoc/>
    public override void Load()
    {
        base.Load();

        Spell.OnStartCast += StartCast;
    }

    /// <inheritdoc/>
    public override void Unload()
    {
        base.Unload();

        Spell.OnStartCast -= StartCast;
    }

    public void StartCast()
    {
        Debug.Log("Quick Hack VFX");
    }
}
