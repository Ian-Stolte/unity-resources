_A procedural generation system for tiled, top-down 2D levels, using Unity's Tilemap system._

**SETUP:**

1) Add the `TilemapGeneration` prefab to your scene, and attach the `TilemapGeneration` script to it.

2) Set the `Map` paramater to the first child of the prefab (named Walls). Take the `Rule Tile.asset` from this folder and assign it to the `Rule Tile` parameter.

3) You should be able to start the game and see a procedurally-generated level appear. Press G or call the Generate() function to generate a new level. To see the whole level, you may want to set your camera's size to around 20 and XY position to (30, 20).

4) Customize the `Number Of Steps` parameter for a longer or shorter level, and the `Brush Size` for wider/smoother rooms or narrower, more chaotic pathways. Increase `Grid Size` if your levels are overflowing off the edge of the tilemap.

5) If you want, replace the sprites in the Rule Tile to customize the appearance with your own art.


**SPIKES SETUP (OPTIONAL):**

1) To also generate spikes along flat edges of the level, first set the `Spawn Spikes` parameter to true. Then set the `Spike Map` paramater to the second child of the prefab (named Spikes).

2) Navigate to the Assets subfolder of this folder and find the 4 spike assets (named `SpikeTile 1` and so on). Assign these to the `Spike Tiles` parameter.

3) Customize the `Spike Pct` paramater for your desired number of spikes, or replace the sprites for the SpikeTiles to change them to your own art.


**ENEMIES SETUP (OPTIONAL):**

1) If you want to spawn enemy prefabs (or any prefabs) in the empty spaces of the level, first set the `Spawn Enemies` parameter to true.

2) Create an empty GameObject to act as the parent for all spawned prefabs, and assign it to `Enemy Parent`. Assign the prefab you want to spawn to `Enemy Prefab`.

3) Set `Min Enemies` and `Max Enemies` for your desired number of spawns. Note that it's possible for the algorithm to fail to find enough empty spots, so it may occasionally spawn fewer prefabs than requested.


**PLAYER SETUP (OPTIONAL):**

1) To place your player (or any GameObject you want) in a suitable empty location, set `Place Player` to true and assign your player GameObject to `Player`.