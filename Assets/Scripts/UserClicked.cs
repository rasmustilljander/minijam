using System.Collections;
using UnityEngine;

public class UserClicked : MonoBehaviour
{
    private bool lastButtonState = false;
    private bool currentButtonState = false;

    public delegate bool UserClickedEvent();
    public UserClickedEvent userClickedEvent;
    private bool explicitlyConsumed = true;

    public void ExplicitlyConsume()
    {
        Debug.Log("ExplicitlyConsume");
        explicitlyConsumed = true;
    }

    private bool DidUserClick()
    {
        return currentButtonState && !lastButtonState;
    }

    private IEnumerator ConsumeClick()
    {
        Debug.Log("ConsumeClick");
        yield return new WaitForEndOfFrame();
        if (explicitlyConsumed)
        {
            yield return null;
        }

        int i = 0;
        foreach (var callback in userClickedEvent.GetInvocationList())
        {
            Debug.Log($"ABC{i++}");
            bool consumed = (bool)callback.DynamicInvoke();
            if (consumed)
            {
                explicitlyConsumed = true;
                break;
            }
        }
    }

    void Update()
    {
        explicitlyConsumed = false;
        lastButtonState = currentButtonState;
        currentButtonState = GetCurrentButtonState();
        if (DidUserClick())
        {
            StartCoroutine(ConsumeClick());
        }
    }

    bool GetCurrentButtonState()
    {
        return Input.GetMouseButton(0);
    }
}
