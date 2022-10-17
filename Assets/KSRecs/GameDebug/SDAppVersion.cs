using UnityEngine;

namespace DebugToScreen
{
    public class SDAppVersion : MonoBehaviour
    {
        [SerializeField] private string prefix;

        void Start()
        {
            GameDebug.Log($"{prefix}{Application.version}", -1);
        }
    }
}