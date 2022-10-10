using System;
using System.Collections.Generic;
using CustomBuildSystem;
using UnityEngine;


public class SelectItemPopup : MonoBehaviour
{
    [SerializeField] private RMF_RadialMenu radialMenu;
    [SerializeField] private RMF_RadialMenuElement menuElement;
    [SerializeField] private Transform elementsParent;
    private Action<PlaceableSOBase> onCompleteAction;
    private PlaceableSOBase[] placeables;

    public void Init(PlaceableSOBase[] placeables, Action<PlaceableSOBase> onCompleteAction)
    {
        this.onCompleteAction = onCompleteAction;
        radialMenu.elements = new List<RMF_RadialMenuElement>();

        
        this.placeables = placeables;
        int i = 0;
        int elementsCount = placeables.Length;
        float angleOffset = 360f / elementsCount;
        
        foreach (PlaceableSOBase placeable in placeables)
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
        if (index >= 0 && index < placeables.Length)
        {
            onCompleteAction.Invoke(placeables[index]);
        }
        else
        {
            onCompleteAction.Invoke(null);
        }
        Destroy(gameObject);
    }
}
