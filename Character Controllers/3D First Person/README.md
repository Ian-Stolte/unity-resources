_A first person character controller for a 3D game._

**SETUP:**

1) Create a player GameObject, give it a Rigidbody, and attach the `PlayerMovement` script.

2) Move the main camera so it is immediately in front of the player's head, and assign it to the `Cam` field.

3) Set the `GroundLayer` field to whichever objects your player can jump off of.

4) Tweak MoveSpeed, YawSpeed, PitchSpeed, Min/MaxPitch, and JumpForce as desired.