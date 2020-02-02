# unity-audio-manager

This project can helps to manage audio settings to save them in PlayerPrefs, Can manage Master volume, BGM, SFX, Ambient or another kind of volume as you wish

## How to use

- Add `AudioManager` component to any empty game object in first scene
- To add volume slider, add `AudioSlider` to ui game object then set kind of setting (Master Volume or BGM or SFX ...)
- To add volume turn on/off toggle, add `AudioToggle` to ui game object then set kind of setting (Master Volume or BGM or SFX ...)
- Then add `AudioSourceSetter` to any audio source and set play mode to `PlayClipAtAudioSource` to apply volume setting to this audio source while play a game
- Or add `AudioSourceSetter` with `PlayClipAtPoint` play mode to play audio at specific point (transform position) with volume setting while play a game