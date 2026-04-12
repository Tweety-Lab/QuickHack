using AliLib.Core;
using AliLib.Core.Abilities;
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
    /// <inheritdoc/>
    public QuickHackEffectsAbility(AbilitySpell spell) : base(spell) { }
    
    /// <summary> The Post Process Volume effects are applied to. </summary>
    public Volume PostProcessVolume { get; set; }

    [ModOption(interactionType = ModOption.InteractionType.Slider)] [ModOptionCategory("Effects", 2)] [ModOptionFloatValues(0f, 1f, 0.05f)] 
    public static float HackModeColorR = 1f;

    [ModOption(interactionType = ModOption.InteractionType.Slider)] [ModOptionCategory("Effects", 2)] [ModOptionFloatValues(0f, 1f, 0.05f)]
    public static float HackModeColorG = 1f;

    [ModOption(interactionType = ModOption.InteractionType.Slider)] [ModOptionCategory("Effects", 2)] [ModOptionFloatValues(0f, 1f, 0.05f)]
    public static float HackModeColorB = 1f;

    /// <summary> The color to tint the screen when in Hack Mode. </summary>
    public static Color HackModeColor => new Color(HackModeColorR, HackModeColorG, HackModeColorB);

    /// <inheritdoc/>
    public override void Load()
    {
        base.Load();

        PostProcessVolume = GameObject.FindAnyObjectByType<Volume>();

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
        if (PostProcessVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
            CoroutineRunner.Instance.PlaySmooth(t => colorAdjustments.colorFilter.Override(Color.Lerp(Color.white, HackModeColor, t)), duration: 0.25f * QuickHackLogicAbility.TimeScale);
    }

    public void StopCast()
    {
        // TODO: AliLib needs a way of stopping specific coroutines
        if (PostProcessVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
        {
            Color from = colorAdjustments.colorFilter.value;
            CoroutineRunner.Instance.PlaySmooth(t => colorAdjustments.colorFilter.Override(Color.Lerp(from, Color.white, t)), duration: 0.25f);
        }
    }
}
