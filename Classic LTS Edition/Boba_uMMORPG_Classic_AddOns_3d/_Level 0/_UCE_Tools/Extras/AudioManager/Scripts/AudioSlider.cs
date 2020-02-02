using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AudioSlider : AudioComponent
{
    public Slider slider { get; private set; }

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.onValueChanged.RemoveListener(OnValueChanged);
        slider.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(float level)
    {
        AudioManager.Singleton.SetVolumeLevel(SettingId, level);
    }

    private void OnEnable()
    {
        slider.value = AudioManager.Singleton.GetVolumeLevel(SettingId);
    }
}