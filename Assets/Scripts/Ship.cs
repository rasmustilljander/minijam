using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System;

public class Ship : MonoBehaviour
{
    GameState gameState;
    UserClicked userClicked;

    public bool selected = false;
    public float speed = 25f;

    Queue<Tuple<Vector2, Vector2>> moves = new Queue<Tuple<Vector2, Vector2>>();

    float timeSinceLastMove = 0;
    int interPolationSteps = 50;
    Vector3 actualTargetVector = Vector3.zero;

    void Awake()
    {
        gameState = FindObjectOfType<GameState>();
        Assert.IsNotNull(gameState, "Gamestate cannot be null");
        userClicked = FindObjectOfType<UserClicked>();
        Assert.IsNotNull(userClicked, "UserClicked cannot be null");
        userClicked.userClickedEvent += UserClicked;
    }

    void OnDestroy()
    {
        userClicked.userClickedEvent -= UserClicked;
    }

    bool UserClicked()
    {
        if(!selected)
        {
            return false;
        }
        moves.Clear();
        Camera camera = Camera.main;
        Vector3 desiredTarget = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -camera.transform.position.z));
        Vector3 currentVector = transform.position + transform.up.normalized * Mathf.Pow(speed, 1 / 4f);

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
        return true;
    }

    void Update()
    {
        if(!gameState.isPlayerTurn())
        {
            if (moves.Count == 0)
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
            Vector3 direction = transform.position - prev;

            //direction = Vector3.RotateTowards(transform.forward, direction, float.MaxValue, 0f);
            //transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void OnGUI()
    {
        if (selected)
        {
            GUILayout.BeginArea(new Rect(10, 300, 300, 300));
            if (GUILayout.Button($"Selected ship {gameObject.name}"))
            {
                userClicked.ExplicitlyConsume();
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