using UnityEngine;

public class UserClicked : MonoBehaviour
{
    private bool lastButtonState = false;
    private bool currentButtonState = false;

    public KeyCode keyCodeToCheck;

    public bool DidUserClick()
    {
        return currentButtonState && !lastButtonState;
    }

    private void Update()
    {
        lastButtonState = currentButtonState;
        currentButtonState = GetCurrentButtonState();
    }

    private bool GetCurrentButtonState()
    {
        return Input.GetKey(keyCodeToCheck);
    }
}
