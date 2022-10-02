using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhereToPlay : MonoBehaviour
{
	public Transform focusOnThis;
	public float largeScale;
	public float lineScale;
	
	public float moveTime;
	
	public const float bigGridLargeScale = 1;
	public const float bigGridLineScale = 2;
	public const float smallGridLargeScale = .34f;
	public const float smallGridLineScale = 8;
	
	
    // Start is called before the first frame update
    void Start()
    {
	    focusOnThis = GameObject.Find("BigGrid").transform; // start focused on the whole area, first player can start whereever
	    SetLocationAndScale(GameObject.Find("BigGrid").transform); // is not focusOnThis because i am worried it would break lol
    }

    // Update is called once per frame
    void Update()
	{
		transform.localScale =  Vector3.Lerp(transform.localScale, new	Vector3(largeScale,largeScale,1), moveTime); // always set scale of the WhereToPlay object to what the large scale is
		transform.position =  Vector3.Lerp(transform.position, focusOnThis.transform.position, moveTime);	// always go to the focus on this position
		foreach (Transform child in transform)
		{
			child.localScale =  Vector3.Lerp(child.localScale	, new	Vector3(child.localScale.x,lineScale,1), moveTime); //adjusts the lines to be the right size
		}
	}
    
	public void	SetLocationAndScale(Transform goHere) //tells the object what it needs to focus on
	{
		if (goHere.name.Contains("Grid ")) // this indicates it is a small grid, because the BigGrid does not have any spaces
		{
			if (goHere.gameObject.GetComponent<GridManager>().thisGridPlayable) // if the grid is playable
			{
				
				largeScale = smallGridLargeScale; 
				lineScale = smallGridLineScale;
				focusOnThis = goHere;
				foreach (Transform child in GameObject.Find("Small Grids").transform)
				{
					child.GetChild(2).gameObject.SetActive(false);
				}
				goHere.GetChild(2).gameObject.SetActive(true);
				
			}else // if the small grid is unplayable, because it is full or won, use big grid settings
			{
				focusOnBigGrid();
			}
			
		}else // the grid is not a small grid so focus on the big one
		{
			focusOnBigGrid();
		}
		
	}
	public void	focusOnBigGrid()
	{
		focusOnThis = GameObject.Find("BigGrid").transform; // focus on big grid
		largeScale = bigGridLargeScale; 
		lineScale = bigGridLineScale;
		foreach (Transform child in GameObject.Find("Small Grids").transform)
		{
			if (child.gameObject.GetComponent<GridManager>().thisGridPlayable) // make every playable child's triggers active
			{
				child.GetChild(2).gameObject.SetActive(true); 
			}else
			{
				child.GetChild(2).gameObject.SetActive(false);
			}
					
		}
	}
	
	
}
