

# Smolbean Ideas

I have decided I want the game to be, eventually, a cross between The Settlers and Rimworld. The aim overall will be to manage a colony of people with increasingly complex needs on a small, resource constrained island.  There won't be an "enemy" as such - in fact, the colonists are their own worst enemies - because their greed could cause them to cut down the last tree or kill the last animal for a tasty snack.  Just like real life, mankind (smolbean kind) will cause it's own downfall 99% of the time.  So resource chains and building-based production like the settlers, but I'd like the player to _care_ about their colonists, thus the Rimworld vibe.  Anyway... a long way to go...

# TO DO List

1. ~Add a stonecutter and get them cuttin' stones~
2. ~Set up the build menu to create buttons automatically, perhaps from some scriptable object data (prefab, thumbnail, building data etc).~
3. Animals (dodos I think) and a hunter
4. Drops!  Trees drop logs, rocks drop stones.  Wood/Stonecutters collect these and dump them by their huts, until there's no more room.
5. Separate people from their buildings - currently, the colonists lifespans are controlled by their buildings.  Do this centrally.
6. Create rigged models for the colonists, animate and import them.  Colonists dress the way they dress, they don't change clothes based on their job.
7. ~Get some decent GPU-instanced grass on the go, to make the ground look more professional.~
8. ~Sort out the lighting so the buildings look less lame.~ (can't get much better without baking)
9. Add time of day as a concept, including day/night cycle.
10. Warehouse and Porters (new job type - carries stuff round the island as needed).  Maybe you start with the "Warehouse" as a big pile of resources and a few people gathered round a camp fire.  Maybe a ship wreck?
11. Building of buildings - so porters and builders create the buildings using resources.
12. First production chain buildings - Sawmill? Butcher?
13. Lights and lighting effects on buildings - colonists carrying torches at night etc.
14. Colonist stats - e.g. stronger people can carry more.  Cutting wood makes you strong etc.
15. Graphs and charts showing progress over time.
16. ~Rotation and deletion of buildings~
17. ~Ground wear - as colonists (and animals?) move around the map, record their steps on a texture.  Use this texture in a shader to start to show wear on the grass.  This should lead to areas of high traffic starting to get paths appearing on them...~
18. ~Save camera position with game~
19. Save ground wear pattern with game
20. Buildings add instant ground wear when placed (how to stop the grass regrowing under buildings...?)
21. Grass is shorter where ground is worn
22. A world map - overlaying the wear texture, buildings and colonist locations etc onto the existing map

...OK, that's like a year's worth of work...

# Devlog and Screenshots

Newest entries at the top :)


## GPU gardening

Here's the shadergraph for the grass shader - in case it's useful to anyone.
![Screenshot](./Docs/Images/screenshot34.png)

So happy with the way grass has turned out - and still with a respectable frame rate, even on my Macbook M1.
![Screenshot](./Docs/Images/screenshot33.png)

![Screenshot](./Docs/Images/screenshot32.png)

![Screenshot](./Docs/Images/screenshot31.png)

Two millon blades of grass - but only 10FPS :)
![Screenshot](./Docs/Images/screenshot30.png)

First working GPU instanced objects (crates).  Source is this video, which is insanely short but the code works well: https://www.youtube.com/watch?v=eyaxqo9JV4w 
![Screenshot](./Docs/Images/screenshot29.png)

New graphics settings.  The depth of field on the camera (IMO) brings so much life to the scene.  Lighting slightly improved.  Held back at this stage by Unity's inability to use baked animation on prefabs... and given that every asset in the game is a prefab... this makes it a blocker.  Just means objcts look
a bit lame with not lit by the "sun" light.
![Screenshot](./Docs/Images/screenshot28.png)


## Very busy weekend

This weekend I had a very bad back, so I was basically stuck in the house, unable to do very much.  As a result though, I made HUGE progress on Smolbean Planet 3D.

Added a variety of menus and toolbars, load and save game functionality, the ability to create a new game, erm, in-game... as well as stones and a new
stonecutter's hut.

Rocks and new buildings.
![Screenshot](./Docs/Images/screenshot27.png)

A Main menu.
![Screenshot](./Docs/Images/screenshot26.png)

Creating an island, with preview, in the game.
![Screenshot](./Docs/Images/screenshot25.png)

Save and load menus, with an automated name generator (foreshadowing random names for colonists, perhaps).
![Screenshot](./Docs/Images/screenshot24.png)

Not-so-low-poly stonecutter hut.  I created the cool stone brick style of the stonecutters hut by staring blankly at this YouTube video for hours: https://www.youtube.com/watch?v=tX3JZF53e24&t=2s
![Screenshot](./Docs/Images/screenshot23.png)

## Making a game

So I suddenly realised that if this is going to be a real game, I'm going to need to be able to:
1. Generate new maps at runtime
2. Save and load games
3. Generate a map with WFC very quickly
4. Add a menu system to actually do the above

This has led me onto a refactoring spree of epic proportions - with little visible change to the game!  So far, I have the WFC algorithm working 20-30 times quicker (from 40s to < 2s>) and have
separated the mesh generation, matching and data prep out into editor functions.  Hopefully this will allow me to do some saving and loading more easily - by just saving the game map and re-skinning by repeating the WFC algo with a known random seed.  No need to store mesh data, I hope...

Lots of progress on creating a god-game out of the WFC map.  I think in my head it's currently looking a lot like the original Settlers game from back in the day.  So far in reality, there's just a woodcutter's hut and a strange blue capsule who cuts down trees.  Check out the video (click through below)...

