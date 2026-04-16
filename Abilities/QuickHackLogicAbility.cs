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
    [ModOption] [ModOptionCategory("Quick Hack", 1)] [ModOptionFloatValues(0.1f, 1f, 0.1f)]
    public static float TimeScale = 0.1f;

    /// <summary> Called when a new target is selected. </summary>
    public ModEvent<GameObject> OnQuickHackTargetSelected { get; set; } = new();

    /// <summary> Called when a new Quick Hack is selected. </summary>
    public ModEvent<(BaseQuickHack QuickHack, GameObject Target)> OnQuickHackSelected { get; set; } = new();

    /// <summary> Called when a Quick Hack is used. </summary>
    public ModEvent<(BaseQuickHack QuickHack, GameObject Target)> OnQuickHackUsed { get; set; } = new();

    /// <summary> The currently targeted object. </summary>
    public GameObject? Target { get; set; }

    /// <summary> Every quick hack that can be used on the current <see cref="Target"/>. </summary>
    public List<BaseQuickHack> AvailableQuickHacks { get; set; } = new();

    /// <summary> The index of the currently selected Quick Hack relative to <see cref="AvailableQuickHacks"/>. </summary>
    public int SelectedQuickHackIndex { get; set; }

    /// <summary> The currently selected Quick Hack. </summary>
    public BaseQuickHack? SelectedQuickHack => AvailableQuickHacks.Count > 0 ? AvailableQuickHacks[SelectedQuickHackIndex] : null;

    /// <summary> The types of every available Quick Hack. </summary>
    /// <remarks> We store a list of types instead of instances so third party mods can register their own Quick Hacks. </remarks>
    public static List<Type> RegisteredQuickHackTypes { get; set; } = new()
    {
        typeof(BreakQuickHack),
        typeof(ExplodeQuickHack),
        typeof(OverheatQuickHack),
        typeof(ShortCircuitQuickHack),
        typeof(CrippleMovementQuickHack),
        typeof(CyberpsychosisQuickHack),
        typeof(MagneticAttractionQuickHack)
    };

    /// <inheritdoc/>
    public QuickHackLogicAbility(AbilitySpell spell) : base(spell) { }

    private List<BaseQuickHack> quickHackInstances = new List<BaseQuickHack>();

    private bool joystickMoved = false;

    /// <inheritdoc/>
    public override void Init()
    {
        base.Init();

        foreach (Type type in RegisteredQuickHackTypes)
            quickHackInstances.Add((BaseQuickHack)Activator.CreateInstance(type)!);

        Spell.OnStartCast += StartCast;
        Spell.OnStopCast += StopCast;
    }

    /// <inheritdoc/>
    public override void OnUnequip()
    {
        base.OnUnequip();

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
            Transform root = hit.collider.transform.root;

            GameObject? logicalTarget = ResolveLogicalTarget(hit.collider.gameObject);
            if (logicalTarget == null)
                return;

            // We cant invert this check because we need to play logic later (and returning wont allow us to do so)
            if (logicalTarget != Target)
            {
                AvailableQuickHacks.Clear();

                foreach (var qh in quickHackInstances)
                    if (qh.CanHack(logicalTarget))
                        AvailableQuickHacks.Add(qh);

                Target = logicalTarget;
                SelectedQuickHackIndex = 0;

                OnQuickHackTargetSelected?.Invoke(Target);

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
        Vector2 stick = PlayerControl.GetHand(Side.Left).JoystickAxis;
        bool up = stick.y > 0.5f;
        bool down = stick.y < -0.5f;

        if (!up && !down)
            joystickMoved = false;

        if (AvailableQuickHacks.Count > 0 && Target != null && !joystickMoved)
        {
            if (up)
            {
                joystickMoved = true;
                SelectedQuickHackIndex = Mathf.Clamp(SelectedQuickHackIndex - 1, 0, AvailableQuickHacks.Count - 1);
                OnQuickHackSelected?.Invoke((SelectedQuickHack!, Target));
            }
            else if (down)
            {
                joystickMoved = true;
                SelectedQuickHackIndex = Mathf.Clamp(SelectedQuickHackIndex + 1, 0, AvailableQuickHacks.Count - 1);
                OnQuickHackSelected?.Invoke((SelectedQuickHack!, Target));
            }
        }

#if DEBUG
        if (Keyboard.current.wKey.wasPressedThisFrame && AvailableQuickHacks.Count > 0 && Target != null)
        {
            SelectedQuickHackIndex = Mathf.Clamp(SelectedQuickHackIndex - 1, 0, AvailableQuickHacks.Count - 1);
            OnQuickHackSelected?.Invoke((SelectedQuickHack!, Target));
        }

        if (Keyboard.current.sKey.wasPressedThisFrame && AvailableQuickHacks.Count > 0 && Target != null)
        {
            SelectedQuickHackIndex = Mathf.Clamp(SelectedQuickHackIndex + 1, 0, AvailableQuickHacks.Count - 1);
            OnQuickHackSelected?.Invoke((SelectedQuickHack!, Target));
        }
#endif
    }

    private GameObject? ResolveLogicalTarget(GameObject hit)
    {
        var item = hit.GetComponentInParent<Item>();
        if (item != null && item.isPooled == false)
            return item.gameObject;

        var creature = hit.GetComponentInParent<Creature>();
        if (creature != null)
            return creature.gameObject;

        return hit;
    }

    public void StartCast() 
    {
        Spell.OnUpdateCast += UpdateCast;

        TimeManager.SetTimeScale(TimeScale);
    }

    public void StopCast()
    {
        // HACK
        Spell.OnUpdateCast -= UpdateCast;

        TimeManager.SetTimeScale(1.0f);
        TimeManager.StopSlowMotion();

        if (SelectedQuickHack != null && Target != null)
        {
            SelectedQuickHack.Hack(Target);
            OnQuickHackUsed.Invoke((SelectedQuickHack, Target));
        }

        SelectedQuickHackIndex = -1;
        Target = null;
        AvailableQuickHacks.Clear();
    }
}
