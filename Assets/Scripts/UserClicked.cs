using UnityEngine;

public class UserClicked : MonoBehaviour
{
    private bool lastButtonState = false;

    public bool DidUserClick()
    {
        bool newButtonState = GetCurrentButtonState();
        return newButtonState && !lastButtonState;
    }

    void LateUpdate()
    {
        bool newButtonState = GetCurrentButtonState();
        lastButtonState = newButtonState;
    }

    bool GetCurrentButtonState()
    {
        return Input.GetMouseButton(0);
    }
}
