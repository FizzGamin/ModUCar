using UnityEngine;
using UnityEngine.UI;

public class TextInteractionHud : MonoBehaviour, InteractionHud
{
    public Text interactionText;
    void Start()
    {
        UIManager.SetInteractionHud(this);
    }

    public void Disable()
    {
        interactionText.text = "";
        gameObject.SetActive(false);
    }

    public void Enable(string text)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        interactionText.text = text;
    }
}
