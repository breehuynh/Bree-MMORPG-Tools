
# Instructions

## Boba Utils Installation (REQUIRED)
1. Download the knowledge-base.zip
2. Looks for Tools (UCE Tools) in the Knowledge Base and follow the same instructions to install Boba Utils.
3. Once you followed the steps for UCE Tools, in order to completely finish the installation for Boba Utils you need to proceed with the following steps.
4. Open up ExampleDevExtMethods.cs and read and follow the instructions. This is how I will be adding functionality to the core with minimal core changes. As you can see, it is very similar to the Utils Hook ons from the Classic Edition, but these hooks are much more optimized and faster.
5. Congrats! Once you followed the instructions, you've successfully installed Boba Utils for your project. Every addon will use these utilities.

## Component Edition Addons Installation 
6. Now that you've installed Boba Utils, the following steps will guide you through how to install the Component Edition addons.
7. Download any Component Edition addon you wish and search through the Knowledge Base for the instructions to install your respective addon.
8. Once you've followed the installation steps from the Knowledge Base for your addon, proceed with the following steps.
9. Double click any script in your Unity Project in order to open up your External IDE. 
10. Press this combination to search through your project solution: Ctrl+Shift+F (CMD+Shift+F for Mac)
11. Search for DevExtMethods
12. If you do not see anything then that means you've completely finish the installation for your addon. Go play!
13. If not, then you will need to follow the steps similar to the ones in ExampleDevExtMethods.
14. For example, click on any DevExtMethod from your search results.
15. In the script, you will see methods with a \[DevExtMethod] attribute. Remember the name of this method.
16. Go to the top of the script and look for the class name. For instance, it may be called "public partial class Player"
17. In that case, open up Player.cs and look for the same method with the same prefix of the method name that I told you to memorize.
18. Add the invoke hook (look through the methods in ExampleDevExtMethods.cs for examples on how to do this).
19. You will need to add these invoke hooks for every DevExtMethod in your addon.
20. Congrats! Once you've added all these DevExtMethod hooks, you've successfully installed your Component Edition addon. Whew! 

I know. It's a lot more work than the Classic Edition addons. Another alternative is manually looking through the DevExtMethods and adding in the Events via the inspector individually. I don't like this method but it's up to you.

