using System;
using UnityEngine;

public abstract class UserControllable : MonoBehaviour
{
    protected bool isControlled = false;
    //This keeps track of whether or not the key-presses made are going to be handled by the player or by something else, for example, being in menu
    private ToggleableUI currentlyOpen = null;

    /// <summary>
    /// A method to allow communication between the controllable object and other objects that involve key bindings which gives this particular object primary control
    /// </summary>
    public abstract void GiveControl();

    /// <summary>
    /// Allows other scripts to force this component to give up its primary control
    /// </summary>
    public abstract void ReleaseControl();

    protected void HandlePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isControlled)
            {
                ReleaseControl();
                currentlyOpen = UIManager.GetPauseMenuUI();
                currentlyOpen.Open(this);
            }
            else
            {
                if (currentlyOpen != null)
                {
                    currentlyOpen.Close();
                    currentlyOpen = null;
                    this.GiveControl();
                }
            }
        }
    }
}
