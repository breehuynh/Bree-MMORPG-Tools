// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

// ===================================================================================
// SIMPLE JUKEBOX (SINGLETON)
// ===================================================================================
public class UCE_Jukebox : MonoBehaviour
{
    // singleton for easier access
    public static UCE_Jukebox singleton;

    [Tooltip("[Required] Assign your UCE Jukebox Template here")]
    public UCE_Tmpl_Jukebox jukeboxTemplate;

    private const float MaxVolume_BGM = 1f;
    private float CurrentVolumeNormalized_BGM = 1f;
    private bool isMuted = false;
    private List<AudioSource> bgmSources;

    private void Awake()
    {
        // initialize singleton
        if (singleton == null) singleton = this;
    }

    // ===============================================================================
    // GENERAL FUNCTIONS
    // ===============================================================================

    // -------------------------------------------------------------------------------
    // GetBGMVolume
    // -------------------------------------------------------------------------------
    public float GetBGMVolume()
    {
        return isMuted ? 0f : MaxVolume_BGM * CurrentVolumeNormalized_BGM;
    }

    // -------------------------------------------------------------------------------
    // DisableSoundImmediate
    // -------------------------------------------------------------------------------
    public void DisableSoundImmediate()
    {
        StopAllCoroutines();
        if (bgmSources != null)
        {
            foreach (AudioSource source in bgmSources)
            {
                source.volume = 0;
            }
        }
        isMuted = true;
    }

    // -------------------------------------------------------------------------------
    // EnableSoundImmediate
    // -------------------------------------------------------------------------------
    public void EnableSoundImmediate()
    {
        if (bgmSources != null)
        {
            foreach (AudioSource source in bgmSources)
            {
                source.volume = GetBGMVolume();
            }
        }
        isMuted = false;
    }

    public void Mute(bool isMuted)
    {
        if (isMuted) DisableSoundImmediate();
        else EnableSoundImmediate();
    }

    // -------------------------------------------------------------------------------
    // SetGlobalVolume
    // -------------------------------------------------------------------------------
    public void SetGlobalVolume(float newVolume)
    {
        CurrentVolumeNormalized_BGM = newVolume;
        AdjustSoundImmediate();
    }

    public void SetVolume(float newVolume)
    {
        SetGlobalVolume(newVolume);
    }

    // -------------------------------------------------------------------------------
    // SetBGMVolume
    // -------------------------------------------------------------------------------
    public void SetBGMVolume(float newVolume)
    {
        CurrentVolumeNormalized_BGM = newVolume;
        AdjustSoundImmediate();
    }

    // -------------------------------------------------------------------------------
    // AdjustSoundImmediate
    // -------------------------------------------------------------------------------
    public void AdjustSoundImmediate()
    {
        if (bgmSources != null)
        {
            foreach (AudioSource source in bgmSources)
            {
                source.volume = GetBGMVolume();
            }
        }
    }

    // ===============================================================================
    // BGM FUNCTIONS
    // ===============================================================================

    // -------------------------------------------------------------------------------
    // GetBGMSource
    // -------------------------------------------------------------------------------
    public AudioSource GetBGMSource()
    {
        AudioSource BGMSource = gameObject.AddComponent<AudioSource>();
        BGMSource.playOnAwake = false;
        BGMSource.volume = GetBGMVolume();
        if (bgmSources == null)
        {
            bgmSources = new List<AudioSource>();
        }
        bgmSources.Add(BGMSource);
        return BGMSource;
    }

    // -------------------------------------------------------------------------------
    // RemoveBGMSource
    // -------------------------------------------------------------------------------
    public IEnumerator RemoveBGMSource(AudioSource BGMSource, float delay = 0)
    {
        delay = (delay <= 0) ? BGMSource.clip.length : delay;
        yield return new WaitForSeconds(delay);
        bgmSources.Remove(BGMSource);
        Destroy(BGMSource);
    }

