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
	

// Start is called before the first frame update
    void Start()
    {
	    gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	    smallGridHolder = GameObject.Find("Small Grids");
    }


    private void OnMouseDown() {
        clickStartedHere = true;
    }
    private void OnMouseExit() {
        clickStartedHere = false;
    }

    /// <summary>
    /// Called every frame while the mouse is over the GUIElement or Collider.
    /// </summary>
    void OnMouseOver()
    {
	    if(Input.GetMouseButtonUp(0) && clickStartedHere)
	    {
		    UpdateTracking();
        	
		    Debug.Log(gameObject.name + gameObject.transform.parent.parent.name);
	        
		    SpawnXorO();
	        
		    GameObject.Find("WhereToPlay").GetComponent<WhereToPlay>().
			    SetLocationAndScale((smallGridHolder.transform.GetChild(transform.GetSiblingIndex())).transform);
			    
	        
	        
	        
        }
    }

    // Update is called once per frame
    void Update()
    {
	   
    }
	
	void SpawnXorO()
	{
		GameObject newPiece = Instantiate(gameManager.prefabXO[Convert.ToInt32(gameManager.xPlayerTurn)]);
		newPiece.transform.position = gameObject.transform.position;
        newPiece.transform.parent = gameObject.transform.parent.parent;
        
		gameManager.xPlayerTurn = !gameManager.xPlayerTurn;

        gameObject.SetActive(false);
	}
    
	void UpdateTracking()
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
