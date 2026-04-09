using AliLib.Core.Abilities;
using AliLib.Core.Events;
using QuickHack.Hacks;
using QuickHack.Hacks.Default;
using System;
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

    /// <summary> The currently targeted object. </summary>
    public GameObject? Target { get; set; }

    /// <summary> The currently selected Quick Hack. </summary>
    public BaseQuickHack? SelectedQuickHack { get; set; }

    /// <summary> Called when a new Quick Hack is selected. </summary>
    public static ModEvent<(BaseQuickHack QuickHack, GameObject Target)> OnQuickHackSelected { get; set; } = new();

    /// <summary> The types of every available Quick Hack. </summary>
    /// <remarks> We store a list of types instead of instances so third party mods can register their own Quick Hacks. </remarks>
    public static List<Type> RegisteredQuickHackTypes { get; set; } = new()
    {
        typeof(BreakQuickHack),
        typeof(OverheatQuickHack)
    };

    private List<BaseQuickHack> quickHackInstances = new List<BaseQuickHack>();

    /// <inheritdoc/>
    public QuickHackLogicAbility(AbilitySpell spell) : base(spell) { }

    /// <inheritdoc/>
    public override void Load()
    {
        base.Load();

        foreach (Type type in RegisteredQuickHackTypes)
            quickHackInstances.Add((BaseQuickHack)Activator.CreateInstance(type)!);

        Spell.OnUpdateCast += UpdateCast;
        Spell.OnStartCast += StartCast;
        Spell.OnStopCast += StopCast;
    }

    /// <inheritdoc/>
    public override void Unload()
    {
        base.Unload();

        quickHackInstances.Clear();

        Spell.OnUpdateCast -= UpdateCast;
        Spell.OnStartCast -= StartCast;
        Spell.OnStopCast -= StopCast;

        StopCast();
    }

    public void UpdateCast()
    {
        Vector3 forward = Spell.spellCaster.ragdollHand.ragdoll.headPart.transform.forward;
        Vector3 origin = Spell.spellCaster.ragdollHand.ragdoll.headPart.transform.position + forward * 0.5f;

        Ray ray = new Ray(origin, forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100.0f))
        {
            foreach (BaseQuickHack qh in quickHackInstances)
                if (qh.CanHack(hit.collider.gameObject) && (qh != SelectedQuickHack || hit.collider.gameObject != Target))
                {
                    SelectedQuickHack = qh;
                    Target = hit.collider.gameObject;

                    OnQuickHackSelected.Invoke((qh, hit.collider.gameObject));
                    break;
                }
        } else
        {
            SelectedQuickHack = null;
            Target = null;
        }
    }

    public void StartCast()
    {
        Debug.Log("Quick Hack Logic");

        OnQuickHackSelected += (info) => Debug.Log($"Selected {info.QuickHack.GetType().Name} on {info.Target.name}");

        TimeManager.SetTimeScale(TimeScale);
    }

    public void StopCast()
    {
        TimeManager.SetTimeScale(1.0f);

        if (SelectedQuickHack != null && Target != null)
            SelectedQuickHack.Hack(Target);

        SelectedQuickHack = null;
        Target = null;
    }
}
