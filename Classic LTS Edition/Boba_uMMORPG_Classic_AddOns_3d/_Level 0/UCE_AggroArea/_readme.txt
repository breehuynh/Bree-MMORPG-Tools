Install:

- remove old AggroArea script from monster prefab -> Aggro Area game object
- add new UCE_AggroArea script to monster prefab -> Aggro Area gameObject
- done!

What it does:

+ Triggers only when the target can actually be attacked
+ Does a NavMesh Raycast and does not trigger if the target is behind a wall etc
+ You do not have to assign "owner" manually anymore