    // -------------------------------------------------------------------------------
    // PlayBGM
    // -------------------------------------------------------------------------------
    public void PlayBGM(AudioClip bgmClip, float fadeDuration, float adjustVol, bool loop)
    {
        //AudioSource curBgm = jukeBox.getCurrentBGMPlaying();
        AudioSource source = GetBGMSource();

        // -- fade-out current BGM
        foreach (AudioSource csource in bgmSources)
        {
            if (csource.isPlaying)
            {
                if (fadeDuration > 0)
                {
                    FadeBGMOut(csource, fadeDuration / 2);
                    StartCoroutine(RemoveBGMSource(csource, fadeDuration / 2));
                }
                else
                {
                    FadeBGMOut(csource, 0);
                    StartCoroutine(RemoveBGMSource(csource));
                }
            }
        }

        // -- adjust Volume for current Song
        SetBGMVolume(adjustVol);

        // -- start new BGM
        if (bgmClip != null)
        {
            source.volume = GetBGMVolume();
            source.clip = bgmClip;
            source.loop = loop;
            source.Play();

            // -- adjust new BGM (either fade-in or not)
            if (fadeDuration > 0)
            {
                FadeBGMIn(source, fadeDuration / 2, fadeDuration / 2);
            }
            else
            {
                float delay = 0f;
                FadeBGMIn(source, delay, fadeDuration);
            }

            if (!loop)
            {
                StartCoroutine(RemoveBGMSource(source));
            }
        }
    }

    // -------------------------------------------------------------------------------
    // StopBGM
    // -------------------------------------------------------------------------------
    public void StopBGM(AudioClip bgmClip, float fadeDuration)
    {
        if (bgmSources != null)
        {
            foreach (AudioSource source in bgmSources)
            {
                if (source.clip == bgmClip && source.isPlaying)
                {
                    FadeBGMOut(source, fadeDuration);
                    StartCoroutine(RemoveBGMSource(source, fadeDuration));
                }
            }
        }
    }

    // -------------------------------------------------------------------------------
    // revertBGM
    // -------------------------------------------------------------------------------
    public void revertBGM(AudioClip bgmClip, float fadeDuration, float adjustedVolume)
    {
        if (bgmClip == jukeboxTemplate.defaultMusicClip) return;

        StopBGM(bgmClip, fadeDuration);

        if (jukeboxTemplate != null &&
            jukeboxTemplate != null &&
            jukeboxTemplate.isActive &&
            jukeboxTemplate.defaultMusicClip != null)
        {
            PlayBGM(jukeboxTemplate.defaultMusicClip, jukeboxTemplate.defaultFadeInFadeOut, jukeboxTemplate.defaultAdjustedVol, true);
        }
    }

    // -------------------------------------------------------------------------------
    // FadeBGMOut
    // -------------------------------------------------------------------------------
    public void FadeBGMOut(AudioSource source, float fadeDuration)
    {
        float delay = 0f;
        float toVolume = 0f;
        StartCoroutine(FadeBGM(source, toVolume, delay, fadeDuration));
    }

    // -------------------------------------------------------------------------------
    // FadeBGMIn
    // -------------------------------------------------------------------------------
    public void FadeBGMIn(AudioSource source, float delay, float fadeDuration)
    {
        float toVolume = GetBGMVolume();
        StartCoroutine(FadeBGM(source, toVolume, delay, fadeDuration));
    }

    // -------------------------------------------------------------------------------
    // checkBGMPlaying
    // -------------------------------------------------------------------------------
    private bool checkBGMPlaying(AudioSource source)
    {
        foreach (AudioSource bgmSource in bgmSources)
        {
            if (bgmSource == source && bgmSource.isPlaying)
            {
                return true;
            }
        }
        return false;
    }

    // -------------------------------------------------------------------------------
    // FadeBGM
    // -------------------------------------------------------------------------------
    public IEnumerator FadeBGM(AudioSource source, float fadeToVolume, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        if (source != null)
        {
            if (duration > 0)
            {
                float volumeDifference = fadeToVolume - source.volume;
                bool pass = false;

                while (!pass)
                {
                    if (source)
                        source.volume += volumeDifference * Time.deltaTime / duration;
                    else
                        break;

                    if (volumeDifference > 0)
                    {
                        pass = source.volume >= fadeToVolume ? true : false;
                    }
                    else if (volumeDifference < 0)
                    {
                        pass = source.volume <= fadeToVolume ? true : false;
                    }
                    else
                    {
                        yield return new WaitForSeconds(duration);
                        break;
                    }
                    yield return null;
                }
            }
            else
            {
                source.volume = fadeToVolume;
            }
        }
    }

    // -------------------------------------------------------------------------------
    // StopAllBGM
    // -------------------------------------------------------------------------------
    public void StopAllBGM(float fadeDuration)
    {
        if (bgmSources != null)
        {
            foreach (AudioSource source in bgmSources)
            {
                if (source != null && source.isPlaying)
                {
                    FadeBGMOut(source, fadeDuration);
                }
            }
        }
    }

    // -------------------------------------------------------------------------------
}

// ===================================================================================
