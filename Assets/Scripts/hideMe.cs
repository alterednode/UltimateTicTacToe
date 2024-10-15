using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hideMe : MonoBehaviour
{
	GameObject bigButton;
	// Start is called before the first frame update
	void Start()
	{
		bigButton = GameObject.Find("RestartButton");
	}

	// Update is called every frame, if the MonoBehaviour is enabled.
	protected void Update()
	{
		gameObject.SetActive(!bigButton.activeSelf);
	}


}
