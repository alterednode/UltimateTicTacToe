using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GUIConsoleDebugging : MonoBehaviour
{
    // Adjust via the Inspector
    public int maxLines = 8;
    private Queue<string> queue = new Queue<string>();
    private string currentText = "";

    public Text consoleText;

    //public int lines = 1;

    void Start(){
        consoleText = GameObject.Find("ConsoleTextLog").GetComponent<Text>();
    }
    void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Delete oldest message
        if (queue.Count >= maxLines) queue.Dequeue();

        queue.Enqueue(logString);
        //lines++;

        var builder = new StringBuilder();
        foreach (string st in queue)
        {
            builder.Append(st).Append("\n\n");
        }

        currentText = builder.ToString();

        consoleText.text = currentText;
    }

    // void OnGUI()
    // {
    //     GUI.Label(
    //        new Rect(
    //            5,                   // x, left offset
    //            Screen.height - 150, // y, bottom offset
    //            480f,                // width
    //            720f                 // height
    //        ),
    //        currentText,             // the display text
    //        GUI.skin.textArea        // use a multi-line text area
    //     );
    // }
}
