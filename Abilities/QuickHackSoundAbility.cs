
using AliLib.Core;
using AliLib.Core.Abilities;
using AliLib.Core.Assets;
using QuickHack.Hacks;
using ThunderRoad;
using UnityEngine;
using static QuickHack.Components.SelectionMenu;

namespace QuickHack.Abilities;

/// <summary>
/// Handles the SFX/Sound of the Quick Hack spell.
/// </summary>
public class QuickHackSoundAbility : Ability
{
    [ModOption(interactionType = ModOption.InteractionType.Slider)]
    [ModOptionCategory("Sound", 3)]
    [ModOptionFloatValues(0f, 1f, 0.1f)]
    public static float UseSoundVolume = 0.6f;

    [ModOption(interactionType = ModOption.InteractionType.Slider)]
    [ModOptionCategory("Sound", 3)]
    [ModOptionFloatValues(0f, 1f, 0.1f)]
    public static float StartSoundVolume = 0.3f;

    [ModOption(interactionType = ModOption.InteractionType.Slider)]
    [ModOptionCategory("Sound", 3)]
    [ModOptionFloatValues(0f, 1f, 0.1f)]
    public static float BackgroundMusicVolume = 0.5f;

    [ModOption(interactionType = ModOption.InteractionType.Slider)]
    [ModOptionCategory("Sound", 3)]
    [ModOptionFloatValues(0f, 1f, 0.1f)]
    public static float SelectSoundVolume = 0.8f;

    [Addressable("QuickHack.Sounds.Use")]
    public static AudioClip? UseSound { get; set; }

    [Addressable("QuickHack.Sounds.Start")]
    public static AudioClip? StartSound { get; set; }

    [Addressable("QuickHack.Sounds.Background")]
    public static AudioClip? BackgroundSound { get; set; }

    [Addressable("QuickHack.Sounds.Select")]
    public static AudioClip? SelectSound { get; set; }

    private AudioSource? backgroundSource;

    /// <inheritdoc/>
    public QuickHackSoundAbility(AbilitySpell spell) : base(spell) { }

    /// <inheritdoc/>
    public override void Load()
    {
        base.Load();

        Spell.OnStartCast += OnStartCast;
        Spell.OnStopCast += OnStopCast;

        var logic = Spell.GetAbility<QuickHackLogicAbility>();
        logic?.OnQuickHackSelected += OnQuickHackSelected;
        logic?.OnQuickHackUsed += OnQuickHackUsed;
    }

    /// <inheritdoc/>
    public override void Unload()
    {
        base.Unload();

        Spell.OnStartCast -= OnStartCast;
        Spell.OnStopCast -= OnStopCast;

        var logic = Spell.GetAbility<QuickHackLogicAbility>();
        logic?.OnQuickHackSelected -= OnQuickHackSelected;
        logic?.OnQuickHackUsed -= OnQuickHackUsed;
    }

    private void OnStartCast()
    {
        // We use AudioMixerName.UI to avoid the slow-mo effects being applied to our audio
        if (StartSound != null)
            Audio.PlayNoBlend(StartSound, AudioMixerName.UI, StartSoundVolume);

        if (BackgroundSound != null)
            backgroundSource = Audio.PlayNoBlend(BackgroundSound, AudioMixerName.UI, BackgroundMusicVolume);
    }

    private void OnStopCast()
    {
        if (backgroundSource != null)
            CoroutineRunner.Instance.StartCoroutine(Audio.FadeOut(backgroundSource, 1f));
    }

    private void OnQuickHackSelected((BaseQuickHack QuickHack, GameObject Target) info)
    {
        if (SelectSound != null)
            Audio.PlayNoBlend(SelectSound, AudioMixerName.UI, SelectSoundVolume);
    }

    private void OnQuickHackUsed((BaseQuickHack QuickHack, GameObject Target) info)
    {
        // This isnt AI guys its just best practice to always assume an [Addressable] can be null !!
        if (UseSound != null)
            Audio.PlayNoBlend(UseSound, AudioMixerName.Effect, UseSoundVolume);
    }
}
