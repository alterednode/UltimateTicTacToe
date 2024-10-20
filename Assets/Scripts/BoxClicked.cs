using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using System.Text.RegularExpressions;

public class BoxClicked : MonoBehaviour
{
    GameManager gameManager;
    bool clickStartedHere = false;
    GameObject smallGridHolder;

    private VirtualMouse controller1;
    private VirtualMouse controller2;

    ControllerManager controllerManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        smallGridHolder = GameObject.Find("Small Grids");

        controllerManager = GameObject.Find("ControllerManager").GetComponent<ControllerManager>();

    }

    /// <summary>
    /// This shit makes it so that the player can only play on a trigger where
    /// they both start and end their click there without leaving the trigger
    /// </summary>

    private void OnMouseDown()
    {
        clickStartedHere = true;
    }

    private void OnMouseExit()
    {
        clickStartedHere = false;
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0) && clickStartedHere)
        {
            MakeMove();
        }
    }

    void VOnMouseOver()
    {
        bool xturn = gameManager.xPlayerTurn;

        bool fireController1 = Input.GetButtonDown("Fire1");
        bool fireController2 = Input.GetButtonDown("Fire1Alt");

	    //Debug.Log("got to the box");

        bool canWeContinueTheGame = false;

        // this is bad code that can be improved, but it functions for now

        if (controllerManager.controller1.inUse && controllerManager.controller2.inUse)
        {
	        //Debug.Log("two controllers detected");
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
	        //Debug.Log("Only one controller detected");
            if (fireController1 || fireController2)
            {
                canWeContinueTheGame = true;
            }
        }

        if (canWeContinueTheGame)
        {
            MakeMove();
        }
    }

    void MakeMove()
    {
        if (!gameManager.canHumanPlayerPlay)
        {
            gameManager.audioManager.PlayClip("ClickFail");
            //TODO: add a sound effect here for if the player tries to play when they are not allowed to. check if this works once online is implemented
            return;
        }

        gameManager.audioManager.PlayClip("ClickSucceed");



        UpdateScoreTracking();

        bool playingAnX = gameManager.xPlayerTurn;

        SpawnXorO(gameManager.xPlayerTurn);

        //update who's turn it is.
        gameManager.xPlayerTurn = !gameManager.xPlayerTurn;

        MoveWhereToPlay();

        if (gameManager.multiplayerEnabled)
        {

            gameManager.canHumanPlayerPlay = false;
            GameObject.Find("OnlineManager").GetComponent<OnlineManager>().humanPlayerPlayedAt(this.transform, playingAnX);
            
        }

    }

    public void MoveWhereToPlay()
    {
        GameObject
           .Find("WhereToPlay")
           .GetComponent<WhereToPlay>()
           .SetLocationAndScale(
               (smallGridHolder.transform.GetChild(transform.GetSiblingIndex())).transform
           );
    }



    public void SpawnXorO(bool xPlayerTurn) // Instantiates a new O or X 
    {
        GameObject newPiece = Instantiate(
            gameManager.prefabXO[Convert.ToInt32(xPlayerTurn)]
        );
        newPiece.transform.position = gameObject.transform.position;
        newPiece.transform.parent = gameObject.transform.parent.parent;

        gameObject.SetActive(false);
    }

    public void UpdateScoreTracking() // tells the Grid Manager which thing was clicked
    {
        int intOfThis = transform.GetSiblingIndex();

        if (gameManager.xPlayerTurn)
        {
            transform
                .parent.parent.gameObject.GetComponent<GridManager>()
                .SetScoreOfThisThing(intOfThis, 1);
        }
        else
        {
            transform
                .parent.parent.gameObject.GetComponent<GridManager>()
                .SetScoreOfThisThing(intOfThis, -1);
        }
    }
}