[![Video](./Docs/Images/screenshot22.png)](https://dantelore.com/assorted/Smolbean3D-1.m4v)

Forest Generation, using some tree prefabs I made myself, perlin noise and some random rotation, scale etc.
![Screenshot](./Docs/Images/screenshot21.png)

Added some trees and a basic woodcutter's hut.
![Screenshot](./Docs/Images/screenshot20.png)

## More tweaks to wave function collapse

It's working!  Using the Perlin noise "game map" as "bones" for an island, wave function collapse then skins it, using available tiles!
![Screenshot](./Docs/Images/screenshot19.png)

Created a quick script to create islands with Perlin noise, which will form the "game map" underneath the mesh.  Wave function collapse will be used to "skin" this map.
![Screenshot](./Docs/Images/islands.gif)

Getting ready to add an offset game grid - looking at grid duality stuff which Oskar Stålberg uses in Townscaper etc. See here: https://www.youtube.com/watch?v=Uxeo9c-PX-w
![Screenshot](./Docs/Images/screenshot17.png)

Maps looking stringier than ever
![Screenshot](./Docs/Images/screenshot18.png)

Funky tunnels - slowly getting better at Blender...
![Screenshot](./Docs/Images/screenshot16.png)

Tweaked the meshes for cliff-slope transitons (applied transforms, sorted normals) allowing them to be used again.  Some sussy matches between meshes to be looked at...
![Screenshot](./Docs/Images/screenshot15.png)

## Ocean

A welcome return to using shadergraph, which I really enjoy!

Beaches!!
![Screenshot](./Docs/Images/screenshot14.png)

Waves!!!!!
![Screenshot](./Docs/Images/waves.gif)

Added a very flat ocean, following a wonderful tutorial from here: https://alexanderameye.github.io/notes/stylized-water-shader/
![Screenshot](./Docs/Images/screenshot13.png)

![Screenshot](./Docs/Images/screenshot12.png)

![Screenshot](./Docs/Images/screenshot11.png)

## Tweaking Wave function collapse to try to build islands

I think I'm missing something here.  I can't seem to get the tile priorities set up in a way that prevents very complex coastlines.  Really I want round islands which pile upwards towards the middle.

Some hard coded tile priorities, to try to make the algorithm favour flat areas and avoid sea-like options.  Not exactly mind blowing results...
![Screenshot](./Docs/Images/screenshot10.png)

Variable island size, by setting a radius.
![Screenshot](./Docs/Images/screenshot9.png)

A nice top down view of the sea around the "island".
![Screenshot](./Docs/Images/screenshot8.png)

Added sea round the edge of the island!
![Screenshot](./Docs/Images/screenshot7.png)

Added some irregularity to make the terrain look a little less blocky - but keeping a low-poly feel for now
![Screenshot](./Docs/Images/screenshot6.png)

Fixed normals on rotated meshes using a snazzy gizmo I copy pasta'd.
![Screenshot](./Docs/Images/screenshot5.png)

## Got basic wave function collapse working

Unlike with the 2D version of this project, where I typed in the tile neighbour relationships by hand, here I load all the meshes from a single FBX file, then use a fuzzy edge matching algorithm to decide whether they are compatible as neighbours or not.  

Hacked in a very basic weighting towards flat surfaces, which gives more open areas.
![Screenshot](./Docs/Images/screenshot4.png)

Added ramps as well as cliffs.  If I'd been manually typing tile relationships, this would have taken forever - but I'm not, so it was shockingly simple!
![Screenshot](./Docs/Images/screenshot3.png)

First working wave function collapse!
![Screenshot](./Docs/Images/screenshot2.png)

Partially working algorithm which seems to create ripples.  Not working but cool :)
![Screenshot](./Docs/Images/screenshot1.png)

# References and Notes:

Here is some nice music...
https://soundimage.org/naturescience-2/

And some free sound effects
https://freesound.org/people/BurghRecords/sounds/490845/

From the undisputed master of wave function collapse:
https://www.youtube.com/watch?v=Uxeo9c-PX-w

Amazing tutorial about creating assets
https://www.youtube.com/watch?v=KFEb51rinwI&list=PLQk3p-aJsSWTKKmuFwnzEVaf3ovKIg1jx

Strategy game camera tutorial:
https://www.youtube.com/watch?v=3Y7TFN_DsoI

Download a pallete
https://lospec.com/palette-list

Use a simple colour palette in Blender
https://www.youtube.com/watch?v=8NEmx0cHwoI

Making a cliff in blender:

Adding some texture at the front:
* Add some vertical loop cuts
* Subdivide the front face
* Open the subdivide transient menu thing at the bottom left
* Along Normal = 1.0
* Play with fractal value to add noise

Aligning edges - e.g. when making a one-sided cliff, do this for left, right and bottom edges, which need to align to other tile blocks
* Select a point on the back face
* shift + s, then "Cursor to selected"
* '.' then "3d Cursor" to transform wrt 3d cursor
* 's' to scale, then 'x', 'y', or 'z' for appropriate axis
* '0' to scale to zero along that axis
* Points now nicely aligned

Lining up points with adjacent tile blocks (https://www.youtube.com/watch?v=2v7BgvUuUQU&t=907s)
Target object = the one you're going to keep.  Reference object = the one you cloned as a reference
* Clone the block and align next to itself
* Set the 3d cursor to a known-good point on the back face
* Select both objects with shift-click and go into edit mode, wireframe
* Select the point on your target object first, then the point on the reference object
* Shift s, then "Selection to active"
* Once they're all lined up, select all points along the edge of both objects and align on the axis, as per above
