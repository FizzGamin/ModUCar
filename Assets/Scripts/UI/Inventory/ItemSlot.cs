using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Reflection;
using System;

public abstract class ItemSlot : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
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

    public virtual void SetItem(IItem item)
    {
        currentItem = item;
        UpdateImageSprite();
    }

    public virtual IItem Swap(IItem newItem)
    {
        if (!CanHold(newItem)) throw new ArgumentException("Attempted to swap in an item that was not able to be held into an item slot");
        IItem ret = GetItem();
        SetItem(newItem);
        return ret;
    }
    public abstract bool CanHold(IItem item);
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

    private void UpdateImageSprite()
    {
        if (currentItem != null)
        {
            Sprite itemSprite = GetSpriteByName(currentItem.GetSpriteName());
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

    public void OnDrag(PointerEventData eventData)
    {
        slotImage.transform.position = Input.mousePosition;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        slotImage.transform.localPosition = Vector3.zero;
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            Debug.Log(eventData.pointerCurrentRaycast.gameObject.name);
            ItemSlot slot = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemSlot>();
            if (slot != null)
            {
                SetItem(slot.Swap(GetItem()));
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
}
