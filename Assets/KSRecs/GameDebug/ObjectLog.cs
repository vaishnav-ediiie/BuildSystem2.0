using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Object = System.Object;

namespace DebugToScreen
{
    public interface ITracker
    {
        public void DrawSelf(Rect rect);
    } 
    
    public class ObjectLog : ITracker
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
                if (getProps) linesCount += properties.Count + fields.Count + 1;
                if (getConts) linesCount += constantTrackers.Count + 1;
            }
        }

        public float LinesCount => linesCount;
        private List<FieldInfo> fields;
        private List<PropertyInfo> properties;
        private List<string> constantTrackers;

        private bool getConts;
        private bool getProps;

        public ObjectLog(string title, bool isExpanded)
        {
            IsExpanded = isExpanded;
            getConts = false;
            getProps = false;


            this.Title = title;
            this.Text = "";
            fields = new List<FieldInfo>();
            properties = new List<PropertyInfo>();
            constantTrackers = new List<string>();
        }

        public void AddText(string value)
        {
            this.text = this.text + value.Replace("\n", "\n    ");
            linesCount += value.Count(c => c.Equals('\n'));
        }

        public void DrawSelf(Rect rect)
        {
            GUI.Label(rect, $"{Title}\n{Text}", GameDebug.ObjectLogStyle);
        }
    }
    
    public class FieldTracker : ITracker
    {
        object theTarget;
        FieldInfo fieldInfo;

        public FieldTracker(FieldInfo fieldInfo, object theTarget)
        {
            this.fieldInfo = fieldInfo;
            this.theTarget = theTarget;
        }

        public void DrawSelf(Rect rect)
        {
            GUI.Label(rect, fieldInfo.GetValue(theTarget).ToString(), GameDebug.ObjectLogStyle);
        }
    }
    
    public class PropertyTracker : ITracker
    {
        object theTarget;
        PropertyInfo propertyInfo;

        public PropertyTracker(PropertyInfo propertyInfo, object theTarget)
        {
            this.propertyInfo = propertyInfo;
            this.theTarget = theTarget;
        }

        public void DrawSelf(Rect rect)
        {
            GUI.Label(rect, propertyInfo.GetValue(theTarget).ToString(), GameDebug.ObjectLogStyle);
        }
    }
}
