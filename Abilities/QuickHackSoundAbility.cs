
using AliLib.Core.Abilities;
using AliLib.Core.Assets;
using AliLib.Core;
using UnityEngine;
using ThunderRoad;

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

    /// <inheritdoc/>
    public QuickHackSoundAbility(AbilitySpell spell) : base(spell) { }

    /// <inheritdoc/>
    public override void Load()
    {
        base.Load();

        Spell.OnStartCast += () =>
        {
            // We use ambient to avoid the slow-mo effects being applied to our audio
            if (StartSound != null)
                Audio.PlayNoBlend(StartSound, AudioMixerName.Ambient, StartSoundVolume);

            if (BackgroundSound != null)
            {
                AudioSource? background = Audio.PlayNoBlend(BackgroundSound, AudioMixerName.Ambient, BackgroundMusicVolume);
                Spell.OnStopCast += () =>
                {
                    if (background != null)
                        CoroutineRunner.Instance.StartCoroutine(Audio.FadeOut(background, 1f));
                };
            }
        };

        Spell.GetAbility<QuickHackLogicAbility>()?.OnQuickHackSelected += (info) =>
        {
            if (SelectSound != null)
                Audio.PlayNoBlend(SelectSound, AudioMixerName.Ambient, SelectSoundVolume);
        };

        Spell.GetAbility<QuickHackLogicAbility>()?.OnQuickHackUsed += (info) =>
        {
            // This isnt AI guys its just best practice to always assume an [Addressable] can be null !!
            if (UseSound != null)
                Audio.PlayNoBlend(UseSound, AudioMixerName.Effect, UseSoundVolume);
        };
    }
}
