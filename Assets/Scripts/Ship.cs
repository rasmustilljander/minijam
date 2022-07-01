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
    public int preInterPolationSteps = 50;
    public int interPolationSteps = 50;
    Vector3 actualTargetVector = Vector3.zero;

    void Awake()
    {
        gameState = FindObjectOfType<GameState>();
        Assert.IsNotNull(gameState, "Gamestate cannot be null");

        userClicked = gameObject.AddComponent<UserClicked>();
        userClicked.keyCodeToCheck = KeyCode.Mouse1;

        lineRenderer = gameObject.GetComponent<LineRenderer>();
        Assert.IsNotNull(lineRenderer, "LineRenderer cannot be null");
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.numCornerVertices = 1;
        lineRenderer.useWorldSpace = true;
    }

    private bool ComputePath()
    {
        moves.Clear();
        gizmoMoves.Clear();
        Vector3 offset = transform.position;
        Vector3 currentVector = transform.up * 0.1f;

        Camera camera = Camera.main;
        Vector3 desiredTarget = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -camera.transform.position.z));
        desiredTarget -= offset;
        {
            // Second part of the movement
            float distance = 0;
            float deltaT = 1f / interPolationSteps;
            for (int i = 0; i < interPolationSteps; ++i)
            {
                Vector3 prev = currentVector;
                currentVector = Vector3.Slerp(currentVector, desiredTarget, deltaT * i);
                float previusDistance = distance;
                distance += Vector3.Distance(prev, currentVector);
                if (!Mathf.Approximately(previusDistance, distance))
                {
                    moves.Add(currentVector + offset);
                    gizmoMoves.Enqueue(new Tuple<Vector2, Vector2>(prev + offset, currentVector + offset));
                }
            }

            lineRenderer.positionCount = moves.Count;
            lineRenderer.SetPositions(moves.ToArray());
        }
        return true;
    }

    void Update()
    {
        if (gameState.isPlayerTurn())
        {
            if (selected && userClicked.DidUserClick())
            {
                ComputePath();
            }
        }
        else
        {
            if (gizmoMoves.Count > 0)
            {
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

                //direction = Vector3.RotateTowards(transform.up, direction, float.MaxValue, 0f);
                //transform.rotation = Quaternion.LookRotation(direction);

                Vector2 prevDirection2d = transform.up;
                Vector2 currentDirection2d = direction;
                float angle = Vector2.SignedAngle(prevDirection2d, currentDirection2d);
                transform.RotateAround(transform.transform.position, Vector3.forward, angle);
                //transform.LookAt(direction, );
            }
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