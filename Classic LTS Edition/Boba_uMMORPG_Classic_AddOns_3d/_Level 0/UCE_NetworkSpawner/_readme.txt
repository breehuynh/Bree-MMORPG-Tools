a very simple network spawner to fix the "scene id not found" issue. it spawns prefabs after the scene is loaded.

1. place "UCE_NetworkSpawner" prefab anywhere in your scene
2. edit the list on the NetworkSpawner and add all objects and their locations where you want them to spawn
3. note that those objects can also be other types of spawners or trigger zones by themselves.
4. make sure those objects have a NetworkIdentity and are listed in your NetworkManagers spawnable prefabs
5. after spawning, the spawner destroys itself

note to coders:

1. no IENumerator on the spawn process yet, but could easily be added
2. for some reason that i dont remember, the spawn starts when the very first "update" is called instead of "start" or "awake".

