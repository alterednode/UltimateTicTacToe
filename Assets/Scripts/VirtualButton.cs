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
        if (Input.GetButtonDown("Fire1"))
        {
            buttonScript.onClick.Invoke();
        }
    }
}
