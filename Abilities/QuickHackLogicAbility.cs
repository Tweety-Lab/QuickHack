using AliLib.Core.Abilities;
using QuickHack.Hacks;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace QuickHack.Abilities;

/// <summary>
/// Handles the Gameplay Logic of the Quick Hack spell.
/// </summary>
public class QuickHackLogicAbility : Ability
{
    [ModOption] [ModOptionCategory("Quick Hack", 1)] [ModOptionFloatValues(0f, 1f, 0.1f)]
    public static float TimeScale = 0.1f;

    /// <summary> All registered Quick Hacks. </summary>
    public List<BaseQuickHack> RegisteredQuickHacks { get; set; } = new()
    {
        new DebugQuickHack()
    };

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

        TimeManager.SetTimeScale(TimeScale);
    }

    public void StopCast()
    {
        TimeManager.SetTimeScale(1.0f);

        Vector3 forward = Spell.spellCaster.ragdollHand.ragdoll.headPart.transform.forward;
        Vector3 origin = Spell.spellCaster.ragdollHand.ragdoll.headPart.transform.position + forward * 0.5f;

        Ray ray = new Ray(origin, forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100.0f))
            foreach (BaseQuickHack qh in RegisteredQuickHacks)
                if (qh.CanHack(hit.collider.gameObject))
                    qh.Hack(hit.collider.gameObject);

    }
}
