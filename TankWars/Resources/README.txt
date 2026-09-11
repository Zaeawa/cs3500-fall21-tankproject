PS9

Design Decisions
- The settings file can't modify game sizes because that might conflict with the clients connecting to the server
- All the dead objects are removed and the cleanup work is in a separate helper method.This is done at the end so the server can the send the client the dead objects before removing them.
- theWorld has two separate constructors - one for the view and the other for the server. This is done so that we can modify different game attributes like speed, etc. and not worry about storing the view related data, like death explosion.

Unique Features
- We can change all game attributes except object sizes.

-------------------------------------------------------------------------------------------------------------------------
PS8

DESIGN DECISIONS
- The project was built from using a lot of the game lab code as a starting point. This is why, for example, we build the buttons/text fields/etc.
from scratch instead of using the designer.
- Our controller handles all networking-related aspects
- Our world project contains all the different game objects
- Our resources contains the dll for our PS7 code, as well as all the sprites. Animations are grouped into their own sub folders
- Because drawingpanel is the only program that uses the images, they are loaded there
- Constans is not used much at the current time, but that may change in PS9

UNIQUE FEATURES
- Rainbow bullets (because why not)
- Laser looks kinda like an actual laser
- Health Bar are very visible now, remaining hp indicated by color and size of bar
- Upon death, players explode and leave behind their remains until they respawn