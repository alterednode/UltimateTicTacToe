using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
//using System.Text.RegularExpressions;

public class BoxClicked : MonoBehaviour
{
	GameManager gameManager;
	GameObject smallGridHolder;
	

// Start is called before the first frame update
    void Start()
    {
	    gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	    smallGridHolder = GameObject.Find("Small Grids");
    }


	/// <summary>
	/// This shit makes it so that the player can only play on a trigger where 
	/// they both start and end their click there without leaving the trigger
	/// </summary>


    void OnMouseOver()
    {
	    if(Input.GetMouseButtonDown(0))
	    {
		    UpdateTracking();
        	
		    //I don't think we still need to log this, do we?
		    //Debug.Log(gameObject.name + gameObject.transform.parent.parent.name);
	        
		    SpawnXorO();
	        
		    GameObject.Find("WhereToPlay").GetComponent<WhereToPlay>().
			    SetLocationAndScale((smallGridHolder.transform.GetChild(transform.GetSiblingIndex())).transform);
        }
    }

	void VOnMouseOver()
    {
	    if(Input.GetButtonDown("Fire1"))
	    {
		    UpdateTracking();
        	
		    //I don't think we still need to log this, do we?
		    //Debug.Log(gameObject.name + gameObject.transform.parent.parent.name);
	        
		    SpawnXorO();
	        
		    GameObject.Find("WhereToPlay").GetComponent<WhereToPlay>().
			    SetLocationAndScale((smallGridHolder.transform.GetChild(transform.GetSiblingIndex())).transform);
        }
    }

	void SpawnXorO() // Instantiates a new O or X based on which player's turn it is and changes who's turn it is
	{
		GameObject newPiece = Instantiate(gameManager.prefabXO[Convert.ToInt32(gameManager.xPlayerTurn)]);
		newPiece.transform.position = gameObject.transform.position;
        newPiece.transform.parent = gameObject.transform.parent.parent;
        
		gameManager.xPlayerTurn = !gameManager.xPlayerTurn;

        gameObject.SetActive(false);
	}
    
	void UpdateTracking() // tells the Grid Manager which thing was clicked
	{
		int intOfThis = transform.GetSiblingIndex();
		
		
		
		if (gameManager.xPlayerTurn)
		{
			transform.parent.parent.gameObject.GetComponent<GridManager>().
				SetScoreOfThisThing(intOfThis,1);
		}else
		{
			transform.parent.parent.gameObject.GetComponent<GridManager>().
				SetScoreOfThisThing(intOfThis,-1);
		}
		
	}
    
    
}
