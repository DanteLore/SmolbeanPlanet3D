

# Devlog and Screenshots

Newest entries at the top :)

## Ocean

A welcome return to using shadergraph, which I really enjoy!

Waves!!
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