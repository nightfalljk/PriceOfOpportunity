using System;
using System.Collections;
using System.Collections.Generic;
using Features.Buildings;
using TMPro;
using UnityEngine;

public class BuildingMenu : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject tooltip;
    
    

    private TMP_Text _tooltipText;

    //TODO: Add images for the different construction buttons
    //TODO: Add tooltip information for building when player hovers over button

    private void Awake()
    {
        _tooltipText = tooltip.GetComponentInChildren<TMP_Text>();
    }

    public void StartConstruction(BuildingData buildingData)
    {
        player.EnterBuildMode(buildingData);
    }

    public void DisplayTooltip(BuildingData buildingData)
    {
        return;
        Debug.Log("Entering");
        Vector3 mousePos = player.GetMousePosition();
        _tooltipText.text = buildingData.ToString();
        Canvas.ForceUpdateCanvases();
        Rect tooltipRect = tooltip.GetComponent<RectTransform>().rect;
        Debug.Log("" + tooltipRect.height + " " + tooltipRect.width);
        Vector3 tooltipPos = new Vector3(mousePos.x + tooltipRect.width/4, mousePos.y + tooltipRect.height/4, 0);
        tooltip.transform.position = tooltipPos;
        tooltip.SetActive(true);
    }

    public void DeactivateTooltip()
    {
        return;
        tooltip.SetActive(false);
    }
}
