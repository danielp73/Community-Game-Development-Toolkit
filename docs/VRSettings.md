#VR Project Setup: Settings and Plugins

1. After creating a 3D project from the 3D Core template Open the Project settings
2. Edit ➡️ Project Settings

   ![](images/VRSettings/1.png)

3. Once the Project settings are open go to, XR plugin management ➡️ Install XR Plugin Management, and then click Install XR Plugin Component

   ![](images/VRSettings/2.png)

4. Then once installed in the same location, XR plugin management ➡️ Install XR Plugin Management, select the Occulus option for both Desktop and for Android

   ![](images/VRSettings/3.png)
   ![](images/VRSettings/4.png)

5. Next go to, File ➡️ Build Settings

   ![](images/VRSettings/5.png)

6. Select the Andriod option and click Switch Platform

   ![](images/VRSettings/6.png)

7. Open Player Settings

   ![](images/VRSettings/7.png)

8. Set Player Minimum API level to Android 6.0 Marshmallow (API level 23) in, Other Settings ➡️ Minimum API Level

   ![](images/VRSettings/8.png)

9. Open the Package Manager window in, Window ➡️ Package Manager

   ![](images/VRSettings/9.png)

10. Open Advaced Project Settings by clicking the ⚙️ in the top right corner
   ![](images/VRSettings/10.png)

11. Enable Pre-release Packages by clicking the check box, click "I Understand" for the poppup
  ![](images/VRSettings/11.png)

12. Go back to the Package Manager, Window ➡️ Package Manager,
13. Make sure that you are filtering by Unity Registry by using the drop down menu in the Top Left of the window and search for XR in the top right search bar
14. click on XR Interaction Toolkit

    ![](images/VRSettings/12.png)

15. Click Install in the bottom right corner
16. Select Yes for the popup warning

    ![](images/VRSettings/13.png)

17. The project will restart
18. Once restarted go to, Window ➡️ Package Manager ➡️ XR Interaction Toolkit, and import the Starter Assets

    ![](images/VRSettings/14.png)

19. Next go to your Project window and find the imported starter assets at, Assets ➡️ Samples ➡️ XR Interaction Toolkit ➡️ 2.2 ➡️ Starter Assets,

    ![](images/VRSettings/15.png)

20. Select each of the XRI Default icons, there should be 8 or more of them

    ![](images/VRSettings/16.png)

21. and in the inspection pannel at the top press Add to ActionBasedContinuousMoveProvider

    ![](images/VRSettings/17.png)

22. Go to, Edit ➡️ Project Settings ➡️ Preset manager

    ![](images/VRSettings/18.png)

24. For all the left and right controller (Can be seen in the right collum ex: XRI Default Right Controller) for the right ones type "Right" in the left collum and for the left ones type "Left" in the same left collum in the same row

    ![](images/VRSettings/19.png)

26. You made it!
27. Next step is to add the Community Game Development Toolkit [Community Game Development Toolkit](ImportToolkit.md)




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
