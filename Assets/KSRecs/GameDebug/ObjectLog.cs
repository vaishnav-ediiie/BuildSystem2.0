using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Object = System.Object;

namespace DebugToScreen
{
    public class ObjectLog : IGameLog
    {
        private string text;
        private int linesCount;
        public bool IsExpanded;
        public string Title;

        public string Text
        {
            get => text;
            set
            {
                this.text = "    " + value.Replace("\n", "\n    ");
                linesCount = text.Count(c => c.Equals('\n')) + 2;
                if (getProps) linesCount += properties.Count + fields.Count+1;
                if (getConts) linesCount += constantTrackers.Count+1;
                getText = !string.IsNullOrEmpty(value);
                if (getText) linesCount++;
            }
        }

        object theTarget;
        public int Priority { get; set; }
        public float LinesCount => linesCount;
        private List<FieldInfo> fields;
        private List<PropertyInfo> properties;
        private List<string> constantTrackers;

        private bool getConts;
        private bool getProps;
        private bool getText;
        
        public ObjectLog(string title, object theTarget, bool isExpanded)
        {
            IsExpanded = isExpanded;
            getConts = false;
            getProps = false;
            getText = false;
            
            
            this.Title = title;
            this.Text = "";
            this.theTarget = theTarget;
            fields = new List<FieldInfo>();
            properties = new List<PropertyInfo>();
            constantTrackers = new List<string>();
        }

        public void Track(FieldInfo obj, bool isConstant)
        {
            if (isConstant)
            {
                constantTrackers.Add($"    {obj.Name}: {obj.GetValue(theTarget)}");
                getConts = true;
            }
            else
            {
                fields.Add(obj);
                getProps = true;
            }
            linesCount++;
            
        }
        
        public void Track(PropertyInfo obj, bool isConstant)
        {
            if (isConstant)
            {
                constantTrackers.Add($"    {obj.Name}: {obj.GetValue(theTarget)}");
                getConts = true;
            }
            else
            {
                properties.Add(obj);
                getProps = true;
            }
            linesCount++;
        }

        private string GetInfo()
        {
            StringBuilder builder = new StringBuilder();
            if (getConts)
            {
                builder.AppendLine("Constants:");
                foreach (string tracker in constantTrackers) builder.AppendLine(tracker);
            }

            if (getProps)
            {
                builder.AppendLine("Properties:");
                foreach (PropertyInfo o in properties) builder.AppendLine($"    {o.Name}: {o.GetValue(theTarget)}");
                foreach (FieldInfo o in fields) builder.AppendLine($"    {o.Name}: {o.GetValue(theTarget)}");
            }

            if (getText)
            {
                builder.AppendLine($"Texts:\n{text}");
            }
            return builder.ToString();
        }

        public void AddText(string value)
        {
            this.text = this.text + value.Replace("\n", "\n    ");
            linesCount += value.Count(c => c.Equals('\n'));
        }
        
        public void DrawSelf(Rect rect)
        {
            GUI.Label(rect, GetInfo(), GameDebug.ObjectLogStyle);
        }
    }
}