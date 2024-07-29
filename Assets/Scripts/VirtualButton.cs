using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VirtualButton : MonoBehaviour
{
    public Button buttonScript;

    void Start(){
        buttonScript = GetComponent<Button>();
    }

	void VOnMouseOver()
	{
    	
    	
		if (Input.GetButtonUp("Fire1") || Input.GetButtonUp("Fire1Alt"))
        {
            buttonScript.onClick.Invoke();
        }
    }
}
