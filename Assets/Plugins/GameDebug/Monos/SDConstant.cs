using UnityEngine;

namespace DebugToScreen
{
    public class SDConstant : MonoBehaviour
    {
        [SerializeField, Tooltip("Duration -1 meaning this message will be there forever")]
        private float duration = -1f;

        [SerializeField] private DebugType debugType;
        [SerializeField] private int priority = 1;
        [SerializeField] private string text;

        void Start()
        {
            LogToScreen();
        }

        void LogToScreen()
        {
            if (debugType == DebugType.Message)
            {
                if (duration <= 0) GameDebug.Log(text, priority);
                else GameDebug.LogTemp(text, duration, priority);
            }
            else if (debugType == DebugType.Warning)
            {
                if (duration <= 0) GameDebug.LogWarning(text, priority);
                else GameDebug.LogWarningTemp(text, duration, priority);
            }
            else if (debugType == DebugType.Error)
            {
                if (duration <= 0) GameDebug.LogError(text, priority);
                else GameDebug.LogErrorTemp(text, duration, priority);
            }
        }
    }

    public enum DebugType
    {
        Message,
        Warning,
        Error
    }
}