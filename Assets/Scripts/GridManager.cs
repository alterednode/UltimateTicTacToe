using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
	public bool thisGridPlayable = true;
	public int[] scoreTracking = new int[9] {0,0,0,0,0,0,0,0,0};
	public GameObject triggers;
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
		
		
		scoreTracking[where] = whatScore;
		thisGridPlayable=GameManager.CheckPlayable(scoreTracking);
		if (GameManager.CheckForWin(scoreTracking)!=0)
		{
			ThisGameWon(GameManager.CheckForWin(scoreTracking));
			thisGridPlayable=false;
		}
		
		
	}
	
	
	
	
	
	public void ThisGameWon(int	winningInt)
	{
		GameObject WinningSymbol;
		triggers.SetActive(false);
		if (winningInt == 3)
		{
			WinningSymbol= Instantiate(gameManager.prefabXO[1]);
			
			GameObject.Find("BigGrid").GetComponent<BigGridManager>().SetScoreOfThisThing(transform.GetSiblingIndex(),1);
		}else
		{
			WinningSymbol= Instantiate(gameManager.prefabXO[0]);
			
			GameObject.Find("BigGrid").GetComponent<BigGridManager>().SetScoreOfThisThing(transform.GetSiblingIndex(),-1);
		}
		WinningSymbol.transform.localScale *=4;
		WinningSymbol.transform.position = transform.position;
	}
	
    
    
}
