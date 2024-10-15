using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool multiplayerEnabled;
    public bool canHumanPlayerPlay;

    public bool xPlayerTurn;
    public GameObject indicator;
    public GameObject[] prefabXO;

    GameObject currentPlayerThing;
    GameObject notCurrentPlayerThing;

    public GameObject restartButton;

    private bool c_xPlayerTurn;

    public VirtualMouse controller1;
    public VirtualMouse controller2;

    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        canHumanPlayerPlay = true;
        xPlayerTurn = Random.value > 0.5f;
        c_xPlayerTurn = !xPlayerTurn;
        if (multiplayerEnabled == false)
        {
            restartButton = GameObject.Find("RestartButton");
            restartButton.SetActive(false);
        }

        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    // Update is called once per frame

    void Update()
    {
        AnimateIndicator(); //animate the indicator of which player's turn it is
    }

    public void Restart()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    void AnimateIndicator()
    {
        //move the indicator based on screen width / height

        if (Screen.width > Screen.height)
        {
            indicator.transform.position = Vector3.Lerp(
                new Vector3(-5.62f, 0, 0),
                indicator.transform.position,
                0.5f
            ); // if screen wider than tall, indicator on the left
        }
        else
        {
            indicator.transform.position = Vector3.Lerp(
                new Vector3(0, 5.4f, 0),
                indicator.transform.position,
                0.5f
            ); // if screen taller than wide, indicator on right
        }

        // check to see if the player's turn has changed
        if (c_xPlayerTurn != xPlayerTurn)
        {
            /*
            dumb logic to change the big / small one
            
            */
            if (xPlayerTurn)
            {
                notCurrentPlayerThing = indicator.transform.Find("O").gameObject;
                currentPlayerThing = indicator.transform.Find("X").gameObject;
            }
            else
            {
                currentPlayerThing = indicator.transform.Find("O").gameObject;
                notCurrentPlayerThing = indicator.transform.Find("X").gameObject;
            }
        }

        // weird lerps to change position and scale
        currentPlayerThing.transform.position = Vector3.Lerp(
            currentPlayerThing.transform.position,
            (indicator.transform.position + new Vector3(-.5f, 0, 0)),
            0.08f
        );
        if (currentPlayerThing.transform.localScale.x < 2)
        {
            currentPlayerThing.transform.localScale *= 1.2f;
        }
        notCurrentPlayerThing.transform.position = Vector3.Lerp(
            notCurrentPlayerThing.transform.position,
            (indicator.transform.position + new Vector3(.5f, 0, 0)),
            0.08f
        );
        c_xPlayerTurn = xPlayerTurn;
        if (notCurrentPlayerThing.transform.localScale.x > .8)
        {
            notCurrentPlayerThing.transform.localScale *= .9f;
        }
    }

    public static bool CheckPlayable(int[] checkThis)
    {
        foreach (int x in checkThis)
        {
            if (x.Equals(0)) //see if there is any empty spaces in the array
            {
                return true; // if there is > 0 empty spaces this space is playable
            }
        }
        return false; // if there are no playable spaces return unplable
    }

    public static int CheckForWin(int[] checkThis) //checks columns, rows and diagonals to find three 1 or -1 in a row
    {
        int checkValue = 0;

        int val1 = 0;
        int val2 = 1;
        int val3 = 2;

        for (int i = 0; i < 9; i++)
        {
            if (i < 3)
            {
                checkValue = checkThis[val1] + checkThis[val2] + checkThis[val3];
                val1 += 3;
                val2 += 3;
                val3 += 3;
            }
            else
            {
                if (i < 6)
                {
                    if (i == 3)
                    {
                        val1 = 0;
                        val2 = 3;
                        val3 = 6;
                    }
                    checkValue = checkThis[val1] + checkThis[val2] + checkThis[val3];
                    val1 += 1;
                    val2 += 1;
                    val3 += 1;
                }
                else
                {
                    if (i == 7)
                    {
                        checkValue = checkThis[0] + checkThis[4] + checkThis[8];
                    }
                    if (i == 8)
                    {
                        checkValue = checkThis[2] + checkThis[4] + checkThis[6];
                    }
                }
            }

            if (checkValue == 3 || checkValue == -3)
            {
                return (checkValue);
            }
        }
        return 0;
    }
}
