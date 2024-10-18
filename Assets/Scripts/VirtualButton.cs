using UnityEngine;
using UnityEngine.UI;

public class VirtualButton : MonoBehaviour
{
    public Button buttonScript;
    GameManager gameManager;
    ControllerManager controllerManager;

    public bool canAlwaysBeClicked = false;

    void Start()
    {
        controllerManager = GameObject.Find("ControllerManager").GetComponent<ControllerManager>();
        try
        {

            if (!canAlwaysBeClicked)
            {
                gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            }
        }
        catch
        {
            Debug.Log("GameManager not found");
        }


        buttonScript = GetComponent<Button>();
    }

    void VOnMouseOver()
    {
        // code pulled from BoxClicked.cs

        if (gameManager != null)
        {


            bool xturn = gameManager.xPlayerTurn;

            bool fireController1 = Input.GetButtonDown("Fire1");
            bool fireController2 = Input.GetButtonDown("Fire1Alt");

            bool canWeContinueTheGame = false;

            if (controllerManager.controller1.inUse && controllerManager.controller2.inUse)
            {
                if (xturn && fireController1)
                {
                    canWeContinueTheGame = true;
                }
                if ((!xturn) && fireController2)
                {
                    canWeContinueTheGame = true;
                }
            }
            if (controllerManager.controller1.inUse != controllerManager.controller2.inUse)
            {
                if (fireController1 || fireController2)
                {
                    canWeContinueTheGame = true;
                }
            }

            if (canWeContinueTheGame)
            {
                buttonScript.onClick.Invoke();
            }

        }
        else
        {

            bool fireController1 = Input.GetButtonDown("Fire1");
            bool fireController2 = Input.GetButtonDown("Fire1Alt");

            if (fireController1 || fireController2)
            {
                buttonScript.onClick.Invoke();
            }
        }
    }
}
