using System;
using System.Collections.Generic;
using CustomBuildSystem;
using CustomBuildSystem.Placing;
using UnityEngine;


public class SelectItemPopup : MonoBehaviour
{
    [SerializeField] private RMF_RadialMenu radialMenu;
    [SerializeField] private RMF_RadialMenuElement menuElement;
    [SerializeField] private Transform elementsParent;
    private Action<PlaceableMonoBase> onCompleteAction;
    private PlaceableMonoBase[] placeables;

    public void Init(PlaceableMonoBase[] placeables, Action<PlaceableMonoBase> onCompleteAction)
    {
        this.onCompleteAction = onCompleteAction;
        radialMenu.elements = new List<RMF_RadialMenuElement>();
        
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent.GetComponent<RectTransform>(), Input.mousePosition, Camera.current, out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
        
        this.placeables = placeables;
        int i = 0;
        int elementsCount = placeables.Length;
        float angleOffset = 360f / elementsCount;
        
        foreach (PlaceableMonoBase placeable in placeables)
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
