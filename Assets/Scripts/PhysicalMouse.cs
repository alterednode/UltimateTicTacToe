using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalMouse : MonoBehaviour
{
    GameManager gameManager;
    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        // if we find gamemanager, we're in a game. If not, we're in a menu
        try
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
        catch
        {
            Debug.Log("PhysicalMouse: GameManager not found");
            audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        }
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
            else if (gameManager == null)
            {
                //only do this in a menu, otherwise this is gonna make this sound play whenever you click wherever
                //sorry, this shit sucks ass, will fix later
                audioManager.PlayClip("ClickSucceed");
            }
        }
    }
}
