// =======================================================================================
// Maintained by bobatea#9400 on Discord
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............:  
  
// * Leave a star on my Github Repo.....: https://github.com/breehuynh/Bree-mmorpg-tools
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UCE_UI_Settings : MonoBehaviour
{
    #region Variables

    #region _iMMOMAINMENU

#if _iMMOMAINMENU

#else
    public KeyCode hotKey = KeyCode.Escape;         //The hotkey used to open the settings menu if UCE Main Menu is not used.

#endif

    #endregion _iMMOMAINMENU

    public GameObject panel;                        //Options menu object.
    public Dropdown resolutionDropdown;             //Dropdown menu of resolutions.
    public Slider[] gameplaySlider;                 //Sliders for all gameplay settings.
    public Slider[] videoSliders;                   //Sliders for all video quality.
    public Slider[] soundSliders;                   //Sliders for all sound volume.
    public Toggle[] gameplayToggle;                 //Toggles for all gameplay settings.
    public Toggle[] videoToggles;                   //Toggles for all video quality.
    public Toggle[] soundToggles;                   //Togggles for all sound volume.
    public Button applyButton;                      //Button to apply the settings.

    public Resolution[] resolutions;                //Array of possible resolutions.
    public AudioSource[] musicPlayed;               //The audio to adjust, this can be made into an array to allow for multiple audio sources.
    public AudioSource[] effectsPlayed;             //The audio to adjust, this can be made into an array to allow for multiple audio sources.
    public AudioSource[] ambientPlayed;             //The audio to adjust, this can be made into an array to allow for multiple audio sources.

    public Text[] keybindingText;
    public GameObject[] uiScalable;                 //Array of all ui components you wish to have scale with uiscale.
    [HideInInspector] public bool waitingForKey = false;
    [HideInInspector] public GameObject currentButton;
    [HideInInspector] public KeyCode currentKey = KeyCode.W;

    private UCE_UI_SettingsVariables settingsVariables;

    #endregion Variables

    #region Functions

    //Loads all of our settings on start.
    private void Start()
    {
        settingsVariables = FindObjectOfType<UCE_UI_SettingsVariables>().GetComponent<UCE_UI_SettingsVariables>();
        resolutionDropdown.options.Clear();
        resolutions = Screen.resolutions;                                                       //Set current resolution to screens resolution.
        foreach (Resolution resolution in resolutions)                                          //Loop through resolution possibilities in the array.
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));     //Populate teh dropdown with the resolution possibilities.
        LoadSettings();
    }

    //Loads all resolutions on enable incase screen swap happens.
    private void OnEnable()
    {
        resolutionDropdown.options.Clear();
        resolutions = Screen.resolutions;                                                       //Set current resolution to screens resolution.
        foreach (Resolution resolution in resolutions)                                          //Loop through resolution possibilities in the array.
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));     //Populate teh dropdown with the resolution possibilities.
    }

    #region _iMMOMAINMENU

#if _iMMOMAINMENU

#else
    //Initiates every frame.
    private void Update()
    {
        Player player = Player.localPlayer;                         //Grab the player from utils.
        if (player == null) return;                                 //Don't continue if there is no player found.

        if (Input.GetKeyDown(hotKey) && !UIUtils.AnyInputActive())  //If the hotkey is pressed and chat is not active then progress.
        {
            CanvasGroup cg = panel.GetComponent<CanvasGroup>();

            if (cg.alpha == 0)
            {
                cg.alpha = 1;                                       //Set the options menu as active.
                cg.blocksRaycasts = true;                           //Set the options menu to catch mouse clicks.
            }
            else
            {
                cg.alpha = 0;                                       //Set the options menu as active.
                cg.blocksRaycasts = false;                          //Set the options menu to catch mouse clicks.
            }
        }
    }

