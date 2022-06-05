[⬅️ back home](intro.html)

# Creating Artwork - Photo Cutouts using your own photos or found photos


### Cropping objects out of images

Example (it’s easiest if your object stands out from the background):

![unity](asset-creation-images/image14.jpg)

- Following same steps as above, double click background layer to turn into a regular layer:

![unity](asset-creation-images/image28.jpg)


### Option 1: Using object selection tool

![unity](asset-creation-images/image32.jpg)


- Drag over the object and see if Photoshop selects what you want it to. In this case it did:

![unity](asset-creation-images/image6.jpg)


### Option 2: Using Quick Selection Tool

![unity](asset-creation-images/image32.jpg)

- Adjust the brush size in this menu bar. Experiment so that brush is similar size, but a bit smaller than the shapes in your object

![unity](asset-creation-images/image35.jpg)

This is a good size to select the main portion of this object:

![unity](asset-creation-images/image8.jpg)

For smaller sections of object, you can make the selection brush smaller.

### Removing from selected area

For example, if you select this sand area:

![unity](asset-creation-images/image31.jpg)

Press the option (or alt if you are on windows) key while selecting the areas you would like to remove from your selection. 

### Now: Once your finished making your selection


Now in the menubar go to Selection -> Inverse to select background. Delete the background to leaves transparency around the object:

![unity](asset-creation-images/image6.jpg)


## Saving as a Transparent PNG

- In the menubar go to Image -> Image Size. Make your image about 1500 pixels in the maximum dimension.
- Go to File -> Export -> Quick Export as PNG. Save the file to your computer in a recognizable location.

## Add the photo to your CGDT Unity project
- Make sure your CGDT project is open
- In the Project panel in your Unity project, navigate to the Photos folder. Drag your PNG to that folder in the Project panel. 
- Drag it into your scene! 

<!---- begin statcounter ---->
<script type="text/javascript">
var sc_project = 12399103;
var sc_invisible = 1;
var sc_security = "dbebcd0c";
</script>
<script type="text/javascript" src="https://www.statcounter.com/counter/counter.js" async></script>
<noscript>
<div class="statcounter">
    <a title="Web Analytics" href="https://statcounter.com/" target="_blank"><img class="statcounter" src="https://c.statcounter.com/12399103/0/dbebcd0c/1/" alt="Web Analytics" /></a>
</div>
</noscript>
<!-- end statcounter -->