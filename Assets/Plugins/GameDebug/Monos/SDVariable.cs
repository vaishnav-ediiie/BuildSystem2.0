using System;
using UnityEngine;

namespace DebugToScreen
{
    public class SDVariable : MonoBehaviour
    {
        [SerializeField, Tooltip("Duration <=0 meaning this message will be there forever")]
        private float duration = -1f;

        [SerializeField] private DebugType debugType;
        [SerializeField] private Func<string> textGetter;
                
        void Start()
        {
            if (debugType == DebugType.Message)
            {
                if (duration <= 0) GameDebug.Log(textGetter.Invoke());
                else GameDebug.LogTemp(textGetter.Invoke(), duration);
            }
            else if (debugType == DebugType.Warning)
            {
                if (duration <= 0) GameDebug.LogWarning(textGetter.Invoke());
                else GameDebug.LogWarningTemp(textGetter.Invoke(), duration);
            }
            else if (debugType == DebugType.Error)
            {
                if (duration <= 0) GameDebug.LogError(textGetter.Invoke());
                else GameDebug.LogErrorTemp(textGetter.Invoke(), duration);
            }
        }

        public void SetText(int intValue)
        {
            
        }

        public void SetText(float floatValue)
        {
            
        }
        
        public void SetText(string stringValue)
        {
            
        }
        
        public void SetText(bool boolValue)
        {
            
        }
        
        public void SetText(Vector2 stringValue)
        {
            
        }
        
        public void SetText(Vector3 stringValue)
        {
            
        }
        
        public void SetText(Color colorValue)
        {
            
        }
        
        public void SetColor(Color color)
        {
            
        }
    }
}