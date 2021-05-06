using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericBarUI : MonoBehaviour
{
    [SerializeField]
    protected GameObject barImage;
    public void SetBar(double current, double capacity)
    {
        RectTransform rt = barImage.GetComponent<RectTransform>();
        double currentOrZero = (current < 0) ? 0 : current;
        rt.sizeDelta = new Vector2((float)(currentOrZero / capacity * 100) * 6.4f, 80);
    }
}
