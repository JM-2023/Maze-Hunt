# Maze Hunt

**Halfway project review:**[Click Here](https://drive.google.com/file/d/1ZSAfCC6HPMZSVZJq6Y39NQoFOMhymFpC/view?usp=sharing)

---

## Description of Current Target for the Game
**Maze Hunt** is a 3D maze game where players navigate through an atmospheric environment to collect scattered props and evade enemies. The primary objective is to collect all required props to unlock the exit door and escape the maze, all while avoiding enemies with dynamic AI behavior. Players must rely on strategic movement, environmental awareness, and potential power-ups to achieve victory or face the game over screen upon capture.

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
- Procedural maze generation using simple grid-based algorithm[Done]
- Basic player movement (WASD + mouse look)[Done]
- Simple environment with basic materials[Done]
- Fundamental collision detection[Done]

### Phase 2: Gameplay Elements[Done]

- Mini Map [Done]
- Collectible items (keys/tokens) with counter UI [Done]
- Exit door that unlocks when all items are collected[Done]
- Basic win condition[Done]
- Simple game over screen[Done]

### Phase 3: Enemy and Atmosphere[Done]
- Single enemy with basic NavMesh pathfinding[Done]
- Enemy detection range and chase behavior[Done]
- Heartbeat sound system (varying with enemy proximity)[Done]
- Basic enemy patrol behavior[Done]

### Stretch Goals[Done]
- Additional enemies[Done]
- Hiding spots (simple trigger areas)[Drop]
- Power-ups (speed boost, enemy freeze)[Done]
- Timer feature[Drop]
- Multiple levels[Drop]

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
