Introduction

The "Asset Bundle Downloader" AddOn allows you to add, manage and update your game content
(Art, Models, Textures, Sound, Music etc.) using the Unity Asset Bundles.

First of all, make sure you know how Asset Bundles work:

https://learn.unity.com/tutorial/assets-resources-and-assetbundles?_ga=2.236684993.1525409092.1573501000-332933643.1573501000

Second, prepare your projects assets to support asset bundles (see link above).

Now, download the free "Asset Bundle Magic" asset from Asset store and import it into
your project.

Check out the PDF that comes with "Asset Bundle Magic" for more details.

- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

Installation

IMPORTANT: This AddOn requires "AssetBundleMagic" (free) from the Unity Asset store.
Download and import it into your project first! When it is asking you to update API's
click "I made a backup, go ahead".

1. UI
1.1 Add everything from this AddOns "UI Prefabs" folder to your Canvas.

2. SCENE
2.1 Add "UCE_AssetBundleManager" object from this AddOns "Prefabs" folder to your scene.

Configuration

1. You can only have one "UCE_AssetBundleManager" object per scene.
2. You can set if the UCE_AssetBundleManager is active on Clients, Server or both (or none).
3. Each scene can have it's own, unique "UCE_AssetBundleManager" with unique assets to be loaded.
4. Now edit the inspector settings:

4.1 Setup your asset bundles via the "Asset Bundle Magic" component first.
4.2 Now in the "UCE_AssetBundleManager" component, enter the name of all bundles you want to load.
The names have to match exactly the names of your bundles in "Asset Bundle Magic".

