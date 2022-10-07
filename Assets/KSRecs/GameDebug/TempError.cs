using UnityEngine;

namespace DebugToScreen
{
    public class TempError: Error
    {
        public string text;
        private float endTime;

        public TempError(string text, float duration): base(text)
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
            GUI.Label(rect, text, GameDebug.ErrorStyle);
        }
    }
}