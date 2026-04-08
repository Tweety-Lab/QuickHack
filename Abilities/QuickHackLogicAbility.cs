using AliLib.Core.Abilities;
using UnityEngine;

namespace QuickHack.Abilities;

/// <summary>
/// Handles the Gameplay Logic of the Quick Hack spell.
/// </summary>
public class QuickHackLogicAbility : Ability
{
    /// <summary> The time scale applied whilst in hacking mode. </summary>
    public static float TimeScale { get; set; } = 0.1f;

    /// <inheritdoc/>
    public QuickHackLogicAbility(AbilitySpell spell) : base(spell) { }

    /// <inheritdoc/>
    public override void Load()
    {
        base.Load();

        Spell.OnStartCast += StartCast;
        Spell.OnStopCast += StopCast;
    }

    /// <inheritdoc/>
    public override void Unload()
    {
        base.Unload();

        Spell.OnStartCast -= StartCast;
        Spell.OnStopCast -= StopCast;

        StopCast();
    }

    public void StartCast()
    {
        Debug.Log("Quick Hack Logic");

        ThunderRoad.TimeManager.SetTimeScale(TimeScale);
    }

    public void StopCast()
    {
        ThunderRoad.TimeManager.SetTimeScale(1.0f);
    }
}
