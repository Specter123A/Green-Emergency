using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtility    
{
  //this class will not derive from anything, but will simply store data
  public const float ResolutionDelayTime =1;
  // the ResolutionDelayTime refers to how long the screen will be displayed. 
  //For example, the correct answer screen is a resolution screen here, and that will be displayed for 1 second 

  //to save highscore, playerprefs will be used

  public const string SavePrefKey = " Game HighScore Value";    //this will be the highscore value key that we can use 
  //it can be accessed from other scripts 



}
