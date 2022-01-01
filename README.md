# Project PROJECT_NAME

### Student Info

-   Name: Max O'Malley
-   Section: 06

### Description

This project implements a basic implementation of the game asteroids in unity.  
It contains the functionality specified of the player controlling a ship that can shoot bullets and must avoid asteroids.
The asteroids can be shot with the bullets to split them apart, creating two smaller child asteroids.  
These child asteroids can then be shot to be destroyed.
Destroying asteroids grants the player points, with extra points being awarded for destroying child asteroids.
The game also has UI showing the score of the player, and the number of hits the ship can take before it is destroyed.


### User Responsibilities

-   _What the user will need to do in the program_
-   _Include user interaction: keys to press, mouse buttons to click/drag, etc._
-   Ship Acceleration: The ship will accelerate when either the W key or the up arrow key are pressed.
-   Ship Turning: The ship can be turned left or right by pressing the left or right arrow keys respectively.  The A and D keys will also turn the ship.
-   Shooting Bullets: The ship will fire bullets either when the space bar or left mouse button is pressed.  There is a delay to firing bullets.
-   Ship Lives: When the ship is hit by an asteroid, it will turn red and become immune to damage for a few seconds.  Afterwards, it will be vulnerable again.
                The ship will also be destroyed if it hits zero lives, at which point the game is over.

### Known Issues

_List any errors, lack of error checking, or specific information that I need to know to run your program_
There is one bug which occurs occassionally whenever an asteroid or a bullet reaches around the middle of the screen,
where it will act as though it collided with something.  I'm not entirely certain what's causing this but the issue isn't 
common and the collision detection otherwise works well in every other case.  

It is also worth noting that once the ship is destroyed, the game will need to be reloaded to start it again, as
I didn't include any functionality for restarting the game, given that it wasn't specified as required in the project requirements.


### Requirements not completed

_If you did not complete a project requirement, notate that here_

### Sources

-   Asteriods and Ship Sprites: https://ansimuz.itch.io/patreons-top-down-collection
-   Bullet Sprite: https://www.kenney.nl/assets/pixel-shmup
-   Background Sprite: https://ansimuz.itch.io/patreons-top-down-collection
