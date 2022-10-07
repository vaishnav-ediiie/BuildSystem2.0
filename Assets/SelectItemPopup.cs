using System;
using System.Collections.Generic;
using CustomBuildSystem;
using UnityEngine;


public class SelectItemPopup : MonoBehaviour
{
    [SerializeField] private RMF_RadialMenu radialMenu;
    [SerializeField] private RMF_RadialMenuElement menuElement;
    [SerializeField] private Transform elementsParent;
    private Action<EdgePlaceableSO, CellPlaceableSO> onCompleteAction;
    private EdgePlaceableSO[] edgePlaceables;
    private CellPlaceableSO[] cellPlaceables;

    public void Init(EdgePlaceableSO[] edgePlaceables, CellPlaceableSO[] cellPlaceables, Action<EdgePlaceableSO, CellPlaceableSO> onCompleteAction)
    {
        this.onCompleteAction = onCompleteAction;
        radialMenu.elements = new List<RMF_RadialMenuElement>();

        
        this.edgePlaceables = edgePlaceables;
        this.cellPlaceables = cellPlaceables;
        int i = 0;
        int elementsCount = edgePlaceables.Length + cellPlaceables.Length;
        float angleOffset = 360f / elementsCount;
        
        foreach (EdgePlaceableSO placeable in edgePlaceables)
        {
            CreateElement(angleOffset, placeable.Icon, i);
            i++;
        }
        foreach (CellPlaceableSO placeable in cellPlaceables)
        {
            CreateElement(angleOffset, placeable.Icon, i);
            i++;
        }
    }

    void CreateElement(float angleOffset, Sprite icon, int i)
    {
        RMF_RadialMenuElement element = Instantiate(menuElement, elementsParent);
        float rotation = -(angleOffset * i) - radialMenu.globalOffset;
        element.Init(rotation, icon, radialMenu, i);
        element.setAllAngles((angleOffset * i) + radialMenu.globalOffset, angleOffset);
        radialMenu.elements.Add(element);
    }
    
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))return;
        
        int index = radialMenu.selectedIndex;
        if (index >= 0)
        {
            if (index < edgePlaceables.Length) onCompleteAction.Invoke(edgePlaceables[index], null);
            else onCompleteAction.Invoke(null, cellPlaceables[index - edgePlaceables.Length]);
        }
        else
        {
            onCompleteAction.Invoke(null, null);
        }
        Destroy(gameObject);
    }
}
