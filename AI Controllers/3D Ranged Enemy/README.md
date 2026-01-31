_A simple AI movement script for a ranged 3D enemy._

**SETUP:**

1) Create an enemy GameObject, give it a RigidBody, and attach the `AIRanged` script.

2) Add a gun (or other weapon) GameObject as a child of the enemy, and assign it to the `Gun` field.

3) Assign your player GameObject to the `Player` field.

4) Use the shooting bool to implement your own gun firing behavior (this script does not shoot on its own, it just sets this bool)

5) Tweak the fields in the inspector or the percentages in `SetDirection()` for different effects.