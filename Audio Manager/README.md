_A basic audio manager for playing sound effects and music._

**SETUP:**

1) Attach the AudioManager script to an empty GameObject in the scene.

2) Add entries to the SFX and Music arrays in the inspector, and drag your audio clips into the 'clip' field.

3) Call `AudioManager.Instance.Play("[Audio Name]")` to play looping/persistent sounds (can only play one instance of that sound at a time),
or `AudioManager.Instance.SpawnSFX("[Audio Name]")` for sounds that can occur in rapid succession (creates a prefab for each instance and randomizes pitch/volume).

4) Make sure you set the pitch & volume of each entry > 0, or you won't hear anything!