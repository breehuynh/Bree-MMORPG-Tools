using System.Collections.Generic;
using UnityEngine;

public class InputSettingManager : MonoBehaviour
{
    [System.Serializable]
    public struct InputSetting
    {
        public string keyName;
        public KeyCode keyCode;
    }

    public static InputSettingManager Singleton { get; protected set; }

    public InputSetting[] settings;

    public readonly Dictionary<string, KeyCode> Settings = new Dictionary<string, KeyCode>();

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = GetComponent<InputSettingManager>();
        DontDestroyOnLoad(gameObject);

        foreach (InputSetting setting in settings)
        {
            Settings[setting.keyName] = setting.keyCode;
        }
    }
}