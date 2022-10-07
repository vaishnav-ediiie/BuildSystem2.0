using System;
using UnityEngine;

namespace DebugToScreen
{
    public interface IGameLog
    {
        public int Priority { get; set; }
        public float LinesCount { get;}
        public abstract void DrawSelf(Rect rect);
                
    }
}