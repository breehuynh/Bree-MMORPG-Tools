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
using Jacovone.AssetBundleMagic;

// ===================================================================================
// AssetBundleManager (SINGLETON)
// ===================================================================================
public class UCE_AssetBundleManager : MonoBehaviour
{

    public static UCE_AssetBundleManager singleton;

    [Header("Options")]
    public bool activeOnServer;
    public bool activeOnClient;

    [Header("Bundles")]
    public string[] bundleNames;

    [Header("Labels")]
    public string labelUncache = "Unloading cache...";
    public string labelVersions = "Updating versions numbers...";
    public string labelBundles = "Starting bundle download... ";
    public string labelBundle = "Downloading Bundle: ";

    [HideInInspector]public int index = -1;
    private AssetBundleMagic.Progress p = null;

    // -------------------------------------------------------------------------------
    // Awake
    // -------------------------------------------------------------------------------
    private void Awake()
    {
        if (singleton == null) singleton = this;
        Invoke(nameof(StartBundleManager), 1f);
    }

    // -------------------------------------------------------------------------------
    // Update
    // -------------------------------------------------------------------------------
    void Update()
    {
#if _CLIENT
        if (index == -1 || index > bundleNames.Length - 1)
        {
            UCE_UI_AssetBundleDownloader.singleton.Hide();
        } else if (p != null) {
            float fProgress = Mathf.Max(0, p.GetProgress());
            string sText = ((int)(100 * fProgress)).ToString() + "%";
            UCE_UI_AssetBundleDownloader.singleton.UpdateUI("", fProgress, sText);
        }
#endif
    }

    // ===============================================================================
    // GENERAL FUNCTIONS
    // ===============================================================================

    // -------------------------------------------------------------------------------
    // StartBundleManager
    // -------------------------------------------------------------------------------
    protected void StartBundleManager()
    {
        DownloadVersions();
    }

    // -------------------------------------------------------------------------------
    // CheckBuildTarget
    // -------------------------------------------------------------------------------
    protected bool CheckBuildTarget()
    {

        if (bundleNames.Length <= 0) return false;

#if _SERVER
        if (activeOnServer) return true;
#endif

#if _CLIENT
        if (activeOnClient) return true;
#endif
        return false;

    }

    // -------------------------------------------------------------------------------
    // UnloadBundles
    // -------------------------------------------------------------------------------
    protected void UnloadBundles()
    {
        if (!CheckBuildTarget()) return;

#if _CLIENT
        UCE_UI_AssetBundleDownloader.singleton.UpdateUI(labelUncache);
#endif

        AssetBundleMagic.CleanBundlesCache();

        foreach (string bundleName in bundleNames)
            AssetBundleMagic.UnloadBundle(bundleName, true);
    }

    // -------------------------------------------------------------------------------
    // DownloadVersions
    // -------------------------------------------------------------------------------
    protected void DownloadVersions()
    {

        if (!CheckBuildTarget()) return;

#if _CLIENT
        UCE_UI_AssetBundleDownloader.singleton.UpdateUI(labelVersions);
#endif

        AssetBundleMagic.DownloadVersions(delegate (string versions) {
            LoadBundles();
        }, delegate (string error) {
            Debug.LogError(error);
        });
    }

    // -------------------------------------------------------------------------------
    // LoadBundles
    // -------------------------------------------------------------------------------
    protected void LoadBundles()
    {

        if (!CheckBuildTarget()) return;

        UnloadBundles();

#if _CLIENT
        UCE_UI_AssetBundleDownloader.singleton.UpdateUI(labelBundles);
#endif

        LoadNextBundle();

    }

    // -------------------------------------------------------------------------------
    // LoadBundle
    // -------------------------------------------------------------------------------
    protected void LoadBundle(string sBundleName)
    {

        if (!CheckBuildTarget()) return;

#if _CLIENT
        UCE_UI_AssetBundleDownloader.singleton.UpdateUI(labelBundle + sBundleName);
#endif

        p = AssetBundleMagic.DownloadBundle(
                    sBundleName,
                    delegate (AssetBundle ab2) {
                        LoadNextBundle();
                    },
                    delegate (string error) {
                        Debug.LogError(error);
                    }
                );

    }

    // -------------------------------------------------------------------------------
    // LoadNextBundle
    // -------------------------------------------------------------------------------
    protected void LoadNextBundle()
    {
        if (!CheckBuildTarget()) return;

        index++;

        if (index > bundleNames.Length-1)
        {
#if _CLIENT
            UCE_UI_AssetBundleDownloader.singleton.Hide();
#endif
            index = -1;
            UnloadBundles();
        }
        else {
            LoadBundle(bundleNames[index]);
        }

    }

    // -------------------------------------------------------------------------------
}

// ===================================================================================
