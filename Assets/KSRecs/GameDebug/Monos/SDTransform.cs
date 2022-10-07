using System;
using UnityEngine;

namespace DebugToScreen
{
    public class SDTransform : MonoBehaviour
    {
        private enum Property
        {
            localPosition,
            localRotation,
            localScale,
            position,
            rotation,
            lossyScale
        }

        [SerializeField] private Property property;
        [SerializeField] private string prefix;
        [SerializeField] private string suffix;
        private Message _message;
        private Func<string> GetterFunc;

        void Start()
        {
            switch (property)
            {
                case (Property.localPosition):
                {
                    GetterFunc = () => transform.localPosition.ToString();
                    break;
                }
                case (Property.localRotation):
                {
                    GetterFunc = () => transform.localRotation.ToString();
                    break;
                }
                case (Property.localScale):
                {
                    GetterFunc = () => transform.localScale.ToString();
                    break;
                }
                case (Property.position):
                {
                    GetterFunc = () => transform.position.ToString();
                    break;
                }
                case (Property.rotation):
                {
                    GetterFunc = () => transform.rotation.ToString();
                    break;
                }
                case (Property.lossyScale):
                {
                    GetterFunc = () => transform.lossyScale.ToString();
                    break;
                }
            }
            _message = GameDebug.Log($"{prefix}{GetterFunc()}{suffix}");
        }

        void Update()
        {
            _message.Text = $"{prefix}{GetterFunc()}{suffix}";
        }
    }
}