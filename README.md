# log8715-lab
Laboratory for the LOG8715 - Video Game Architecture course at Polytechnique Montréal.

## Lab 1 - Implementation ECS
This is a circle simulation application implemented using an ECS (Entity-Component-System) architecture.
### Functionnality
The application provides the following features:
- Circles: The circles have different sizes represented by an integer value.
- Dynamics: Some circles are dynamic, meaning they have a velocity and move on the screen.
- Statics: Some circles are static and do not move. These circles represent obstacles.
- Collisions: When two circles collide, the following events occur:
  - Circles bounce off each other using the CalculateCollision method in the CollisionUtility utility class.
  - If the circles have different sizes, the larger circle's size increases by 1 and the smaller circle's size decreases by 1.
  - If the circles have the same size, their sizes remain unchanged.
  - A collision between a static circle and a dynamic circle does not affect their sizes. Only the dynamic circle bounces, while the static circle remains stationary.
- Destruction: When a circle reaches a size of 0, it is destroyed.
- Explosions: When a circle reaches a predefined explosion size specified in the configuration, it explodes into two smaller circles moving in opposite directions. The resulting circles have sizes half of the original circle's size.
- Border Collision: When a circle collides with the screen border, it bounces off.
- Interaction: The user can click on a dynamic circle to either make it explode into two circles if its size is 2 or greater, or make it disappear if its size is 0.
- Time Rewind: By pressing the spacebar, the simulation state rewinds by 3 seconds (frame precision) with a cooldown period of 3 seconds. The cooldown state is displayed in the editor console if the spacebar is pressed before the cooldown ends
- Circle Colors: Circles have colors that change based on their state:
  - Static circles are red.
  - Dynamic circles without collisions are blue.
  - Dynamic circles with collisions are green.
  - Dynamic circles that will reach their maximum size on the next collision with a smaller circle are orange.
  - A clicked dynamic circle turns pink.
- Protection Mechanism: Circles with a size equal to or smaller than a value defined in the configuration can become randomly protected. The protected size is always smaller than the explosion size. At each frame, a circle with a size below the protected size has a probability defined in the configuration to become protected. A protected circle cannot change its size. Smaller circles colliding with the protected circle do not change their size. Larger circles colliding with the protected circle decrease their size by 1. The protection state of a circle expires after a delay defined in the configuration. After being protected, a circle cannot become protected again until a cooldown defined in the configuration has elapsed.

Please refer to the configuration file for customizable values and settings related to the simulation.

### Implementation Details
#### Components

The simulation utilizes several components to represent different aspects of the circles:

- `Position`: Stores the position of a circle on the screen.
- `Size`: Represents the size of a circle.
- `Color`: Determines the color of a circle based on its state.
- `Protection`: Indicates whether a circle is currently protected.
- `Rewind`: Stores the historical data of a circle for time rewind functionality.
- `Velocity`: Represents the velocity of a dynamic circle.

#### Tags

Tags are used to mark specific entities with certain characteristics or states:

- `IsInCollision`: Indicates that a circle is involved in a collision with another circle.
- `IsRewind`: Marks the entities that are eligible for time rewind.
- `IsClicked`: Indicates that a dynamic circle has been clicked.
- `IsStatic`: Marks static circles that do not move.

#### Systems

The simulation employs various systems to handle different functionalities:

- `BoundaryCollision`: Manages the collisions of circles with the screen boundaries.
- `CircleCollision`: Handles the collisions between circles, including size adjustments and bouncing behavior.
- `Color`: Manages the color of circles based on their states.
- `Explosion`: Handles the explosion of circles when they reach a specified size.
- `Input`: Handles user input, including mouse clicks and spacebar presses.
- `Movement`: Manages the movement of dynamic circles based on their velocity.
- `Rewind`: Facilitates the time rewind functionality by storing and retrieving historical data of entities.
- `Spawn`: Controls the spawning of new circles in the simulation.
