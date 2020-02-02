using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class AudioToggle : AudioComponent
{
    public Toggle toggle { get; private set; }

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.RemoveListener(OnValueChanged);
        toggle.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(bool isOn)
    {
        AudioManager.Singleton.SetVolumeIsOn(SettingId, isOn);
    }

    private void OnEnable()
    {
        toggle.isOn = AudioManager.Singleton.GetVolumeIsOn(SettingId);
    }
}