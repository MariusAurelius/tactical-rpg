# Tactical RPG

A Tactical Role-Playing Game (RPG) built in Unity, featuring team-based mechanics, AI-controlled units, and customizable game settings. 
This project demonstrates the use of Unity's NavMesh system, message-based communication between units, and dynamic scene management.

## Features
- **Team Management**: Units are divided into teams and subteams, with leaders dynamically assigning tasks and destinations.
- **AI Behavior**: Units exhibit behaviors such as wandering, attacking, and retreating based on their state and surroundings.
- **Dynamic Scene Management**: Scenes are loaded dynamically based on user-defined settings like the type of map or the number of troops of a certain type.
- **Customizable Settings**: Includes a settings menu for adjusting audio, graphics, and resolution.
- **Message-Based Communication**: Units communicate using a message system to coordinate actions like attacking in groups or moving to specific locations.

## How It Works
1. **Team and Subteam Management**:
  - Units belong to 1 of 2 teams (Red or Blue) and are divided into subteams.
  - Leaders assign destinations and tasks to their subteam members.

2. **AI Behaviors**:
  - Units dynamically switch between behaviors such as wandering, attacking, and retreating based on their state and environment.
  - Leaders communicate with subteam members using a message-passing system.

3. **Scene Management**:
  - Scenes are loaded dynamically using the ScenesManager class.
  - Game settings (selected in the GameSettingsMenu scene) determine which scene to load and how many troops of each type to spawn, with fallback options for invalid configurations.

4. **Customizable Settings**:
  - The GeneralSettingsMenu scene allows users to adjust audio, graphics, and resolution settings.
  - Settings are saved and applied across scenes.

## Running the Game in the Unity Editor
1. Open and start the MainMenu scene in Unity.
2. Press the Play button to start the game, or change your settings first.

### Controls (once in the game scene)
- **Space**: Start the game and spawn units.
- **R**: Reset the current scene.
- **P**: Pause or resume the game.
- **Escape**: Exit to the main menu.




