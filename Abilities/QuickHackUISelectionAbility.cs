using AliLib.Core;
using AliLib.Core.Abilities;
using AliLib.Core.Assets;
using AliLib.Core.GC;
using QuickHack.Components;
using QuickHack.Hacks;
using System;
using System.Collections;
using ThunderRoad;
using UnityEngine;

namespace QuickHack.Abilities;

/// <summary>
/// Handles the quickhack selection menu.
/// </summary>
public class QuickHackUISelectionAbility : Ability
{
    [Addressable("QuickHack.SelectionScreen")]
    public static GameObject? SelectionScreen { get; set; }

    private Coroutine? followCoroutine;

    /// <inheritdoc/>
    public QuickHackUISelectionAbility(AbilitySpell spell) : base(spell) { }


    /// <inheritdoc/>
    public override void Load()
    {
        base.Load();

        Spell.OnStartCast += OnStartCast;
    }

    /// <inheritdoc/>
    public override void Unload()
    {
        base.Unload();

        base.Unload();
        Spell.OnStartCast -= OnStartCast;
        if (followCoroutine != null)
            CoroutineRunner.Instance.StopCoroutine(followCoroutine);
    }

    private void OnStartCast()
    {
        SmartObject<GameObject> instance = GameObject.Instantiate(SelectionScreen)!;
        if (instance == null || instance.Object == null)
            return;

        instance.Object.transform.position = Spell.spellCaster.ragdollHand.ragdoll.headPart.transform.position;

        QuickHackLogicAbility? logic = Spell.GetAbility<QuickHackLogicAbility>();

        SelectionMenu menu = instance.Object.AddComponent<SelectionMenu>();

        followCoroutine = CoroutineRunner.Instance.StartCoroutine(FollowHead(instance.Object.transform, Spell.spellCaster.ragdollHand.ragdoll.headPart.transform));

        void OnTargetSelected(GameObject target)
        {
            menu.Entries.Clear();

            foreach (BaseQuickHack qh in logic.AvailableQuickHacks)
            {
                menu.Entries.Add(new SelectionMenu.QuickHackInfo()
                {
                    Name = qh.Name.ToUpper(),
                    IconAddress = qh.Icon,
                });
            }

            menu.Populate();
        }

        void OnQuickHackSelected((BaseQuickHack QuickHack, GameObject Target) info)
        {
            menu.SelectedIndex = Spell.GetAbility<QuickHackLogicAbility>()?.SelectedQuickHackIndex ?? 0;
        }

        // It may seem dumb to store this here instead of in a dedicated onstop method but since we only need to cleanup when the spell has started AND THEN stopped
        // we can just do it here.
        void OnStop()
        {
            CoroutineRunner.Instance.StopCoroutine(followCoroutine);

            logic?.OnQuickHackTargetSelected -= OnTargetSelected;
            logic?.OnQuickHackSelected -= OnQuickHackSelected;

            CoroutineRunner.Instance.StartCoroutine(DeferredCleanup()); // Defer unsubscription to avoid modifying the delegate chain mid enumeration
        }

        IEnumerator DeferredCleanup()
        {
            yield return null;

            Spell.OnStopCast -= OnStop;
        }

        logic?.OnQuickHackTargetSelected += OnTargetSelected;
        logic?.OnQuickHackSelected += OnQuickHackSelected;
        Spell.OnStopCast += OnStop;
    }

    private IEnumerator FollowHead(Transform instance, Transform head)
    {
        float followSpeed = 50f;
        Vector3 offset = new Vector3(0f, -0.5f, 2f); // Because of inversion: Z = forward, Y = Right, X = Up

        while (instance != null && head != null)
        {
            Vector3 targetPosition = head.position + head.TransformDirection(offset);
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - head.position);

            instance.position = Vector3.Lerp(instance.position, targetPosition, followSpeed * Time.deltaTime);
            instance.rotation = Quaternion.Slerp(instance.rotation, targetRotation, followSpeed * Time.deltaTime);

            yield return null;
        }
    }
}
