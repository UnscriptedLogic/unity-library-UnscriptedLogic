## What
- A library that houses common logic across games. It currently contains logic for 
  - common math and randomizing operations
  - common raycasting operations
  - a simple currency handler
  - an endless spawning system

## Why
- To store my re-usable code for rapid prototyping or for participating in game jams (that allow re-using code. No it's not cheating I swear) 
- To hide the logic somewhere else, only exposing what is really needed.

### PointBasedWaveSystem.cs
![image](https://user-images.githubusercontent.com/86519190/220220904-1f069d05-2e04-4ecc-a9eb-21a0845f32bf.png)

### Using the wave system from the library
![image](https://user-images.githubusercontent.com/86519190/220223058-5f5bfb26-0ae0-40be-be65-05114a316fd5.png)

## How
- Put the .dll found in the releases tab on the right in 'Assets/Plugins' in your Unity project. Create the folder if there isn't one because Unity looks for this specific folder with this specific name at that specific folder heirarchy. Altering the folder's name or heirarchy will prevent Unity from registering that it's a .dll and may not initialize it properly or even at all when binding Unity to your IDE.

  - I understand that this may no longer be required and Unity will continue to initialize DLLs regardless of where they are in the project but it seems that old traces of the legacy system remain involving script compilation order when examining the folders. Just to be safe, I've stated what is currently working for me.
  
## Documentation
To Do
