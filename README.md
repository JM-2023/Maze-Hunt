# Maze Hunt

### Group Members
- Mohamed Sulevani  
- Nikoli Oudo  
- Jimin Li  
- Jesus Serna  



**Halfway project review:**[Click Here](https://drive.google.com/file/d/1ZSAfCC6HPMZSVZJq6Y39NQoFOMhymFpC/view?usp=sharing)

---

## Description of Current Target for the Game
**Maze Hunt** is a 3D maze game where players navigate through an atmospheric environment to collect scattered props and evade enemies. The primary objective is to collect all required props to unlock the exit door and escape the maze, all while avoiding enemies with dynamic AI behavior. Players must rely on strategic movement, environmental awareness, and potential power-ups to achieve victory or face the game over screen upon capture.

---

## General Goals for Each Team Member
### Mohamed Sulevani
- **Enemy System**: 
  - Develop the enemy AI using Unity's NavMesh for pathfinding.
  - Implement enemy patrol, chase, and detection behaviors.
  - Configure increasing difficulty over time by introducing more enemies or faster movement speeds.
- **Audio Design**:
  - Create and integrate the dynamic heartbeat system.
  - Add sound effects for enemy growls and player footsteps.

### Nikoli Oudo
- **Maze Generation**:
  - Implement procedural maze creation using a grid-based algorithm (e.g., Recursive Backtracking).
  - Design modular wall and floor prefabs for maze construction.
  - Ensure maze dimensions and properties are customizable in the Unity Inspector.

### Jimin Li
- **Player Mechanics**:
  - Implement player controls (WASD for movement, mouse look for camera control, and jumping).
  - Develop the prop collection system, including collectible items and a counter UI.
  - Ensure smooth collision detection with maze walls and props.

### Jesus Serna
- **Game State and UI**:
  - Design and implement the win condition (unlocking the exit door upon collecting all props).
  - Develop the "Game Over" and "Victory" screens.
  - Add UI elements for prop collection count, player health, and game status.
- **Stretch Goals**:
  - Collaborate on optional features like hiding spots, power-ups, and level-based difficulty scaling.

---

#### **Game Objective**

- The player starts at the entrance of a procedurally generated 3D maze and must navigate through it to collect props (e.g., keys or tokens) scattered around the maze.
- Once all required props are collected, the exit door unlocks, allowing the player to escape.
- Enemies will chase the player within the maze, creating a challenging and engaging gameplay experience.

------

#### **Environment & Maze**

1. **Maze Creation:**
   - The maze should be procedurally generated using a grid-based algorithm (e.g., Recursive Backtracking or Prim’s Algorithm).
   - The maze is constructed from modular assets:
     - **Walls:** Prefab for maze walls (e.g., simple cubes).
     - **Floor Tiles:** Prefab for the ground of the maze.
   - The maze dimensions (width and height) should be customizable in the Unity Inspector.
2. **Visual Design:**
   - Basic materials for walls and floors (e.g., different colors or textures).
   - Optionally, include environmental lighting effects (e.g., dim or spooky lighting) to add atmosphere.

------

#### **Player Mechanics**

1. **Movement:**
   - The player should be able to move freely within the maze using `WASD` keys for movement and mouse for camera control.
   - The player can also jump to avoid obstacles or add a fun traversal element.
2. **Prop Collection:**
   - Props (e.g., keys, coins, or switches) are scattered throughout the maze.
   - The player must collect a specified number of props to unlock the exit door.
   - A UI counter shows how many props the player has collected.

------

#### **Enemies**

1. **Enemy AI:**
   - Enemies patrol the maze or randomly wander until they detect the player.
   - When the player enters an enemy’s detection range (e.g., through a line-of-sight check or proximity), the enemy begins chasing the player.
   - Enemies use Unity’s **NavMesh** for navigation and pathfinding within the maze.
2. **Enemy Behavior:**
   - Enemies should be faster(a little bit) than the player but with a limited detection range.
   - If the player escapes the detection range, the enemy reverts to patrolling or wandering.
   - Add an increasing difficulty curve by introducing more enemies or faster speeds over time.

------

#### **Victory & Game Over**

1. **Victory:**
   - The player wins by collecting all the props and reaching the exit door.
   - The exit door should open once the required props are collected, triggering a "victory" screen.
2. **Game Over:**
   - If an enemy catches the player, a "game over" screen appears, allowing the player to restart.

------

#### **Additional Features (Optional Enhancements)**

1. **Hiding Spots:**
   - Create safe zones or hiding spots where the player can evade enemies temporarily.
2. **Power-Ups:**
   - Include power-ups, such as:
     - Speed boosts to escape enemies.
     - Freezing enemies for a short time.
     - Temporary invincibility.
3. **Audio Design:**
   - [Maybe not a good idea]Background music to set the mood.
   - [We Need To Do This]**Heart Beat(the rate should based on the move speed or the distance of enemy)**
   - Enemy growls(Use Zombie?) and player footsteps for immersion.
4. **Timer:**
   - Add a timer to increase tension, where players must complete the maze within a certain time.

------

#### **Technical Details**

1. **Tools & Features:**
   - Unity’s **NavMesh** for enemy pathfinding.
   - A grid-based algorithm for maze generation (e.g., Recursive Backtracking).
   - Modular wall and floor prefabs for maze construction.
   - Basic UI for displaying the prop collection count, player health, and game status (win/lose).
2. **Customizability:**
   - Allow maze size (width and height) and the number of props/enemies to be adjustable via the Unity Inspector.
   - Support for multiple levels with increasing difficulty.





## Development Phases

### Phase 1: Core Maze Mechanics[Done]
- Procedural maze generation using simple grid-based algorithm
- Basic player movement (WASD + mouse look)
- Simple environment with basic materials
- Fundamental collision detection

### Phase 2: Gameplay Elements[Working]

- Mini Map
- Collectible items (keys/tokens) with counter UI
- Exit door that unlocks when all items are collected
- Basic win condition
- Simple game over screen

### Phase 3: Enemy and Atmosphere
- Single enemy with basic NavMesh pathfinding
- Enemy detection range and chase behavior
- Heartbeat sound system (varying with enemy proximity)
- Basic enemy patrol behavior

### Stretch Goals
- Additional enemies
- Hiding spots (simple trigger areas)
- Power-ups (speed boost, enemy freeze)
- Timer feature
- Multiple levels

## Technical Implementation Notes

### Core Features (Must Have)
1. **Maze Generation**
   - Using simple recursive backtracking algorithm
   - Modular wall and floor prefabs
   - Customizable maze size in Inspector

2. **Player Controller**
   - First-person camera control
   - Basic movement and jumping
   - Collision detection with maze walls

3. **Enemy System**
   - Unity NavMesh for pathfinding
   - Simple state machine (patrol/chase)
   - Detection range using sphere collider

4. **Audio System**
   - Dynamic heartbeat based on enemy distance
   - Basic sound effects for collecting items
   - Simple enemy sound effects
