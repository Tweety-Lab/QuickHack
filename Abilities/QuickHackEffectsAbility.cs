using AliLib.Core;
using AliLib.Core.Abilities;
using AliLib.Core.Assets;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace QuickHack.Abilities;

/// <summary>
/// Handles the VFX (Everything visual + non-logical) of the Quick Hack spell.
/// </summary>
public class QuickHackEffectsAbility : Ability
{
    /// <summary> The Post Process Volume effects are applied to. </summary>
    public Volume PostProcessVolume { get; set; }

    [ModOption(interactionType = ModOption.InteractionType.Slider)] [ModOptionCategory("Effects", 2)] [ModOptionFloatValues(0f, 1f, 0.05f)] 
    public static float HackModeColorR = 0.55f;

    [ModOption(interactionType = ModOption.InteractionType.Slider)] [ModOptionCategory("Effects", 2)] [ModOptionFloatValues(0f, 1f, 0.05f)]
    public static float HackModeColorG = 1f;

    [ModOption(interactionType = ModOption.InteractionType.Slider)] [ModOptionCategory("Effects", 2)] [ModOptionFloatValues(0f, 1f, 0.05f)]
    public static float HackModeColorB = 0.8f;

    [Addressable("QuickHack.ScannedObjectMat")]
    public static Material? ScannedObjectMaterial { get; set; }

    /// <summary> The color to tint the screen when in Hack Mode. </summary>
    public static Color HackModeColor => new Color(HackModeColorR, HackModeColorG, HackModeColorB);

    /// <inheritdoc/>
    public QuickHackEffectsAbility(AbilitySpell spell) : base(spell) { }

    private List<Renderer> scannedRenderers = new List<Renderer>();

    /// <inheritdoc/>
    public override void OnEquip()
    {
        base.OnEquip();

        PostProcessVolume = GameObject.FindAnyObjectByType<Volume>();
    }
    
    /// <inheritdoc/>
    public override void Init()
    {
        base.Init();

        Spell.OnStartCast += StartCast;
        Spell.OnStopCast += StopCast;
    }

    /// <inheritdoc/>
    public override void OnUnequip()
    {
        base.OnUnequip();

        StopCast();
    }

    public void StartCast()
    {
        if (!PostProcessVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
            colorAdjustments = PostProcessVolume.profile.Add<ColorAdjustments>(true);

        colorAdjustments.colorFilter.overrideState = true;

        CoroutineRunner.Instance.PlaySmooth(
            t => colorAdjustments.colorFilter.Override(Color.Lerp(Color.white, HackModeColor, t)),
            duration: 0.25f * QuickHackLogicAbility.TimeScale
        );

        Spell.GetAbility<QuickHackLogicAbility>()?.OnQuickHackTargetSelected += OnQuickHackTargetSelected;
    }

    public void StopCast()
    {
        ClearScannedOverlay();

        // TODO: AliLib needs a way of stopping specific coroutines
        if (PostProcessVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
        {
            Color from = colorAdjustments.colorFilter.value;
            CoroutineRunner.Instance.PlaySmooth(t => colorAdjustments.colorFilter.Override(Color.Lerp(from, Color.white, t)), duration: 0.25f);
        }

        Spell.GetAbility<QuickHackLogicAbility>()?.OnQuickHackTargetSelected -= OnQuickHackTargetSelected;
    }

    private void OnQuickHackTargetSelected(GameObject target)
    {
        ClearScannedOverlay();

        if (Spell.GetAbility<QuickHackLogicAbility>()?.AvailableQuickHacks.Count == 0)
            return;

        scannedRenderers.AddRange(target.GetComponentsInChildren<Renderer>());

        foreach (Renderer r in scannedRenderers)
        {
            Material[] mats = r.materials;
            Material[] newMats = new Material[mats.Length + 1];
            mats.CopyTo(newMats, 0);
            newMats[newMats.Length - 1] = ScannedObjectMaterial ?? new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            r.materials = newMats;
        }
    }

    private void ClearScannedOverlay()
    {
        foreach (Renderer r in scannedRenderers)
        {
            if (r == null) continue;

            Material[] mats = r.sharedMaterials;
            if (mats.Length == 0)
                continue;

            Material[] newMats = new Material[mats.Length - 1];
            System.Array.Copy(mats, newMats, newMats.Length);
            r.materials = newMats;
        }

        scannedRenderers.Clear();
    }
}
