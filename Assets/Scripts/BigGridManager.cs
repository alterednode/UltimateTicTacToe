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

    // Update is called once per frame
    void Update()
    {
        
    }
	public void SetScoreOfThisThing(int	where, int whatScore)
	{
		GameObject WinningSymbol;
		isGameNotWon=GameManager.CheckPlayable(scoreTracking);
		scoreTracking[where] = whatScore;
		if (GameManager.CheckForWin(scoreTracking)!=0)
		{
			Debug.Log(GameManager.CheckForWin(scoreTracking) + "WON!!!!!!!     !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
			
			if (GameManager.CheckForWin(scoreTracking) == -3)
			{
				WinningSymbol= Instantiate(gameManager.prefabXO[0]);
			}else
			{
				WinningSymbol= Instantiate(gameManager.prefabXO[1]);
			}
			WinningSymbol.transform.localScale *=16;
			WinningSymbol.transform.position = transform.position;
			foreach (Transform child in GameObject.Find("Small Grids").transform)
			{
				child.GetChild(2).gameObject.SetActive(false);
			}
			gameManager.restartButton.SetActive(true);
			
		}
		
		
	}
    
	/*
	
	Dont ask about the fucking formatting
	
	
	*/
}
