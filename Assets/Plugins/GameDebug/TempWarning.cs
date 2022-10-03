using UnityEngine;

namespace DebugToScreen
{
    public class TempWarning: Warning
    {
        public string text;
        private float endTime;

        public TempWarning(string text, float duration): base(text)
        {
            this.text = text;
            endTime = Time.time + duration;
        }

        public void DrawSelf(Rect rect)
        {
            if (Time.time > endTime)
            {
                GameDebug.RemoveLog(this);
                return;
            }
            GUI.Label(rect, text, GameDebug.WarningStyle);
        }
    }
}