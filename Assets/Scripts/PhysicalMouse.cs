using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalMouse : MonoBehaviour
{
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out raycastHit, 100f))
            {
                gameManager.audioManager.PlayClip("ClickFail");
            }
        }
    }
}
