using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//using System.Text.RegularExpressions;

public class BoxClicked : MonoBehaviour
{
    GameManager gameManager;
    bool clickStartedHere = false;
	GameObject smallGridHolder;
    
	private VirtualMouse controller1;
	private VirtualMouse controller2;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	    smallGridHolder = GameObject.Find("Small Grids");
	    
	    controller1 = GameObject.Find("Controller Cursor 1").GetComponent<VirtualMouse>();
	    controller2 = GameObject.Find("Controller Cursor 2").GetComponent<VirtualMouse>();
    
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
		
		
		Debug.Log("got to the box");
		
		bool canWeContinueTheGame = false;	
		
		// this is bad code that can be improved, but it functions for now
		
		if(controller1.inUse&&controller2.inUse)
		{
			Debug.Log("two controllers detected");
			if(xturn&&fireController1)
			{
				canWeContinueTheGame=true;
			}
			if((!xturn)&&fireController2)
			{
				canWeContinueTheGame=true;
			}
		}
		if(controller1.inUse!=controller2.inUse)
		{
			Debug.Log("Only one controller detected");
			if(fireController1||fireController2)
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
        UpdateTracking();

        //I don't think we still need to log this, do we?
        //Debug.Log(gameObject.name + gameObject.transform.parent.parent.name);

        SpawnXorO();

        //update who's turn it is.
        gameManager.xPlayerTurn = !gameManager.xPlayerTurn;

        GameObject
            .Find("WhereToPlay")
            .GetComponent<WhereToPlay>()
            .SetLocationAndScale(
                (smallGridHolder.transform.GetChild(transform.GetSiblingIndex())).transform
            );
    }

    void SpawnXorO() // Instantiates a new O or X based on which player's turn it is and changes who's turn it is
    {
        GameObject newPiece = Instantiate(
            gameManager.prefabXO[Convert.ToInt32(gameManager.xPlayerTurn)]
        );
        newPiece.transform.position = gameObject.transform.position;
        newPiece.transform.parent = gameObject.transform.parent.parent;


        gameObject.SetActive(false);
    }

    void UpdateTracking() // tells the Grid Manager which thing was clicked
    {
        int intOfThis = transform.GetSiblingIndex();

        if (gameManager.xPlayerTurn)
        {
            transform.parent.parent.gameObject
                .GetComponent<GridManager>()
                .SetScoreOfThisThing(intOfThis, 1);
        }
        else
        {
            transform.parent.parent.gameObject
                .GetComponent<GridManager>()
                .SetScoreOfThisThing(intOfThis, -1);
        }
    }
}
