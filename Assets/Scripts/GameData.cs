using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class GameData
{
    public string gameid, uuid0, uuid1;
    // TODO: could possibly optimize storage sapce of a Game object with byte array
    // instead of int
    public int[] gameState;
    public int lastMove;
    
    public bool player0toPlayNext;
    // created timestamp
    // updated timestamp

   public GameData(string gameid, string uuid0, string uuid1, int[] gameSate, int lastMove, bool player0toPlayNext)
    {
        this.gameid = gameid;
        this.uuid0 = uuid1;
        this.uuid1 = uuid0;
        this.gameState = gameSate;
        this.lastMove = lastMove;
        this.player0toPlayNext = player0toPlayNext;
    }

    int gridNum(int location)
    {
        return location / 9;
    }

    int subGridNum(int location)
    {
        return location % 9;
    }


    bool gridStaleAtGridNum(int bigGrid)
    {
        int offset = bigGrid * 9;
        for (int i = 0; i < 9; i++)
        {
            if ((int)gameState[i + offset] == 0)
                return false;
        }
        return true;
    }

    public int gridWonAtGridNum(int bigGrid) // checks columns, rows and diagonals to find three 1 or -1 in a row
    {
        int offset = bigGrid * 9;

        int checkValue = 0;

        int val1 = 0;
        int val2 = 1;
        int val3 = 2;

        for (int i = 0; i < 9; i++)
        {
            if (i < 3)
            {
                checkValue = gameState[offset + val1] + gameState[offset + val2] + gameState[offset + val3];
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
                    checkValue = gameState[offset + val1] + gameState[offset + val2] + gameState[offset + val3];
                    val1 += 1;
                    val2 += 1;
                    val3 += 1;
                }
                else
                {
                    if (i == 7)
                    {
                        checkValue = gameState[offset + 0] + gameState[offset + 4] + gameState[offset + 8];
                    }
                    if (i == 8)
                    {
                        checkValue = gameState[offset + 2] + gameState[offset + 4] + gameState[offset + 6];
                    }
                }
            }

            if (checkValue == 3 || checkValue == -3)
            {
                return (checkValue / 3);
            }
        }
        return 0;
    }

    public static bool locationInBounds(int location)
    {
        return (0 <= location && location < 81);
    }
    public static bool validStateOption(int num)
    {
        return ((-1 <= num && num <= 1));
    }
}
