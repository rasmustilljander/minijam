using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System;

public class Ship : MonoBehaviour
{
    GameState gameState;

    public bool selected = false;
    public float speed = 25f;

    Vector3 direction = Vector3.up;

    Queue<Tuple<Vector2, Vector2>> moves = new Queue<Tuple<Vector2, Vector2>>();
    UserClicked userClicked;

    void Awake()
    {
        gameState = FindObjectOfType<GameState>();
        Assert.IsNotNull(gameState, "Gamestate cannot be null");
        userClicked = gameObject.AddComponent<UserClicked>();
    }

    float timeSinceLastMove = 0;
    int interPolationSteps = 50;
    Vector3 actualTargetVector = Vector3.zero;
    void Update()
    {
        if (selected && gameState.isPlayerTurn())
        {
            if (userClicked.DidUserClick())
            {
                Camera camera = Camera.main;
                Vector3 desiredTarget = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -camera.transform.position.z));
                Vector3 currentVector = transform.position + direction.normalized * Mathf.Pow(speed, 1 / 4f);

                float distance = 0;
                for (int i = 0; i < interPolationSteps; ++i)
                {
                    Vector3 prev = currentVector;
                    currentVector = Vector3.Slerp(currentVector, desiredTarget, 0.05f * i);
                    moves.Enqueue(new Tuple<Vector2, Vector2>(prev, currentVector));
                    distance += Vector3.Distance(prev, currentVector);
                    if (distance > speed)
                    {
                        break;
                    }
                }
            }
        }
        else
        {
            if(moves.Count == 0)
            {
                return;
            }
            timeSinceLastMove += Time.deltaTime;
            if (timeSinceLastMove > GameState.TurnLength / interPolationSteps)
            {
                timeSinceLastMove = 0;
                actualTargetVector = moves.Dequeue().Item2;
            }
            Vector3 vel = Vector3.zero;
            Vector3 prev = transform.position;
            transform.position = actualTargetVector;
            //Vector3.SmoothDamp(transform.position, actualTargetVector, ref vel, GameState.TurnLength / interPolationSteps / Time.deltaTime);
            direction = transform.position - prev;
        }
    }

    void OnGUI()
    {
        if (selected)
        {
            GUILayout.BeginArea(new Rect(10, 300, 300, 300));
            if (GUILayout.Button($"Selected ship {gameObject.name}"))
            {
            }
            GUILayout.EndArea();
        }
    }

    void OnDrawGizmos()
    {
        foreach (var gizmo in moves)
        {
            Gizmos.DrawLine(gizmo.Item1, gizmo.Item2);
            Gizmos.DrawCube(gizmo.Item2, Vector3.one * 0.3f);
        }
    }
}