using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameState : MonoBehaviour
{
    private bool playerTurn = true;
    public const float TurnLength = 2f;
    UserClicked userClicked;

    void Awake()
    {
        userClicked = FindObjectOfType<UserClicked>();
        Assert.IsNotNull(userClicked, "UserClicked cannot be null");
    }

    void OnGUI()
    {
        if (playerTurn)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (GUILayout.Button("End Turn"))
            {
                userClicked.ExplicitlyConsume();
                StartCoroutine(EndTurn());
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
        yield return new WaitForEndOfFrame();
        playerTurn = false;
        yield return new WaitForSeconds(TurnLength);
        playerTurn = true;
    }
}
