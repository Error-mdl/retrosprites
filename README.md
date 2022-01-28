#Retrosprites

This shader imitates the Psuedo-3d directional sprites used in many early to mid 90's first person games like Doom, Daggerfall, Marathon, Hexen, etc. Rather than use 3d models for enemies, these games used 2d billboards which would change their image based on the relative postions of the viewer and the enemy. This shader was built for VRChat, and as such was made for the built-in render pipeline with single-pass stereo VR support in mind and some game specific optimizations like changing billboarding behavior in mirrors.

# Usage

## Creating Sprite Sheets

If your sprite is animated you will first need to assemble sprite sheets from individual sprites. Sprites for each direction go in separate sheets. The shader reads frames from the sheet going left to right through each row and reads rows top to bottom, just like text. Unlike the previous version of this shader, you may leave empty spaces at the end of the sheet. Every sheet must have exactly the same dimensions and number of sprites!

## Creating Texture Arrays
This shader does not use normal textures. It instead uses an object called a Texture2DArray, which is essentially just a bunch of textures with identical dimensions and properties bundled together in a single object. Using a Texture2DArray massively simplifies and increases the efficiency of this shader, but requires the user to go through a couple of extra steps to create an array from their source textures. Unity versions below 2021 require external scripts to generate these. A good tool to use is [Pschraut's Texture2DArray Pipeline](https://github.com/pschraut/UnityTexture2DArrayImportPipeline) or you can use the simple script included with this project. In order to create an array, all textures must have identical dimensions and import settings. Textures in the array can either be individual sprites or sprite sheets for each viewing angle. For individual sprites, sprites in the array need to be arranged first in groups of viewing angles for an individual animation frame, and then by order of their animation frame. Viewing angles should be sorted starting from the front and going clockwise or counter clockwise.

If you are using the included script, right click in your project view and click Create>Texture Lists>2D Texture list. Select the texture list object, and lock the inspector (lock icon in the upper right corner). Select all the sprites and drag them on to the text "Tex Array" in the inspector. All textures should be then assigned to the list object. Now go to tools/Create Texture Array. Drop the texture list you just made into the corresponding box and click "Create Array". Assuming all your sprites have the same dimensions and same properties, you'll be prompted to save the texture array.

## Applying the shader
You should apply a material using the shader to a Z-facing quad, either
sprite.fbx included with this shader or unity's quad primitive (which faces
-z so the texture will be mirrored). The parameters for the shader are mostly
self-explanatory.

In order to get the shader to work properly with particle systems, you must
change several settings in the render tab. See the particle system prefab for an example.
Render Mode should be billboard (default). Render alignment should be world. Custom vertex streams should be
enabled with these streams, in order: Position, Color, UV, Center, and Velocity.
Light Probes should be set to blend probes if lighting is enabled.


#Credits

Example sprites from the [Freedoom](https://github.com/freedoom/freedoom) project, and are under the license (COPYING.adoc) found in the example assets texture folder.