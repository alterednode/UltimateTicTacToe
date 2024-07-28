using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is for all the platform specific tweaks required for this game. 60fps for iOS, and whatever else we run into later.
public class platformSpecific : MonoBehaviour
{
	public bool gamepadSupport = false;

    // Start is called before the first frame update
    void Start()
	{
    	
		Debug.Log( Input.GetJoystickNames()[0]);
	    //set framerate to 60. iOS will often default to 30fps if you don't set this value.
	    if (Application.platform == RuntimePlatform.IPhonePlayer)
	    {
		    Application.targetFrameRate = 60;
	    }

		if (gamepadSupport){
			GameObject.Find("Cursor").SetActive(true);
		}else{
			GameObject.Find("Cursor").SetActive(false);
		}
    }
}
