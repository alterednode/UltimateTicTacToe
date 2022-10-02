using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigGridManager : MonoBehaviour
{
	public bool isGameNotWon;
	public int[] scoreTracking = new int[9] {0,0,0,0,0,0,0,0,0};
	GameManager gameManager;
	
	
    // Start is called before the first frame update
    void Start()
    {
	    gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

	public void SetScoreOfThisThing(int	where, int whatScore)
	{
		GameObject WinningSymbol;
		scoreTracking[where] = whatScore;//update scoretracking 
		
		isGameNotWon=GameManager.CheckPlayable(scoreTracking); // detect if the game is won
		
		if (GameManager.CheckForWin(scoreTracking)!=0) //detect if a player has won
		{
			//Debug.Log(GameManager.CheckForWin(scoreTracking) + "WON!!!!!!!     !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
			
			if (GameManager.CheckForWin(scoreTracking) == -3) //if O won
			{
				WinningSymbol= Instantiate(gameManager.prefabXO[0]);
			}else// if X won
			{
				WinningSymbol= Instantiate(gameManager.prefabXO[1]);
			}
			WinningSymbol.transform.localScale *=16; // scale up to the size of the big grid
			WinningSymbol.transform.position = transform.position + new Vector3(0,0,-8.5f); // center the thing and move in front of everything
			foreach (Transform child in GameObject.Find("Small Grids").transform) // player canot play anymore
			{
				child.GetChild(2).gameObject.SetActive(false);
			}
			gameManager.restartButton.SetActive(true); // show reset button
			
		}
		
		
	}
    
	/*
	
	Dont ask about the fucking formatting
	
	
	*/
}
