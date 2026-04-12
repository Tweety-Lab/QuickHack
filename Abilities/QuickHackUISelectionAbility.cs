using AliLib.Core;
using AliLib.Core.Abilities;
using AliLib.Core.Assets;
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


    /// <inheritdoc/>
    public QuickHackUISelectionAbility(AbilitySpell spell) : base(spell) { }


    /// <inheritdoc/>
    public override void Load()
    {
        base.Load();

        Spell.OnStartCast += OnStartCast;
    }

    private void OnStartCast()
    {
        GameObject? instance = GameObject.Instantiate(SelectionScreen);
        if (instance == null)
            return;

        instance.layer = (int)ThunderRoad.LayerName.UI;

        instance.transform.position = Spell.spellCaster.ragdollHand.ragdoll.headPart.transform.position;

        QuickHackLogicAbility? logic = Spell.GetAbility<QuickHackLogicAbility>();

        Spell.OnStopCast += () => GameObject.Destroy(instance);

        SelectionMenu menu = instance.AddComponent<SelectionMenu>();

        Coroutine followCoroutine = CoroutineRunner.Instance.StartCoroutine(FollowHead(instance.transform, Spell.spellCaster.ragdollHand.ragdoll.headPart.transform));

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
            logic?.OnQuickHackTargetSelected -= OnTargetSelected;
            logic?.OnQuickHackSelected -= OnQuickHackSelected;
            Spell.OnStopCast -= OnStop;
            CoroutineRunner.Instance.StopCoroutine(followCoroutine);
            GameObject.Destroy(instance);
        }

        logic?.OnQuickHackTargetSelected += OnTargetSelected;
        logic?.OnQuickHackSelected += OnQuickHackSelected;
        Spell.OnStopCast += OnStop;
    }

    private IEnumerator FollowHead(Transform instance, Transform head)
    {
        float followSpeed = 50f;
        Vector3 offset = new Vector3(0f, -0.5f, 2f); // Because of inversion: Z = forward, Y = Right, X = Up

        while (true)
        {
            Vector3 targetPosition = head.position + head.TransformDirection(offset);
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - head.position);

            instance.position = Vector3.Lerp(instance.position, targetPosition, followSpeed * Time.deltaTime);
            instance.rotation = Quaternion.Slerp(instance.rotation, targetRotation, followSpeed * Time.deltaTime);

            yield return null;
        }
    }
}
