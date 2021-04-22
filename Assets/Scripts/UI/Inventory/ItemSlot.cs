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
            ItemSlot slot = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<ItemSlot>();
            if (slot != null && slot.CanHold(GetItem()) && this.CanHold(slot.GetItem()))
            {
                IItem temp = GetItem();
                IItem temp2 = slot.GetItem();
                SetItem(null);
                slot.SetItem(temp);
                SetItem(temp2);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
}
