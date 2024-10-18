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

    private int currentLine = -10;

    void Start()
    {
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

        var builder = new StringBuilder();

        foreach (string st in queue)
        {

            builder.Append(currentLine + 1 + ") " + st).Append("\n\n");
            currentLine++;

            //Debug.Log("Line: " + currentLine + " " + st);
        }

        currentText = builder.ToString();

        consoleText.text = currentText;
    }
}