#endif

    #endregion _iMMOMAINMENU

    #region Button

    //Close the options menu, all settings save on change.
    public void OnApplyClick()
    {
        LoadSettings();
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        cg.alpha = 0;                                       //Set the options menu as active.
        cg.blocksRaycasts = false;                          //Set the options menu to catch mouse clicks.
    }

    #endregion Button

    #region Save Settings

    #region Keybinding

    // Begins our keybinding assignment.
    public void StartKeybinding(int keyIndex)
    {
        if (!waitingForKey)
            StartCoroutine(AssignKey(keyIndex));
    }

    // Waits for a keybinding to be hit then assigns it.
    public IEnumerator AssignKey(int keyIndex)
    {
        waitingForKey = true;
        currentButton = EventSystem.current.currentSelectedGameObject;

        yield return WaitForKey(); //Executes endlessly until user presses a key

        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            if (Input.GetKey(kcode))
                currentKey = kcode;

        for (int i = 0; i < settingsVariables.keybindings.Length; i++)
        {
            if (settingsVariables.keybindings[i] == currentKey)
            {
                settingsVariables.keybindings[i] = KeyCode.None;
                keybindingText[i].text = "";
                PlayerPrefs.SetString("keybindings[" + settingsVariables.keybindings[i] + "]", KeyCode.None.ToString());
            }
        }

        currentButton.GetComponentInChildren<Text>().text = currentKey.ToString();
        settingsVariables.keybindings[keyIndex] = currentKey;
        settingsVariables.keybindUpdate[keyIndex] = true;
        PlayerPrefs.SetString("keybindings[" + keyIndex + "]", currentKey.ToString());

        waitingForKey = false;

        yield return null;
    }

    // Waits for a key to be pressed.
    private IEnumerator WaitForKey()
    {
        while (!Input.anyKeyDown)
            yield return null;
    }

    #endregion Keybinding

    #region Gameplay

    // Set block trades and save its settings.
    public void SaveBlockTrades(Toggle toggle)
    {
        Player player = Player.localPlayer;
        if (player != null)
            player.CmdBlockTradeRequest(toggle.isOn);
        PlayerPrefs.SetInt("BlockTrades", toggle.isOn ? 1 : 0);
    }

    // Set block party invites and save its settings.
    public void SaveBlockParty(Toggle toggle)
    {
        Player player = Player.localPlayer;
        if (player != null)
            player.CmdBlockPartyInvite(toggle.isOn);
        PlayerPrefs.SetInt("BlockParty", toggle.isOn ? 1 : 0);
    }

    // Set block guild invites and save its settings.
    public void SaveBlockGuild(Toggle toggle)
    {
        Player player = Player.localPlayer;
        if (player != null)
            player.CmdBlockGuildInvite(toggle.isOn);
        PlayerPrefs.SetInt("BlockGuild", toggle.isOn ? 1 : 0);
    }

    // Set show overhead health and save its settings.
    public void SaveShowOverhead(Toggle toggle)
    {
        settingsVariables.isShowOverhead = toggle.isOn;
        PlayerPrefs.SetInt("ShowOverhead", toggle.isOn ? 1 : 0);
    }

    // Set show chat and save its settings.
    public void SaveShowChat(Toggle toggle)
    {
        settingsVariables.isShowChat = toggle.isOn;
        PlayerPrefs.SetInt("ShowChat", toggle.isOn ? 1 : 0);
    }

    // Set ui scale and save its settings.
    public void SaveUiScale(Slider slider)
    {
        for (int i = 0; i < uiScalable.Length; i++)
        {
            uiScalable[i].transform.localScale = new Vector3(slider.value, slider.value, 1);
        }

        PlayerPrefs.SetFloat("UiScale", slider.value);
    }

    #endregion Gameplay

    #region Video

    // Set the overall quality level and save its settings.
    public void SaveOverallQuality(Slider slider)
    {
        QualitySettings.SetQualityLevel((int)slider.value);
        PlayerPrefs.SetInt("OverallQuality", (int)slider.value);
    }

    // Set the texture quality level and save its settings.
    public void SaveTextureQuality(Slider slider)
    {
        switch ((int)slider.value)
        {
            case 0:
                QualitySettings.masterTextureLimit = 3;
                break;

            case 1:
                QualitySettings.masterTextureLimit = 2;
                break;

            case 2:
                QualitySettings.masterTextureLimit = 1;
                break;

            case 3:
                QualitySettings.masterTextureLimit = 0;
                break;
        }

        PlayerPrefs.SetInt("TextureQuality", (int)slider.value);
    }

    // Set the anisotropic level and save its settings.
    public void SaveAnisotropic(Slider slider)
    {
        switch ((int)slider.value)
        {
            case 0:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                break;

            case 1:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                break;

            case 2:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                break;
        }

        PlayerPrefs.SetInt("Anisotropic", (int)slider.value);
    }

    // Set the anti-aliasing level and save its settings.
    public void SaveAntiAliasing(Slider slider)
    {
        QualitySettings.antiAliasing = (int)slider.value;
        PlayerPrefs.SetInt("AntiAliasing", (int)slider.value);
    }

    // Set the soft particles and save its settings.
    public void SaveSoftParticles(Toggle toggle)
    {
        QualitySettings.softParticles = toggle.isOn;
        PlayerPrefs.SetInt("SoftParticles", toggle.isOn ? 1 : 0);
    }

    // Set the shadows level and save its settings.
    public void SaveShadows(Slider slider)
    {
        switch ((int)slider.value)
        {
            case 0:
                QualitySettings.shadows = ShadowQuality.Disable;
                break;

            case 1:
                QualitySettings.shadows = ShadowQuality.HardOnly;
                break;

            case 2:
                QualitySettings.shadows = ShadowQuality.All;
                break;
        }

        PlayerPrefs.SetInt("Shadows", (int)slider.value);
    }

    // Set the shadow resolution level and save its settings.
    public void SaveShadowResolution(Slider slider)
    {
        switch ((int)slider.value)
        {
            case 0:
                QualitySettings.shadowResolution = ShadowResolution.Low;
                break;

            case 1:
                QualitySettings.shadowResolution = ShadowResolution.Medium;
                break;

            case 2:
                QualitySettings.shadowResolution = ShadowResolution.High;
                break;

            case 3:
                QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                break;
        }

        PlayerPrefs.SetInt("ShadowResolution", (int)slider.value);
    }

    // Set the shadow projection level and save its settings.
    public void SaveShadowProjection(Toggle toggle)
    {
        if (toggle.isOn)
            QualitySettings.shadowProjection = ShadowProjection.StableFit;
        else
            QualitySettings.shadowProjection = ShadowProjection.CloseFit;

        PlayerPrefs.SetInt("ShadowProjection", toggle.isOn ? 1 : 0);
    }

    // Set the shadow distance and save its settings.
    public void SaveShadowDistance(Slider slider)
    {
        QualitySettings.shadowDistance = (int)slider.value;
        PlayerPrefs.SetInt("ShadowDistance", (int)slider.value);
    }

    // Set the shadow cascade level and save its settings.
    public void SaveShadowCascade(Slider slider)
    {
        QualitySettings.shadowCascades = (int)slider.value;
        PlayerPrefs.SetInt("ShadowCascades", (int)slider.value);
    }

    // Set the blend weight level and save its settings.
    public void SaveBlendWeight(Slider slider)
    {
        switch ((int)slider.value)
        {
            case 0:
                QualitySettings.skinWeights = SkinWeights.OneBone;
                break;

            case 1:
                QualitySettings.skinWeights = SkinWeights.TwoBones;
                break;

            case 2:
                QualitySettings.skinWeights = SkinWeights.FourBones;
                break;
        }

        PlayerPrefs.SetInt("BlendWeights", (int)slider.value);
    }

    // Set the vsync level and save its settings.
    public void SaveVsync(Slider slider)
    {
        QualitySettings.vSyncCount = (int)slider.value;
        PlayerPrefs.SetInt("Vsync", (int)slider.value);
    }

    // Set fullscreen and save its settings.
    public void SaveFullscreen(Toggle toggle)
    {
        Screen.fullScreen = toggle.isOn;
        PlayerPrefs.SetInt("Fullscreen", toggle.isOn ? 1 : 0);
    }

    // Set the resolution level and save its settings.
    public void SaveResolution(Dropdown dropdown)
    {
        Screen.SetResolution(resolutions[dropdown.value].width, resolutions[dropdown.value].height, Screen.fullScreen); //Set the game resolution to the dropdown value.
        PlayerPrefs.SetInt("Resolution", dropdown.value);
    }

    #endregion Video

    #region Sound

    // Set the music level and save its settings.
    public void SaveMusicLevel(Slider slider)
    {
        for (int i = 0; i < musicPlayed.Length; i++)
            musicPlayed[i].volume = slider.value;

        PlayerPrefs.SetFloat("MusicLevel", slider.value);
    }

    // Set the effects level and save its settings.
    public void SaveEffectLevel(Slider slider)
    {
        for (int i = 0; i < effectsPlayed.Length; i++)
            effectsPlayed[i].volume = slider.value;

        PlayerPrefs.SetFloat("EffectLevel", slider.value);
    }

    // Set the ambient level and save its settings.
    public void SaveAmbientLevel(Slider slider)
    {
        for (int i = 0; i < ambientPlayed.Length; i++)
            ambientPlayed[i].volume = slider.value;

        PlayerPrefs.SetFloat("AmbientLevel", slider.value);
    }

    // Set sound mute and save its settings.
    public void SaveSoundMute(Toggle toggle)
    {
        if (toggle.isOn)
        {
            for (int i = 0; i < musicPlayed.Length; i++)
                musicPlayed[i].mute = true;

            for (int i = 0; i < effectsPlayed.Length; i++)
                effectsPlayed[i].mute = true;

            for (int i = 0; i < ambientPlayed.Length; i++)
                ambientPlayed[i].mute = true;
        }
        else
        {
            for (int i = 0; i < musicPlayed.Length; i++)
                musicPlayed[i].mute = false;

            for (int i = 0; i < effectsPlayed.Length; i++)
                effectsPlayed[i].mute = false;

            for (int i = 0; i < ambientPlayed.Length; i++)
                ambientPlayed[i].mute = false;
        }

        PlayerPrefs.SetInt("SoundMute", toggle.isOn ? 1 : 0);
    }

    #endregion Sound

    #endregion Save Settings

    #region Load Settings

    // Load all of the player saved settings.
    private void LoadSettings()
    {
        LoadKeybindings();
        LoadGameplay();
        LoadVideo();
        LoadSound();
    }

    #region Keybinding

    // Loads all of the players saved keybindings.
    private void LoadKeybindings()
    {
        int keyCount = settingsVariables.keybindings.Length;
        for (int i = 0; i < keyCount; i++)
        {
            if (PlayerPrefs.HasKey("keybindings[" + i + "]"))
                settingsVariables.keybindings[i] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("keybindings[" + i + "]"));

            keybindingText[i].text = settingsVariables.keybindings[i].ToString();
            settingsVariables.keybindUpdate[i] = true;
        }
    }

    #endregion Keybinding

    #region Gameplay

    // Load all of the players saved gameplay settings.
    private void LoadGameplay()
    {
        LoadBlockTrades();
        LoadBlockParty();
        LoadBlockGuild();
        LoadShowOverhead();
        LoadShowChat();
        LoadUiScale();
    }

    // Load block trades and save its settings.
    public void LoadBlockTrades()
    {
        if (PlayerPrefs.HasKey("BlockTrades"))
        {
            if (PlayerPrefs.GetInt("BlockTrades") == 1)
                gameplayToggle[0].isOn = true;
            else
                gameplayToggle[0].isOn = false;
        }
        else gameplayToggle[0].isOn = false;

        Player player = Player.localPlayer;
        if (player != null)
            player.CmdBlockTradeRequest(gameplayToggle[0].isOn);
    }

    // Load block party invites and save its settings.
    public void LoadBlockParty()
    {
        if (PlayerPrefs.HasKey("BlockParty"))
        {
            if (PlayerPrefs.GetInt("BlockParty") == 1)
                gameplayToggle[1].isOn = true;
            else
                gameplayToggle[1].isOn = false;
        }
        else gameplayToggle[1].isOn = false;

        Player player = Player.localPlayer;
        if (player != null)
            player.CmdBlockPartyInvite(gameplayToggle[1].isOn);
    }

    // Load block guild invites and save its settings.
    public void LoadBlockGuild()
    {
        if (PlayerPrefs.HasKey("BlockGuild"))
        {
            if (PlayerPrefs.GetInt("BlockGuild") == 1)
                gameplayToggle[2].isOn = true;
            else
                gameplayToggle[2].isOn = false;
        }
        else gameplayToggle[2].isOn = false;

        Player player = Player.localPlayer;
        if (player != null)
            player.CmdBlockGuildInvite(gameplayToggle[2].isOn);
    }

    // Load show overhead health and save its settings.
    public void LoadShowOverhead()
    {
        if (PlayerPrefs.HasKey("ShowOverhead"))
        {
            if (PlayerPrefs.GetInt("ShowOverhead") == 1)
                gameplayToggle[3].isOn = true;
            else
                gameplayToggle[3].isOn = false;
        }
        else gameplayToggle[3].isOn = true;
    }

    // Load show chat and save its settings.
    public void LoadShowChat()
    {
        if (PlayerPrefs.HasKey("ShowChat"))
        {
            if (PlayerPrefs.GetInt("ShowChat") == 1)
                gameplayToggle[4].isOn = true;
            else
                gameplayToggle[4].isOn = false;
        }
        else gameplayToggle[4].isOn = true;
    }

    // Load ui scale and save its settings.
    public void LoadUiScale()
    {
        if (PlayerPrefs.HasKey("UiScale"))
        {
            gameplaySlider[0].value = PlayerPrefs.GetFloat("UiScale");
        }
        else gameplaySlider[0].value = 1;

        if (gameplaySlider[0].value == 0) gameplaySlider[0].value = 1;
    }

    #endregion Gameplay

    #region Video

    // Load all of the player video settings.
    private void LoadVideo()
    {
        LoadOverallQuality();
        LoadTextureQuality();
        LoadAnisotropic();
        LoadAntiAliasing();
        LoadSoftParticles();
        LoadShadows();
        LoadShadowResolution();
        LoadShadowProjection();
        LoadShadowDistance();
        LoadShadowCascade();
        LoadBlendWeight();
        LoadVsync();
        LoadFullscreen();
        LoadResolution();
    }

    // Load the overall quality level and set its settings.
    public void LoadOverallQuality()
    {
        if (PlayerPrefs.HasKey("OverallQuality"))
        {
            videoSliders[0].value = PlayerPrefs.GetInt("OverallQuality");
        }
        else videoSliders[0].value = 3;
    }

    // Load the texture quality level and set its settings.
    public void LoadTextureQuality()
    {
        if (PlayerPrefs.HasKey("TextureQuality"))
        {
            videoSliders[1].value = PlayerPrefs.GetInt("TextureQuality");
        }
        else videoSliders[1].value = 2;
    }

    // Load the anisotropic level and set its settings.
    public void LoadAnisotropic()
    {
        if (PlayerPrefs.HasKey("Anisotropic"))
        {
            videoSliders[2].value = PlayerPrefs.GetInt("Anisotropic");
        }
        else videoSliders[2].value = 1;
    }

    // Load the anti-aliasing level and set its settings.
    public void LoadAntiAliasing()
    {
        if (PlayerPrefs.HasKey("AntiAliasing"))
        {
            videoSliders[3].value = PlayerPrefs.GetInt("AntiAliasing");
        }
        else videoSliders[3].value = 1;
    }

    // Load the soft particles and set its settings.
    public void LoadSoftParticles()
    {
        if (PlayerPrefs.HasKey("SoftParticles"))
        {
            if (PlayerPrefs.GetInt("SoftParticles") == 1)
                videoToggles[0].isOn = true;
            else
                videoToggles[0].isOn = false;
        }
        else videoToggles[0].isOn = true;
    }

    // Load the shadows level and set its settings.
    public void LoadShadows()
    {
        if (PlayerPrefs.HasKey("Shadows"))
        {
            videoSliders[4].value = PlayerPrefs.GetInt("Shadows");
        }
        else videoSliders[4].value = 1;
    }

    // Load the shadow resolution level and set its settings.
    public void LoadShadowResolution()
    {
        if (PlayerPrefs.HasKey("ShadowResolution"))
        {
            videoSliders[5].value = PlayerPrefs.GetInt("ShadowResolution");
        }
        else videoSliders[5].value = 1;
    }

    // Load the shadow projection level and set its settings.
    public void LoadShadowProjection()
    {
        if (PlayerPrefs.HasKey("ShadowProjection"))
        {
            if (PlayerPrefs.GetInt("ShadowProjection") == 1)
                videoToggles[1].isOn = true;
            else
                videoToggles[1].isOn = false;
        }
        else videoToggles[1].isOn = true;
    }

    // Load the shadow distance and set its settings.
    public void LoadShadowDistance()
    {
        if (PlayerPrefs.HasKey("ShadowDistance"))
        {
            videoSliders[6].value = PlayerPrefs.GetInt("ShadowDistance");
        }
        else videoSliders[6].value = 80;
    }

    // Load the shadow cascade level and set its settings.
    public void LoadShadowCascade()
    {
        if (PlayerPrefs.HasKey("ShadowCascades"))
        {
            videoSliders[7].value = PlayerPrefs.GetInt("ShadowCascades");
        }
        else videoSliders[7].value = 1;
    }

    // Load the blend weight level and set its settings.
    public void LoadBlendWeight()
    {
        if (PlayerPrefs.HasKey("BlendWeights"))
        {
            videoSliders[8].value = PlayerPrefs.GetInt("BlendWeights");
        }
        else videoSliders[8].value = 2;
    }

    // Load the vsync level and set its settings.
    public void LoadVsync()
    {
        if (PlayerPrefs.HasKey("Vsync"))
        {
            videoSliders[9].value = PlayerPrefs.GetInt("Vsync");
        }
        else videoSliders[9].value = 1;
    }

    // Load the fullscreen and set its settings.
    public void LoadFullscreen()
    {
        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            if (PlayerPrefs.GetInt("Fullscreen") == 1)
                videoToggles[2].isOn = true;
            else
                videoToggles[2].isOn = false;
        }
        else videoToggles[2].isOn = true;
    }

    // Load the screen resolution and set its settings.
    public void LoadResolution()
    {
        if (PlayerPrefs.HasKey("Resolution"))
        {
            resolutionDropdown.value = PlayerPrefs.GetInt("Resolution");
        }
        else resolutionDropdown.value = resolutionDropdown.options.Count;
    }

    #endregion Video

    #region Sound

    // Load all of the player sound settings.
    private void LoadSound()
    {
        LoadMusicLevel();
        LoadEffectLevel();
        LoadAmbientLevel();
        LoadSoundMute();
    }

    // Load the music level and set its settings.
    public void LoadMusicLevel()
    {
        if (PlayerPrefs.HasKey("MusicLevel"))
        {
            soundSliders[0].value = PlayerPrefs.GetFloat("MusicLevel");
        }
        else soundSliders[0].value = 50;

#if _iMMOJUKEBOX
        UCE_Jukebox.singleton.SetVolume(soundSliders[0].value);

#endif
    }

    // Load the effects level and set its settings.
    public void LoadEffectLevel()
    {
        if (PlayerPrefs.HasKey("EffectLevel"))
        {
            soundSliders[1].value = PlayerPrefs.GetFloat("EffectLevel");
        }
        else soundSliders[1].value = 50;
    }

    // Load the ambient level and set its settings.
    public void LoadAmbientLevel()
    {
        if (PlayerPrefs.HasKey("AmbientLevel"))
        {
            soundSliders[2].value = PlayerPrefs.GetFloat("AmbientLevel");
        }
        else soundSliders[2].value = 50;
    }

    // Load sound mute and set its settings.
    public void LoadSoundMute()
    {
        if (PlayerPrefs.HasKey("SoundMute"))
        {
            if (PlayerPrefs.GetInt("SoundMute") == 1)
            {
                soundToggles[0].isOn = true;

                for (int i = 0; i < musicPlayed.Length; i++)
                    musicPlayed[i].mute = true;

                for (int i = 0; i < effectsPlayed.Length; i++)
                    effectsPlayed[i].mute = true;

                for (int i = 0; i < ambientPlayed.Length; i++)
                    ambientPlayed[i].mute = true;

#if _iMMOJUKEBOX
                UCE_Jukebox.singleton.Mute(true);

#endif
            }
            else
            {
                soundToggles[0].isOn = false;

                for (int i = 0; i < musicPlayed.Length; i++)
                    musicPlayed[i].mute = false;

                for (int i = 0; i < effectsPlayed.Length; i++)
                    effectsPlayed[i].mute = false;

                for (int i = 0; i < ambientPlayed.Length; i++)
                    ambientPlayed[i].mute = false;

#if _iMMOJUKEBOX
                UCE_Jukebox.singleton.Mute(false);

#endif
            }
        }
        else
        {
            soundToggles[0].isOn = false;

            for (int i = 0; i < musicPlayed.Length; i++)
                musicPlayed[i].mute = false;

            for (int i = 0; i < effectsPlayed.Length; i++)
                effectsPlayed[i].mute = false;

            for (int i = 0; i < ambientPlayed.Length; i++)
                ambientPlayed[i].mute = false;

#if _iMMOJUKEBOX
            UCE_Jukebox.singleton.Mute(false);

#endif
        }
    }

    #endregion Sound

    #endregion Load Settings

    #endregion Functions
}
