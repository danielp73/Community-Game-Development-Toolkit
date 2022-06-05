[⬅️ back home](intro.html)

# Creating drawings and sketches for use in your project




## Start by making a drawing

### Crayons are fun!

Make a drawing and take a photo of it. Try to take photo with even background light. Better yet, scan your drawing. Here is an example:

![unity](asset-creation-images/image7.jpg)



### Open in Photoshop

- Open your image in Photoshop
- Crop using the Crop Tool:

![unity](asset-creation-images/image12.jpg)


- Double click Background Layer to turn it into a regular Layer.


Before: 

![unity](asset-creation-images/image30.jpg)

 After:
 

![unity](asset-creation-images/image5.jpg)



### Adjusting your image in Photoshop to make it easier to make a selection

Open your image, and from the menu bar choose Image -> Adjustments -> Levels. Fool around with these markers to make your drawing stand out from the background:


![unity](asset-creation-images/image22.jpg)




Usually it helps to move the right most marker inwards to make bright parts brighter, and the middle marker to the left to increase contrast:


![unity](asset-creation-images/image33.jpg)





### Make a selection using the Color Range Tool

- From menu bar choose Select -> Color Range. 
- Check ‘Invert’
- Use the eye dropper tool to click in your drawing. 
- The black areas in the previews are your selection
- Try clicking on different locations to see what creates the best result. Also fool around with the Fuzziness slider.
- When done, press OK. (now, the background will be selected--you’ll see why in a minute)


![unity](asset-creation-images/image24.jpg)



Now you should have something that looks like this, showing your selection:


![unity](asset-creation-images/image23.jpg)




Press delete to delete the selected background. You should see checkerboard background, indicating transparent areas:


![unity](asset-creation-images/image26.jpg)



### Saving as a Transparent PNG

- In the menu bar go to Image -> Image Size. Make your image about 1500 pixels in the maximum dimension.
- Go to File -> Export -> Quick Export as PNG. Make sure to give your file a recognizable name, and take note of where you save it on your computer.

## If you have a multicolor drawing

Example:


![unity](asset-creation-images/image25.jpg)



- In the selection step with the Color Range tool, don’t click the Invert check box. 
- Then click on the image’s background to select the background:



![unity](asset-creation-images/image27.jpg)




- Adjust where you click and the fuzziness slider. 
- When finished, press OK, and then delete background. Complete rest of steps using the same process.



![unity](asset-creation-images/image13.jpg)



## Saving as a Transparent PNG

- In the menubar go to Image -> Image Size. Make your image about 1500 pixels in the maximum dimension.
- Go to File -> Export -> Quick Export as PNG.
- Find your Unity project folder, and navigate to Assets->Drawings inside that folder. Save your PNG at that location.

## Add the photo to your CGDT Unity project
- Make sure your CGDT project is open
- In the Project panel in your Unity project, navigate to the Drawings folder. Drag your PNG to that folder in the Project panel. 
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