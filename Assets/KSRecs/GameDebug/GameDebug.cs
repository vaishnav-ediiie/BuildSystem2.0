using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace DebugToScreen
{
    [DefaultExecutionOrder(-10)]
    public class GameDebug : MonoBehaviour, IComparer<Message>
    {
        private static GameDebug Instance;
        internal static readonly GUIStyle MessageStyle = new GUIStyle();
        internal static readonly GUIStyle ErrorStyle = new GUIStyle();
        internal static readonly GUIStyle WarningStyle = new GUIStyle();

        internal static readonly GUIStyle ObjectLogStyle = new GUIStyle();
        internal static readonly GUIStyle ObjectLogTitleStyleActive = new GUIStyle();
        internal static readonly GUIStyle ObjectLogTitleStyleInactive = new GUIStyle();
        private ObjectLog currentObjectLog;
        private List<ITracker> trackers;
        Rect rectOI;
        private List<Message> _allLogs;
        float height;
        float width;
        Rect rectNormal;
        private Vector2 scrollPos;
        [SerializeField] private bool acceptLogs = true;
        [SerializeField, Range(3, 100)] private int fontSize = 30;
        [SerializeField, Range(0f, 1f)] private float xOffset = 0f;
        [SerializeField, Range(0f, 1f)] private float yOffset = 0f;
        [SerializeField] private float logOffset = 4f;
        private float lineOffset = 4f;

        public int FontSize
        {
            get => fontSize;
            set
            {
                if (value <= 3) fontSize = 3;
                else fontSize = value;

                MessageStyle.fontSize = fontSize;
                ErrorStyle.fontSize = fontSize;
                WarningStyle.fontSize = fontSize;
                rectNormal.x = -xOffset;
                
                rectOI.x = xOffset;
                rectOI.height = fontSize;
                ObjectLogStyle.fontSize = fontSize;
                ObjectLogTitleStyleActive.fontSize = fontSize;
                ObjectLogTitleStyleInactive.fontSize = fontSize;
            }
        }

        public float XOffset
        {
            get => xOffset;
            set
            {
                if (value <= 0) xOffset = 0f;
                else if (value >= 1) xOffset = 1f;
                else xOffset = value;
                rectNormal = new Rect(-width * xOffset, height * yOffset, width, fontSize);
                rectOI = new Rect(width * xOffset, height * yOffset, width * 0.5f, fontSize);
            }
        }

        public float YOffset
        {
            get => YOffset;
            set
            {
                if (value <= 0) YOffset = 0;
                else if (value >= 0) YOffset = 1;
                else yOffset = value;
                rectNormal = new Rect(-width * xOffset, height * yOffset, width, fontSize);
                rectOI = new Rect(width * xOffset, height * yOffset, width * 0.5f, fontSize);
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                _allLogs = new List<Message>();
                DontDestroyOnLoad(gameObject);
                Init();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Init()
        {
            MessageStyle.normal.textColor = Color.white;
            ErrorStyle.normal.textColor = Color.red;
            WarningStyle.normal.textColor = Color.yellow;
            MessageStyle.fontSize = fontSize;
            ErrorStyle.fontSize = fontSize;
            WarningStyle.fontSize = fontSize;
            MessageStyle.alignment = TextAnchor.UpperRight;
            ErrorStyle.alignment = TextAnchor.UpperRight;
            WarningStyle.alignment = TextAnchor.UpperRight;
            height = Screen.height;
            width = Screen.width;
            rectNormal = new Rect(-width * xOffset, height * yOffset, width, fontSize);

            ObjectLogStyle.normal.textColor = Color.white;
            ObjectLogTitleStyleActive.normal.textColor = new Color(0.1f, 0.8f, 0.1f, 1f);
            ObjectLogTitleStyleInactive.normal.textColor = new Color(1, 1, 1, 0.5f);
            ObjectLogStyle.fontSize = fontSize;
            ObjectLogTitleStyleActive.fontSize = fontSize;
            ObjectLogTitleStyleInactive.fontSize = fontSize;
            ObjectLogTitleStyleActive.alignment = TextAnchor.UpperLeft;
            ObjectLogTitleStyleInactive.alignment = TextAnchor.UpperLeft;
            ObjectLogTitleStyleActive.fontStyle = FontStyle.Bold;
            ObjectLogTitleStyleInactive.fontStyle = FontStyle.Bold;
            rectOI = new Rect(width * xOffset, height * yOffset, width * 0.5f, fontSize);

            trackers = new List<ITracker>();
        }

        public int Compare(Message x, Message y) => x.Priority.CompareTo(y.Priority);

        internal static void RemoveLog(Message gameLog)
        {
            if (Instance._allLogs.Contains(gameLog))
            {
                Instance._allLogs.Remove(gameLog);
            }
        }

        public static void ClearLogs()
        {
            Instance._allLogs.Clear();
        }

        private void AddToLogStack(Message log)
        {
            this._allLogs.Add(log);
            this._allLogs.Sort(this);
        }

        public static Message Log(string message)
        {
            Message r = new Message(message);
            Instance.AddToLogStack(r);
            return r;
        }

        public static Message Log(string message, int priority)
        {
            Message r = new Message(message);
            r.Priority = priority;
            Instance.AddToLogStack(r);
            return r;
        }

        public static void StartInfo(string title, bool isExpanded = false)
        {
            Instance.currentObjectLog = new ObjectLog(title, isExpanded);
        }

        public static void UpdateInfo(string message)
        {
            if (Instance.currentObjectLog == null) Instance.currentObjectLog = new ObjectLog("New Log", true);
            Instance.currentObjectLog.Text = message;
        }

        public static void AppendText(string message)
        {
            if (!Instance.acceptLogs) return;
            if (Instance.currentObjectLog == null) Instance.currentObjectLog = new ObjectLog("New Log", true);
            Instance.currentObjectLog.AddText(message);
        }

        public static void AppendLine(string message)
        {
            if (!Instance.acceptLogs) return;
            if (Instance.currentObjectLog == null) Instance.currentObjectLog = new ObjectLog("New Log", true);
            Instance.currentObjectLog.AddText($"\n{message}");
        }

        public static void ClearInfo()
        {
            if (Instance.currentObjectLog == null) Instance.currentObjectLog = new ObjectLog("New Log", true);
            Instance.currentObjectLog.Text = "";
        }

        public static void StopInfo()
        {
            Instance.currentObjectLog = null;
        }

        public static void TrackField(object target, string fieldName)
        {
            FieldInfo fieldInfo = FirstFieldByName(target, fieldName);
            if (fieldInfo != null)
            {
                Instance.trackers.Add(new FieldTracker(fieldInfo, target));
            }
        }
        
        public static void TrackProperty(object target, string propertyName)
        {
            PropertyInfo propInfo = FirstPropByName(target, propertyName);
            if (propInfo != null)
            {
                Instance.trackers.Add(new PropertyTracker(propInfo, target));
            }
        }
        

        void OnGUI()
        {
            #if UNITY_EDITOR
            Init();
            #endif

            rectNormal.y = width * yOffset;
            foreach (Message message in _allLogs)
            {
                message.DrawSelf(rectNormal);
                rectNormal.y += (fontSize + lineOffset) * message.LinesCount + logOffset;
                if (rectNormal.y >= height) return;
            }
            
            if (currentObjectLog != null)
            {
                // rectOI.y = height * yOffset;
                // rectOI.width = Screen.width;
                // Rect viewRect = new Rect(rectOI)  { height = (fontSize + lineOffset + 10) * currentObjectLog.LinesCount };
                // Rect posRect = new Rect(viewRect) { height = viewRect.height * 0.5f /*Mathf.Min(, height - rectOI.y)*/};
                
                // scrollPos = GUI.BeginScrollView(posRect, scrollPos, viewRect, alwaysShowVertical: true, alwaysShowHorizontal: false, horizontalScrollbar: GUIStyle.none, verticalScrollbar: GUI.skin.verticalScrollbar);
                
                if (currentObjectLog.IsExpanded)
                {
                    currentObjectLog.IsExpanded = GUI.Toggle(rectOI, currentObjectLog.IsExpanded, currentObjectLog.Title, ObjectLogTitleStyleActive);
                    rectOI.x += fontSize;
                    rectOI.y += fontSize + lineOffset;
                    currentObjectLog.DrawSelf(rectOI);
                    rectOI.y += (fontSize + lineOffset + 1) * currentObjectLog.LinesCount + logOffset;
                    rectOI.x -= fontSize;
                }
                else
                {
                    currentObjectLog.IsExpanded = GUI.Toggle(rectOI, currentObjectLog.IsExpanded, currentObjectLog.Title, ObjectLogTitleStyleInactive);
                    rectOI.y += fontSize + logOffset + lineOffset;
                }
                // GUI.EndScrollView();
            }
            
            /*if (trackers.Count != 0)
            {
                rectOI.height = (fontSize + lineOffset) * trackers.Count + lineOffset;
                scrollPos = GUI.BeginScrollView(rectOI, scrollPos, rectOI);
                rectOI.height = fontSize;
                rectOI.width = width;
                
                foreach (ITracker tracker in trackers)
                {
                    tracker.DrawSelf(rectOI);
                    rectOI.y += fontSize + lineOffset + logOffset;;
                }
                GUI.EndScrollView();
            }*/
            
        }

        private static FieldInfo FirstFieldByName(object target, string name)
        {
            foreach (var info in target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic |
                                                            BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Default))
            {
                if (info.Name == name) return info;
            }
            return null;
        }

        private static PropertyInfo FirstPropByName(object target, string name)
        {
            foreach (var info in target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic |
                                                                BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Default))
            {
                if (info.Name == name) return info;
            }

            return null;
        }
    }
}