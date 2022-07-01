using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ShipSelector : MonoBehaviour
{
    Ship selectedShip = null;
    List<Tuple<Vector2, Vector2>> gizmos = new List<Tuple<Vector2, Vector2>>();
    UserClicked userClicked;

    void Awake()
    {
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
        Camera camera = Camera.main;
        Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane));
        //gizmos.Add(new Tuple<Vector2, Vector2>(screenToWorld, screenToWorld + new Vector3(0, 0, 100)));

        RaycastHit hit;
        Vector3 direction = screenToWorld - camera.transform.position;
        bool rayCheck = Physics.Raycast(screenToWorld, direction, out hit, 100f);

        if (rayCheck)
        {

            Debug.Log($"hit.collider.gameobject.name={hit.collider.gameObject.name}");
            Ship newSelectedShip = hit.collider.gameObject.transform.parent.parent.GetComponentInChildren<Ship>();
            if (newSelectedShip)
            {
                StartCoroutine(UpdateSelectedShip(newSelectedShip));
                return true;
            }
        }
        return false;
    }

    IEnumerator UpdateSelectedShip(Ship newSelectedShip)
    {
        yield return new WaitForEndOfFrame();
        if (selectedShip)
        {
            selectedShip.selected = false;
        }
        selectedShip = newSelectedShip;
        selectedShip.selected = true;
    }

    void OnDrawGizmos()
    {
        foreach (var gizmo in gizmos)
        {
            Gizmos.DrawLine(gizmo.Item1, gizmo.Item2);
            Gizmos.DrawCube(gizmo.Item2, Vector3.one * 0.3f);
        }
    }
}
