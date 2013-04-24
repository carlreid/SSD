/////////////////////////////////////////////////////////
/							/
/	  A game based on Super Stardust HD		/
/	     Some content is copyrighted.		/
/	     Please read bottom of README		/
/							/
/////////////////////////////////////////////////////////



Milestone 10 (24/04/2013) - ICA Hand in

New Functionality
-----------------

- Bitmap Fonts
- Power Ups
- Spawn Progression
- Menus
- Difficulty
- Snake Boss

Code Additions
--------------

- Added Bitmap font support to get a nicer font working for the GUI.
- Power ups of various things to tweak player speed, shooting and what not.
- Spawn progression per level. Should be easy to add future levels.
- Difficulty menu tweaks a DifficultyManager which is called in various code areas.

Tests
-----

- Visual tests on the Bitmap font.
- Some character tests on the Bitmap font.
- Menu navigation testing

Notes
-----
Bitmap font stuff used from http://www.craftworkgames.com/blog/tutorial-bmfont-rendering-with-monogame/

Also this is the "final" hand in. Wish I could do more but there's been so many ICAs and more to go...
______________________________________________________________________________________________________

Milestone 9 (20/04/2013)

New Functionality
-----------------

- Bombs
- Player Death
- GUI

Code Additions
--------------

- Made it so the player's death is handles properly. Also sounds and particles.
- Some of the GUI is starting to take shape.

Tests
-----

- The bombs were tested quite a bit to get the best radius. I think it's pretty good now.

Notes
-----
Started creating some folders since there's getting to be a few classes now.
______________________________________________________________________________________________________

Milestone 8 (14/04/2013)

New Functionality
-----------------

- Spawn Manager
- Mine Enemy
- Fire/Ice Elements
- Some more sounds
- Particle changes

Code Additions
--------------

- Tweaks to entities for elements
- FireBullet.cs
- IceBullet.cs

Tests
-----

- Mainly visual but also checks to see if the new bullets are working like the old ones.
- Also tests to see if elements do more damage to the opposite one.

Notes
-----
Also moved the XACT project into the correct place so it actually compiles when you run the game.
______________________________________________________________________________________________________

Milestone 7 (19/03/2013)

New Functionality
-----------------

- Audio
- Bloom

Code Additions
--------------

- SoundManager.cs
- SoundAttacher.cs

Tests
-----

- Some audio checks to check 3D sound working

Notes
-----
Also added a "Play Sphere" which is more of an idea of what the player is on. Also added bloom which
is from the Microsoft source at http://xbox.create.msdn.com/en-US/education/catalog/sample/bloom
______________________________________________________________________________________________________

Milestone 6 (17/03/2013)

New Functionality
-----------------

- More of a massive restructure

Code Additions
--------------

- ModelEntity.cs

Tests
-----

- Just checked to see if it was working like before

Notes
-----
This was mainly a restructure of how entities work in the inheritence hierarchy.

Aim for next milestone is to try get XACT working for sound.

______________________________________________________________________________________________________


Milestone 5 (10/03/2013)

New Functionality
-----------------

- Bounding Spheres
- BoundingSphereRenderer

Code Additions
--------------

- BoundingSphereRenderer.cs
- Bounding Sphere to entity

Tests
-----

- Checked various collisions
- Visual checks from the renderer

Notes
-----
Bounding sphere rendering code is from http://codingquirks.com/2011/01/render-boundingsphere-in-xna-4-0/
so credit to to codingquirks.

______________________________________________________________________________________________________

Milestone 4 (02/03/2013)

New Functionality
-----------------

- Entity Class
- Camera Fix/Working

Code Additions
--------------

- Updates to above classes

Tests
-----

- Tested the camera by going around the sphere and seeing if there were any glitches appearking.

Notes
-----
The camera fix was done by setting the ships yaw to 0, set the camera's up to the ship's forward then
applying the old yaw. Seems like a cheap fix though.

______________________________________________________________________________________________________

Milestone 3 (25/02/2013)

New Functionality
-----------------

- "First Person" Camera
- Terrain generation

Code Additions
--------------

