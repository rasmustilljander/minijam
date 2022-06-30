using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private bool playerTurn = true;
    public const float TurnLength = 2f;

    void Start()
    {
    }

    void OnGUI()
    {
        if(playerTurn)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (GUILayout.Button("End Turn"))
            {
                playerTurn = false;
            }
            GUILayout.EndArea();
        }
    }

    public bool isPlayerTurn()
    {
        return playerTurn;
    }

    IEnumerator EndTurn()
    {
        yield return new WaitForSeconds(TurnLength);
        playerTurn = true;
    }
}
