using UnityEngine;
using UnityEngine.UI;

public class VirtualButton : MonoBehaviour
{
    public Button buttonScript;
    GameManager gameManager;
    ControllerManager controllerManager;
    AudioManager audioManager;
    BoxCollider collider;

    public bool canAlwaysBeClicked = false;

    void Start()
    {
        controllerManager = GameObject.Find("ControllerManager").GetComponent<ControllerManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();


        if (!canAlwaysBeClicked)
        {
            //try catch is likely unnecessary, but it's here just in case finding the gameManager fails
            try
            {
                gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            }
            catch
            {
                Debug.Log("GameManager not found");
            }
        }



        buttonScript = GetComponent<Button>();
    }

    void VOnMouseOver()
    {

        // if gameManager, figure out who clicked and take action based on that.
        if (gameManager != null)
        {

            // code pulled from BoxClicked.cs

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
                audioManager.PlayClip("ClickSucceed");
                buttonScript.onClick.Invoke();
            }

        }
        //if no game manager, we don't care who clicked
        else
        {

            bool fireController1 = Input.GetButtonDown("Fire1");
            bool fireController2 = Input.GetButtonDown("Fire1Alt");

            if (fireController1 || fireController2)
            {
                //TODO: this sound gets cut off when loading scenes, add some delay on interaction? probably within the sceneLoader script
                audioManager.PlayClip("ClickSucceed");
                buttonScript.onClick.Invoke();
            }
        }
    }
}