- Terrain.cs (Kieth's code)

Tests
-----

- Visual checks to see if camera working correctly and terrain generating correctly.

Notes
-----
The functionality added here I won't be adding into my game. I simply added them to meet milestone 3.
I may end up attempting a first person camera for some future reason though.

______________________________________________________________________________________________________

Milestone 2 (23/02/2013)
--------------------------------

New Functionality
-----------------

- Multiple 3D Models

Code Additions
--------------

- ModelContainer which stores all model data and functionality.

Tests
-----

- Visual tests to see if the model can be rotated/moved.
- Loaded more than one model

Notes
-----
None.

______________________________________________________________________________________________________

Milestone 1 (14/01/2013)
--------------------------------

New Functionality
-----------------

- Can draw sprite
- Move sprite
- Output debug text to screen/console.

Code Additions
--------------

- Basic 2D drawing

Tests
-----

- Visual tests to see if the sprite+text was drawn

Notes
-----
None.


______________________________________________________________________________________________________

References and things


http://www.youtube.com/watch?v=RGTdHFgUXWA

Font for GUI/UI:
Used tutorial: http://www.craftworkgames.com/blog/tutorial-bmfont-rendering-with-monogame/
Used Software: http://www.angelcode.com/products/bmfont/
Used Font: http://www.dafont.com/starduster.font (Stardusteracad/Starduster Academy)       Previous: http://www.dafont.com/ethnocentric.font
Modidified to allow scale, colour change and right to left (to draw text at right of viewport)


Sounds/Music:
https://acapela-box.com/AcaBox/index.php - Nelly (US) - Voice
http://www.youtube.com/watch?v=u1aYffM0NqQ - Robotic voice guide Audacity
http://www.bfxr.net/ - Make sounds


https://soundcloud.com/uvrecordings/new-edge-mirrors-version
http://soundcloud.com/tracks/search?q[genre_tags]=+%22%22+%22%22+%22%22+%22%22+%22%22Breakbeat%22%22+%22%22Electronic%22%22&q[type]=&q[duration]=&q[cc_licensed]=1&advanced=1

Xbox Icons:
http://xbox.create.msdn.com/en-US/education/catalog/utility/controller_buttons

Universe Images:
http://joejesus.deviantart.com/art/Resemble-300196630
http://tylercreatesworlds.deviantart.com/art/Eye-for-an-Eye-363414225?q=gallery%3Atylercreatesworlds%2F14822827&qo=0

Menu:
Background: http://tylercreatesworlds.deviantart.com/art/Eye-for-an-Eye-363414225?q=gallery%3Atylercreatesworlds%2F14822827&qo=0
Font: Starduster (http://www.dafont.com/starduster.font)


Bullet Sounds:
Ice:
- Fire:   http://www.pond5.com/sound-effect/8620656/created-magic-spell-ice-bolt03-stereo.html
- Impact: http://www.pond5.com/sound-effect/8620673/created-magic-spell-ice-bolt-impact10-stereo.html
Fire:
- Fire:   http://www.pond5.com/sound-effect/8620635/created-magic-spell-fireball-impact03-stereo.html
- Impact: http://www.pond5.com/sound-effect/8620634/created-magic-spell-fireball-impact02-stereo.html


Mine Entity:
Loop Sampled:    http://www.pond5.com/sound-effect/18334366/computer-bleep-sound-effect06.html
Explode Sampled: http://www.pond5.com/sound-effect/10774959/explosion-006.html

Rock Entity:
Move Loop: Noise from Audacity
Explode Sampled: http://www.pond5.com/sound-effect/8616619/rock-dirt-slide-small24.html

Player Entity:
Boost Sampled: http://www.pond5.com/sound-effect/219086/sci-fi-engine-power.html
Death Sampled: http://www.pond5.com/sound-effect/706759/sci-fi-firework.html
Bomb Sampled: http://www.pond5.com/sound-effect/8745902/explosionbomb.html


Announcer:
Vocals from https://acapela-box.com/AcaBox/index.php (Nelly (US))
Fire Activated: http://www.pond5.com/sound-effect/6220188/fireball-flame-04.htmlyVel
Ice Activated:  http://www.pond5.com/sound-effect/8157800/ice-smash-light-02.html
Spawn Alarm: http://www.pond5.com/sound-effect/22608068/alarm-01.html

Other (Done by myself):
Planet: 3D Model + Texture
Universe: 3D Model
Ice Rock: 3D Model + Texture
Bullets (All): 3D Model + Texture
