using AliLib.Core.Abilities;
using AliLib.Core.Events;
using QuickHack.Hacks;
using QuickHack.Hacks.Default;
using System;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

using Keyboard = UnityEngine.InputSystem.Keyboard;

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

    /// <summary> Every quick hack that can be used on the current <see cref="Target"/>. </summary>
    public List<BaseQuickHack> AvailableQuickHacks { get; set; } = new();

    /// <summary> The index of the currently selected Quick Hack relative to <see cref="AvailableQuickHacks"/>. </summary>
    public int SelectedQuickHackIndex { get; set; }

    /// <summary> The currently selected Quick Hack. </summary>
    public BaseQuickHack? SelectedQuickHack => AvailableQuickHacks.Count > 0 ? AvailableQuickHacks[SelectedQuickHackIndex] : null;

    /// <summary> Called when a new Quick Hack is selected. </summary>
    public static ModEvent<(BaseQuickHack QuickHack, GameObject Target)> OnQuickHackSelected { get; set; } = new();

    /// <summary> The types of every available Quick Hack. </summary>
    /// <remarks> We store a list of types instead of instances so third party mods can register their own Quick Hacks. </remarks>
    public static List<Type> RegisteredQuickHackTypes { get; set; } = new()
    {
        typeof(BreakQuickHack),
        typeof(ExplodeQuickHack),
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

        StopCast();
    }

    public void UpdateCast()
    {
        // Target Logic
        Vector3 forward = Spell.spellCaster.ragdollHand.ragdoll.headPart.transform.forward;
        Vector3 origin = Spell.spellCaster.ragdollHand.ragdoll.headPart.transform.position + forward * 0.5f;

        Ray ray = new Ray(origin, forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            // HACK: We need to support more than just Item and Creature
            GameObject logicalTarget = ResolveLogicalTarget(hit.collider.gameObject);

            // We cant invert this check because we need to play logic later (and returning wont allow us to do so)
            if (logicalTarget != Target)
            {
                AvailableQuickHacks.Clear();

                foreach (var qh in quickHackInstances)
                    if (qh.CanHack(logicalTarget))
                        AvailableQuickHacks.Add(qh);

                Target = logicalTarget;
                SelectedQuickHackIndex = 0;

                if (AvailableQuickHacks.Count > 0)
                    OnQuickHackSelected?.Invoke((SelectedQuickHack!, Target));
            }
        } else
        {
            SelectedQuickHackIndex = -1;
            Target = null;
            AvailableQuickHacks.Clear();
        }

        // QH Selection Logic
#if DEBUG
        if (Keyboard.current.downArrowKey.wasPressedThisFrame && AvailableQuickHacks.Count > 0 && Target != null)
        {
            SelectedQuickHackIndex = Mathf.Clamp(SelectedQuickHackIndex + 1, 0, AvailableQuickHacks.Count - 1);
            OnQuickHackSelected?.Invoke((SelectedQuickHack!, Target));
        }

        if (Keyboard.current.upArrowKey.wasPressedThisFrame && AvailableQuickHacks.Count > 0 && Target != null)
        {
            SelectedQuickHackIndex = Mathf.Clamp(SelectedQuickHackIndex - 1, 0, AvailableQuickHacks.Count - 1);
            OnQuickHackSelected?.Invoke((SelectedQuickHack!, Target));
        }
#endif
    }

    private GameObject ResolveLogicalTarget(GameObject hit) => hit.GetComponentInParent<Item>()?.gameObject ?? hit.GetComponentInParent<Creature>()?.gameObject ?? hit;

    public void StartCast() 
    {
        Debug.Log("Quick Hack Logic");

        Spell.OnUpdateCast += UpdateCast;
        OnQuickHackSelected += DebugLog;

        TimeManager.SetTimeScale(TimeScale);
    }

    public void StopCast()
    {
        // HACK
        Spell.OnUpdateCast -= UpdateCast;
        OnQuickHackSelected -= DebugLog;

        TimeManager.SetTimeScale(1.0f);

        if (SelectedQuickHack != null && Target != null)
            SelectedQuickHack.Hack(Target);

        SelectedQuickHackIndex = -1;
        Target = null;
        AvailableQuickHacks.Clear();
    }

    private void DebugLog((BaseQuickHack QuickHack, GameObject Target) info) => Debug.Log($"Selected {info.QuickHack.Name} on {info.Target.name}");
}
