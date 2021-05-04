using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelBarUI : MonoBehaviour
{
    [SerializeField]
    private GameObject fuelImage;
    private void Awake()
    {
        UIManager.SetFuelBarUI(this);
        gameObject.SetActive(false);
    }

    public void SetFuel(double current, double capacity)
    {
        RectTransform rt = fuelImage.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2((float)(current/capacity * 100) * 6.4f, 80);
    }
}
