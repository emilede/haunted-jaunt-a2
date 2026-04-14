# Haunted Jaunt
Modified Version by: 

Emile Delisle

## New Gameplay Elements

All new behavior lives in [Observer.cs](Assets/UnityTechnologies/3DBeginnerTutorialComplete/Scripts/Observer.cs). 

### Dot Product — Vision Cone
Each ghost now has a vision cone based on the direction they are looking. The script compares the ghost's forward vector against the normalized direction to the player. 

The player is only seen if they are inside the cone and a direct line between the player and ghost exists (so that player can hide behind walls ect.)

### Linear Interpolation — Suspicion Meter
Added suspicion meter, where when a player is in the ghosts line of site, the suspicion bar goes up. This value is lerped from 0 to 1 when the player is in view and then declines back to zero if the player isn't in view. Once you've been spotted the ghost transitions to the chase state, leaving it's patrol area and glowing red visually. The ghost will chase the player until he is caught. 

### Particle Effect — Blood Burst on Catch
When a ghost catches a player, a blood burst particle effect is played. 

### Sound Effect — Chase Stinger
Added a piano chase stinger sound effect when the ghost changes from patrolling to chasing state.

## Running
Open the project in Unity 6 (6000.3.11f1) and press Play on `Assets/UnityTechnologies/3DBeginnerTutorialComplete/Scenes/MainScene.unity`.
