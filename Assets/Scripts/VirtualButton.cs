using UnityEngine;
using UnityEngine.UI;

public class VirtualButton : MonoBehaviour
{
    public Button buttonScript;
    GameManager gameManager;
    private VirtualMouse controller1;
    private VirtualMouse controller2;

    public bool canAlwaysBeClicked = false;

    void Start()
    {
        try
        {

            if (!canAlwaysBeClicked)
            {
                gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            }
            //TODO: make controllers work on main menu
            try
            {
                controller1 = GameObject.Find("Controller Cursor 1").GetComponent<VirtualMouse>();
                controller2 = GameObject.Find("Controller Cursor 2").GetComponent<VirtualMouse>();

            }
            catch (System.Exception)
            {
                Debug.LogWarning("remember to make controllers work on main menu");
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

            if (controller1.inUse && controller2.inUse)
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
            if (controller1.inUse != controller2.inUse)
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
            buttonScript.onClick.Invoke();
        }
    }
}
