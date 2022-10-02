using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhereToPlay : MonoBehaviour
{
	public Transform focusOnThis;
	public float largeScale;
	public float lineScale;
	
	public float moveTime;
	
    // Start is called before the first frame update
    void Start()
    {
	    focusOnThis = GameObject.Find("BigGrid").transform;
	    SetLocationAndScale(GameObject.Find("BigGrid").transform);
    }

    // Update is called once per frame
    void Update()
	{
		transform.localScale =  Vector3.Lerp(transform.localScale, new	Vector3(largeScale,largeScale,1), moveTime);
		transform.position =  Vector3.Lerp(transform.position, focusOnThis.transform.position, moveTime);
		foreach (Transform child in transform)
		{
			child.localScale =  Vector3.Lerp(child.localScale	, new	Vector3(child.localScale.x,lineScale,1), moveTime);
		}
	}
    
	public void	SetLocationAndScale(Transform goHere)
	{
		if (goHere.name.Contains("Grid "))
		{
			if (goHere.gameObject.GetComponent<GridManager>().thisGridPlayable)
			{
				largeScale = .34f;
				lineScale = 8;
				focusOnThis = goHere;
				foreach (Transform child in GameObject.Find("Small Grids").transform)
				{
					child.GetChild(2).gameObject.SetActive(false);
				}
				goHere.GetChild(2).gameObject.SetActive(true);
				
			}else
			{
				focusOnThis = goHere.parent.parent;
				largeScale = 1;
				lineScale = 2;
				foreach (Transform child in GameObject.Find("Small Grids").transform)
				{
					if (child.gameObject.GetComponent<GridManager>().thisGridPlayable)
					{
						child.GetChild(2).gameObject.SetActive(true);
					}else
					{
						child.GetChild(2).gameObject.SetActive(false);
					}
					
				}
			}
			
		}else
		{
			largeScale = 1;
			lineScale = 2;
			foreach (Transform child in GameObject.Find("Small Grids").transform)
			{
				if (child.gameObject.GetComponent<GridManager>().thisGridPlayable)
				{
					child.GetChild(2).gameObject.SetActive(true);
				}else
				{
					child.GetChild(2).gameObject.SetActive(false);
				}
					
			}
		}
		
	}
}
