
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
    public static float BackgroundMusicVolume = 0.5f;

    [Addressable("QuickHack.Sounds.Use")]
    public static AudioClip? UseSound { get; set; }

    [Addressable("QuickHack.Sounds.Background")]
    public static AudioClip? BackgroundSound { get; set; }

    /// <inheritdoc/>
    public QuickHackSoundAbility(AbilitySpell spell) : base(spell) { }

    /// <inheritdoc/>
    public override void Load()
    {
        base.Load();

        Spell.OnStartCast += () =>
        {
            if (BackgroundSound != null)
            {
                AudioSource? background = Audio.PlayNoBlend(BackgroundSound, ThunderRoad.AudioMixerName.Ambient, BackgroundMusicVolume);
                Spell.OnStopCast += () =>
                {
                    if (background != null)
                        Audio.Stop(background);
                };
            }
        };

        Spell.GetAbility<QuickHackLogicAbility>()?.OnQuickHackUsed += (info) =>
        {
            // This isnt AI guys its just best practice to always assume an [Addressable] can be null !!
            if (UseSound == null)
                return;

            Audio.PlayNoBlend(UseSound, ThunderRoad.AudioMixerName.Effect, UseSoundVolume);
        };
    }
}
