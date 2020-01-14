using UnityEngine;

public class AudioSourceSetter : AudioComponent
{
    public enum PlayMode
    {
        PlayClipAtAudioSource,
        PlayClipAtPoint,
    }

    public PlayMode playMode;
    public bool playOnAwake = true;
    public AudioClip[] randomClips;
    private AudioSource cacheAudioSource;

    private void Awake()
    {
        if (playMode == PlayMode.PlayClipAtAudioSource)
        {
            cacheAudioSource = GetComponent<AudioSource>();
            if (cacheAudioSource == null)
                cacheAudioSource = gameObject.AddComponent<AudioSource>();
            cacheAudioSource.Stop();
        }

        if (playOnAwake)
            Play();
    }

    public void Play()
    {
        AudioClip clip = null;
        if (randomClips.Length > 0)
            clip = randomClips[Random.Range(0, randomClips.Length)];

        // No random clips, try to use clip from audio source
        if (clip == null)
        {
            if (cacheAudioSource.clip == null)
                return;
            clip = cacheAudioSource.clip;
        }

        float volume = AudioManager.Singleton.GetVolumeLevel(SettingId);
        switch (playMode)
        {
            case PlayMode.PlayClipAtAudioSource:
                cacheAudioSource.clip = clip;
                cacheAudioSource.volume = volume;
                cacheAudioSource.Play();
                break;

            case PlayMode.PlayClipAtPoint:
                AudioSource.PlayClipAtPoint(clip, transform.position, volume);
                break;
        }
    }

    private void Update()
    {
        if (playMode == PlayMode.PlayClipAtAudioSource)
            cacheAudioSource.volume = AudioManager.Singleton.GetVolumeLevel(SettingId);
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.volume = 0;
        }
    }

#endif
}