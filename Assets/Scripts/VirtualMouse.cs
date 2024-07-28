using UnityEngine;
using System.Collections;

public class VirtualMouse : MonoBehaviour {

	public bool isPlayer1; 
	public float speed = 20f; // Speed of the cursor movement
	public string horizAxisName;
	public string vertAxisName;
	public bool inUse = false;
	GameManager gameManager;
	
    private Vector3 cursorPosition;
	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		cursorPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		// Get input from the left stick
        float horizontal = Input.GetAxis(horizAxisName);
		float vertical = Input.GetAxis(vertAxisName);
        
        
		// we check to see if the controller is used at all and if it is, we know that this controller is in use and make the cursor visible
		if(horizontal!=0||vertical!=0) {inUse = true;}
		transform.GetChild(0).gameObject.SetActive(inUse);
		if(!inUse){return;}

        // Update the cursor position based on input
        cursorPosition += new Vector3(horizontal, vertical, 0) * speed * Time.deltaTime;

		Vector3 screenPos = Camera.main.WorldToScreenPoint(cursorPosition);


        // Clamp the cursor position to the screen bounds (optional)
		//Debug.Log("Screen width: " + Screen.width);
        screenPos.x = Mathf.Clamp(screenPos.x, 0, Screen.width);
        screenPos.y = Mathf.Clamp(screenPos.y, 0, Screen.height);

		cursorPosition = Camera.main.ScreenToWorldPoint(screenPos);


        // Apply the new position to the cursor GameObject
		transform.position = cursorPosition;
        
		if(gameManager.controller1.inUse&&gameManager.controller2.inUse)
		{
			if(isPlayer1!=gameManager.xPlayerTurn)
			{
				return;
			}
		}

		RaycastHit hit = new RaycastHit();
		Ray ray = new Ray(this.transform.position, Vector3.forward * 10);
		//Debug.DrawRay(this.transform.position, Vector3.forward * 10, Color.red);

		if (Physics.Raycast(ray, out hit, 1000.0f)){
			hit.transform.SendMessage ("VOnMouseOver");
		}
	}
}
