public interface ToggleableUI
{
    /// <summary>
    /// Opens the UI and passes control to the UI.
    /// </summary>
    public void Open();

    /// <summary>
    /// Closes the UI and closes all child UIs opened by this UI, then optionally passes control to the player.
    /// Note that if it is the lowest level UI and control is not passed back to the player, they will be unable to move.
    /// </summary>
    public void Close();
}
