Install

1. Attach "Patreon" component to each Player Prefab
2. Enter credentials as provided by patreon
3. Check the included "Readme.pdf" of original Patreon API for unity (free) for further details
4. See new section on player prefab "Patreon Manager", edit update time in seconds there
5. The update time interval can be very high, it only revalidates the token to prevent problems on monthly turnover (subscription cancelled but the player is still treated as having one)
6. Use in conjunction with any "UCE Interactable" / "UCE Requirement"
7. All "UCE Requirements" now feature a new field "Required Minimum Patreon Subscription"
8. Enter desired amount there

Best Example on how to use it:

Make a teleporter using "UCE Teleporter" and add a "Patreon minimum subscription" to it.
Now, only players with a active pledge of that amount or higher will be able to use the teleporter.
"Hide" all premium zones and their content behind that teleporter.