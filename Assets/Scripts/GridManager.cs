using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
	public bool thisGridPlayable = true;
	public int[] scoreTracking = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
	public GameObject triggers;
	GameManager gameManager;
	BigGridManager bigGridManager;

	// Start is called before the first frame update
	void Start()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		bigGridManager = GameObject.Find("BigGrid").GetComponent<BigGridManager>();
	}

	public void SetScoreOfThisThing(int where, int whatScore) // update the scoreTracking
	{
		scoreTracking[where] = whatScore;
		thisGridPlayable = GameManager.CheckPlayable(scoreTracking);
		if (GameManager.CheckForWin(scoreTracking) != 0) // check for a win
		{
			ThisGameWon(GameManager.CheckForWin(scoreTracking)); //inform big grid that this game was won
			thisGridPlayable = false; // no more playing here
		}

		bigGridManager.checkForTie();

	}

	public void ThisGameWon(int winningInt)
	{
		GameObject WinningSymbol; // winning symbol will be the thing that shows up to show the player if X or O won that small game
		triggers.SetActive(false); // no more playing here
		if (winningInt == 3)
		{
			WinningSymbol = Instantiate(gameManager.prefabXO[1]);

			bigGridManager.SetScoreOfThisThing(transform.GetSiblingIndex(), 1);
		}
		else
		{
			WinningSymbol = Instantiate(gameManager.prefabXO[0]);

			bigGridManager.SetScoreOfThisThing(transform.GetSiblingIndex(), -1);
		}
		WinningSymbol.transform.localScale *= 4; // make the thing bigger
		WinningSymbol.transform.position = transform.position; //reposition it to the right spot
	}





}
