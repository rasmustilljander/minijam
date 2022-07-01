using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System;

public class Ship : MonoBehaviour
{
    GameState gameState;
    UserClicked userClicked; 
    LineRenderer lineRenderer;

    public bool selected = false;
    public float speed = 25f;

    Queue<Tuple<Vector2, Vector2>> gizmoMoves = new Queue<Tuple<Vector2, Vector2>>();
    List<Vector3> moves = new List<Vector3>();

    float timeSinceLastMove = 0;
    int interPolationSteps = 50;
    Vector3 actualTargetVector = Vector3.zero;

    void Awake()
    {
        gameState = FindObjectOfType<GameState>();
        Assert.IsNotNull(gameState, "Gamestate cannot be null");
    
        userClicked = gameObject.AddComponent<UserClicked>();
        userClicked.keyCodeToCheck = KeyCode.Mouse1;

        lineRenderer = gameObject.GetComponent<LineRenderer>();
        Assert.IsNotNull(lineRenderer, "LineRenderer cannot be null");
    }

    private bool ComputePath()
    {
        if(!selected)
        {
            return false;
        }
        moves.Clear();
        gizmoMoves.Clear();
        Camera camera = Camera.main;
        Vector3 desiredTarget = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -camera.transform.position.z));
        Vector3 currentVector = transform.position + transform.up.normalized * Mathf.Pow(speed, 1 / 4f);

        float distance = 0;
        for (int i = 0; i < interPolationSteps; ++i)
        {
            Vector3 prev = currentVector;
            currentVector = Vector3.Slerp(currentVector, desiredTarget, 0.05f * i);
            moves.Add(currentVector);
            gizmoMoves.Enqueue(new Tuple<Vector2, Vector2>(prev, currentVector));
            distance += Vector3.Distance(prev, currentVector);
            if (distance > speed)
            {
            //    break;
            }
        }
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.numCornerVertices = 1;
        lineRenderer.positionCount = 50;
        lineRenderer.SetPositions(moves.ToArray());
        foreach(var move in moves)
        {
            Debug.Log(move);
        }
        lineRenderer.useWorldSpace = true;
        Debug.Log(moves.ToArray().Length);
        Debug.Log(moves.Count);
        Debug.Log(lineRenderer.positionCount);
        return true;
    }

    void Update()
    {
        if(gameState.isPlayerTurn())
        {
            if(selected && userClicked.DidUserClick())
            {
                ComputePath();
            }
        }
        else
        {
            if (gizmoMoves.Count == 0)
            {
                return;
            }
            timeSinceLastMove += Time.deltaTime;
            if (timeSinceLastMove > GameState.TurnLength / interPolationSteps)
            {
                timeSinceLastMove = 0;
                actualTargetVector = gizmoMoves.Dequeue().Item2;
            }
            Vector3 vel = Vector3.zero;
            Vector3 prevPosition = transform.position;
            transform.position = actualTargetVector;
            //Vector3.SmoothDamp(transform.position, actualTargetVector, ref vel, GameState.TurnLength / interPolationSteps / Time.deltaTime);
            Vector3 direction = transform.position - prevPosition;

            //direction = Vector3.RotateTowards(transform.forward, direction, float.MaxValue, 0f);
            //transform.rotation = Quaternion.LookRotation(direction);

            Vector2 prevDirection2d = transform.up;
            Vector2 currentDirection2d = direction;
            float angle = Vector2.Angle(prevDirection2d, currentDirection2d);
            transform.RotateAround(transform.transform.position, Vector3.forward, angle);
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
        foreach (var gizmo in gizmoMoves)
        {
            Gizmos.DrawLine(gizmo.Item1, gizmo.Item2);
            Gizmos.DrawCube(gizmo.Item2, Vector3.one * 0.3f);
        }
    }
}