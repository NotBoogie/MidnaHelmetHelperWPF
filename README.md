# MidnaHelmetHelperWPF

The app auto loads the content in the /Models folder. If you draw something else that isn't Midna a lot, you can probably just swap out those files for yours

Control definitions:

-LOCK/UNLOCK: Locks the popup window to allow you to draw through it

-SHOW/HIDE: Displays the popup window

-PASTE/CLEAR BG: Takes the image data off of your clipboard and makes it the background of the popup. Useful if you want to just screenshot/snip your sketch's head and position the model as a posed reference

-BASE/CUSTOM (Model): Toggle between the fused shadow and a custom .obj file of your choosing

-BACK/NOBACK: will show/hide the backs of polygons in the viewer. This is so that when you have the model posed over Midna's face, the back part isn't blocking your sketch

-BASE/LINE/CUSTOM (Texture): Toggle between the base fused shadow texture, a quick line guide version I made, and your own file

-SAVE SCREENSHOT: Saves a screenshot of the popup window, including any custom pasted background

-HELP PLS: Shows a quick rundown of what the app does in a popup window


Basic use:
Position the popup window over your sketch, tweak the model as desired (right click to rotate, mousewheel to zoom in/out, mousewheel click to move the model - note that this affects the actual position of the model in 3d space, so it will mess up your right-click rotation point). The sliders in the main window can be used to get those exact tweaks and each one is a 360 degree turn.

TODO: Applying custom textures will always align to the upper left coordinates, haven't figured out how to get around this yet
TODO: Figure out a non-shit way to control model position for people with smaller screens
