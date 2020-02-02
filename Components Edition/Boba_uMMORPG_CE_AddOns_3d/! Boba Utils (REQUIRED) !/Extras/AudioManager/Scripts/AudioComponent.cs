using UnityEngine;

public enum AudioComponentSettingType
{
    MASTER,
    BGM,
    SFX,
    AMBIENT,
    OTHER
}

public class AudioComponent : MonoBehaviour
{
    public AudioComponentSettingType type;
    public string otherSettingId;

    public string SettingId
    {
        get
        {
            switch (type)
            {
                case AudioComponentSettingType.MASTER:
                    return AudioManager.Singleton.masterVolumeSetting.id;

                case AudioComponentSettingType.BGM:
                    return AudioManager.Singleton.bgmVolumeSetting.id;

                case AudioComponentSettingType.SFX:
                    return AudioManager.Singleton.sfxVolumeSetting.id;

                case AudioComponentSettingType.AMBIENT:
                    return AudioManager.Singleton.ambientVolumeSetting.id;
            }
            return otherSettingId;
        }
    }
}