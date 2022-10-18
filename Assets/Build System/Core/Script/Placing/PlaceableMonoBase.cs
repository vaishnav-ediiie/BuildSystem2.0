using System;
using UnityEngine;

namespace CustomBuildSystem.Placing
{
    public abstract class PlaceableMonoBase : MonoBehaviour
    {
        public GameObject placingOkay;
        public GameObject placingError;
        public GameObject placed;
    
        [SerializeField] internal int ID;
        [SerializeField] internal Sprite Icon;
        [SerializeField] internal bool scaleToCellSize;
        [SerializeField] internal bool isDecorator = false;
        [SerializeField, DrawIfBool("isDecorator", true)] internal bool parentRelativeRotation = false;
        [SerializeField, DrawIfBool("isDecorator", true)] internal int rotationOffset = 0;
        
        
        public enum CombineMode
        {
            And,
            Or
        }
    }

}