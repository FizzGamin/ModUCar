using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Reflection;

public class ItemSlot : MonoBehaviour, IDragHandler
{
    protected IItem currentItem;
    protected Image slotImage;

    public void Awake()
    {
        slotImage = gameObject.GetComponentsInChildren<Image>(true).Where((img) => img.gameObject != gameObject).First();
    }

    public IItem GetItem()
    {
        return currentItem;
    }

    public void SetItem(IItem item)
    {
        currentItem = item;
        if (item != null)
        {
            Sprite itemSprite = GetSpriteByName(item.GetSpriteName());
            if (itemSprite != null)
            {
                slotImage.sprite = itemSprite;
            }
            else
            {
                slotImage.sprite = GameManager.GetSpriteService().GetMissingSprite();
            }
            slotImage.gameObject.SetActive(true);
        }
        else
        {
            slotImage.sprite = null;
            slotImage.gameObject.SetActive(false);
        }
    }

    public void Select()
    {
        transform.localScale = new Vector3(.95f, .95f, .95f);
    }

    public void Deselect()
    {
        transform.localScale = new Vector3(.9f, .9f, .9f);
    }

    private Sprite GetSpriteByName(string name)
    {
        return GameManager.GetSpriteService().GetSprite(name);
    }

    public void OnDrag(PointerEventData eventData)
    {
        slotImage.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        slotImage.transform.localPosition = Vector3.zero;
    }
}
