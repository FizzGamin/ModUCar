using UnityEngine;

public abstract class ToggleableUI : MonoBehaviour
{
    protected UserControllable controller;
    protected bool isOpen;

    /// <summary>
    /// Opens the UI and passes control to the UI.
    /// </summary>
    public virtual void Open(UserControllable controller)
    {
        gameObject.SetActive(true);
        this.controller = controller;
        isOpen = true;
    }

    /// <summary>
    /// Closes the UI and closes all child UIs opened by this UI, then optionally passes control to the player.
    /// Note that if it is the lowest level UI and control is not passed back to the player, they will be unable to move.
    /// </summary>
    public virtual void Close()
    {
        gameObject.SetActive(false);
        controller.GiveControl();
        controller = null;
        isOpen = false;
    }

    /// <summary>
    /// A method that will allow the UI to close itself if escape is pressed while it is open
    /// </summary>
    protected void CloseOnEscape()
    {
        if (isOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.Close();
            }
        }
    }

    protected void CloseOnF()
    {
        if (isOpen)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                this.Close();
            }
        }
    }
}
