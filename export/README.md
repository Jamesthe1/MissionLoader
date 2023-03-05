# MissionLoader
A BepInEx plugin that loads modded missions for House of the Dying Sun.
# Using as a library
> NOTE: You will need to have a `SortieTemplate`. It is recommended that the game is decompiled first and an AssetBundle is created.

> NOTE: If your node depends on a mission from another mod, please add a `BepInDependency` to that mod in your plugin so that yours loads after it. This mod does not work well with cyclic dependencies.

MissionLoader comes pre-packed with an event to detect when a scene is completely ready to spawn objects in. This is useful for loading objects from an AssetBundle, as well as spawning new GameObjects.
1. At the top of your plugin script, add `using MissionLoader`.
2. Create a method called `OnSceneReady` in your plugin class with no arguments.
3. Inside this method:
	1. Add the line `NodeSpawner.SceneReady -= OnSceneReady`.
	2. Load your AssetBundle(s) here, if you have any.
	3. Create a `NodeFactoryDatum` with your `SortieTemplate`(s), a list of the GameObject names of the nodes you want this to connect to, and the position of your node on the map.
	4. Call `NodeSpawner.ReadyNodes` and input a list of your `NodeFactoryDatum`(s).
4. In the `Awake` method of your plugin, add the line `NodeSpawner.SceneReady += OnSceneReady`.

You should now have your node(s) spawn in the game with the connections you specified.