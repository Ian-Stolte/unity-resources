_A basic audio manager for playing sound effects and music._

**SETUP:**

1) Attach the  `AudioManager` script to an empty GameObject in the scene.

2) Attach the `DestroyAfterDelay` script to the `SFX` prefab from this folder, and assign that prefab to the `SFX Prefab` field in the AudioManager.

3) Add entries to the SFX and Music arrays in the inspector, and drag your audio clips into the `Clip` field.

4) Call `AudioManager.Instance.Play("[Audio Name]")` to play looping/persistent sounds (can only play one instance of that sound at a time), or `AudioManager.Instance.SpawnSFX("[Audio Name]")` for sounds that can occur in rapid succession (creates a prefab for each instance and randomizes pitch/volume).

5) Make sure you set the pitch & volume of each entry > 0, or you won't hear anything!