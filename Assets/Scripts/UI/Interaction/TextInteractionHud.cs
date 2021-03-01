using UnityEngine;
using UnityEngine.UI;

public class TextInteractionHud : MonoBehaviour, InteractionHud
{
    private Text interactionText;
    void Start()
    {
        UIManager.SetInteractionHud(this);
        interactionText = gameObject.GetComponent<Text>();
    }
    public void Disable()
    {
        interactionText.text = null;
        gameObject.SetActive(false);
    }

    public void Enable(string text)
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
        interactionText.text = text;
    }
}
