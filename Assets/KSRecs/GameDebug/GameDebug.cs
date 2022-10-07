using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace DebugToScreen
{
    [DefaultExecutionOrder(-10)]
    public class GameDebug : MonoBehaviour, IComparer<IGameLog>
    {
        public static GameDebug Instance { get; private set; }
        internal static readonly GUIStyle MessageStyle = new GUIStyle();
        internal static readonly GUIStyle ErrorStyle = new GUIStyle();
        internal static readonly GUIStyle WarningStyle = new GUIStyle();

        internal static readonly GUIStyle ObjectLogStyle = new GUIStyle();
        internal static readonly GUIStyle ObjectLogTitleStyleActive = new GUIStyle();
        internal static readonly GUIStyle ObjectLogTitleStyleInactive = new GUIStyle();

        private List<IGameLog> _allLogs;
        private Dictionary<object, ObjectLog> objectLogs;
        float height;
        float width;
        Rect rectNormal;
        Rect rectOI;


        [SerializeField, Range(3, 100)] private int fontSize = 30;
        [SerializeField, Range(0f, 1f)] private float xOffset = 0f;
        [SerializeField, Range(0f, 1f)] private float yOffset = 0f;
        [SerializeField] private float logOffset = 4f;
        [SerializeField] private Color messageColor;
        private float lineOffset = 4f;
        public bool acceptInput;

        public int FontSize
        {
            get => fontSize;
            set
            {
                if (value <= 3) fontSize = 3;
                else fontSize = value;

                MessageStyle.fontSize = fontSize;
                ObjectLogStyle.fontSize = fontSize;
                ObjectLogTitleStyleActive.fontSize = fontSize;
                ObjectLogTitleStyleInactive.fontSize = fontSize;
                ErrorStyle.fontSize = fontSize;
                WarningStyle.fontSize = fontSize;
                rectNormal.x = -xOffset;
                rectOI.x = xOffset;
                rectOI.height = fontSize;
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
                _allLogs = new List<IGameLog>();
                objectLogs = new Dictionary<object, ObjectLog>();
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
            MessageStyle.normal.textColor = messageColor;
            ObjectLogStyle.normal.textColor = messageColor;
            ObjectLogTitleStyleActive.normal.textColor = new Color(0.1f, 0.8f, 0.1f, 1f);
            ObjectLogTitleStyleInactive.normal.textColor = messageColor;
            ErrorStyle.normal.textColor = Color.red;
            WarningStyle.normal.textColor = Color.yellow;

            MessageStyle.fontSize = fontSize;
            ObjectLogStyle.fontSize = fontSize;
            ObjectLogTitleStyleActive.fontSize = fontSize;
            ObjectLogTitleStyleInactive.fontSize = fontSize;
            ErrorStyle.fontSize = fontSize;
            WarningStyle.fontSize = fontSize;

            MessageStyle.alignment = TextAnchor.UpperRight;
            ObjectLogTitleStyleActive.alignment = TextAnchor.UpperLeft;
            ObjectLogTitleStyleInactive.alignment = TextAnchor.UpperLeft;
            ErrorStyle.alignment = TextAnchor.UpperRight;
            WarningStyle.alignment = TextAnchor.UpperRight;

            ObjectLogTitleStyleActive.fontStyle = FontStyle.Bold;
            ObjectLogTitleStyleInactive.fontStyle = FontStyle.Bold;


            height = Screen.height;
            width = Screen.width;
            rectNormal = new Rect(-width * xOffset, height * yOffset, width, fontSize);
            rectOI = new Rect(width * xOffset, height * yOffset, width * 0.5f, fontSize);
        }

        public int Compare(IGameLog x, IGameLog y) => x.Priority.CompareTo(y.Priority);

        internal static void RemoveLog(IGameLog gameLog)
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

        private void AddToLogStack(IGameLog log)
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

        public static Warning LogWarning(string message)
        {
            Warning r = new Warning(message);
            Instance.AddToLogStack(r);
            return r;
        }

        public static Error LogError(string message)
        {
            Error r = new Error(message);
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

        public static Warning LogWarning(string message, int priority)
        {
            Warning r = new Warning(message);
            r.Priority = priority;
            Instance.AddToLogStack(r);
            return r;
        }

        public static Error LogError(string message, int priority)
        {
            Error r = new Error(message);
            r.Priority = priority;
            Instance.AddToLogStack(r);
            return r;
        }

        public static Message LogTemp(string message, float duration)
        {
            TempMessage r = new TempMessage(message, duration);
            Instance.AddToLogStack(r);
            return r;
        }

        public static Warning LogWarningTemp(string message, float duration)
        {
            TempWarning r = new TempWarning(message, duration);
            Instance.AddToLogStack(r);
            return r;
        }

        public static Error LogErrorTemp(string message, float duration)
        {
            TempError r = new TempError(message, duration);
            Instance.AddToLogStack(r);
            return r;
        }

        public static Message LogTemp(string message, float duration, int priority)
        {
            TempMessage r = new TempMessage(message, duration);
            r.Priority = priority;
            Instance.AddToLogStack(r);
            return r;
        }

        public static Warning LogWarningTemp(string message, float duration, int priority)
        {
            TempWarning r = new TempWarning(message, duration);
            r.Priority = priority;
            Instance.AddToLogStack(r);
            return r;
        }

        public static Error LogErrorTemp(string message, float duration, int priority)
        {
            TempError r = new TempError(message, duration);
            r.Priority = priority;
            Instance.AddToLogStack(r);
            return r;
        }

        public static void StartMyInfo(object caller, string title = "", bool isExpanded = false)
        {
            ObjectLog r;
            if (string.IsNullOrEmpty(title)) r = new ObjectLog(caller.GetType().Name, caller, isExpanded);
            else r = new ObjectLog(title, caller, isExpanded);
            Instance.objectLogs.Add(caller, r);
        }

        public static void TrackMyField(object caller, string fieldName, bool isConstant)
        {
            FieldInfo fi = FirstFieldByName(caller, fieldName);
            if (fi != null) Instance.objectLogs[caller].Track(fi, isConstant);
            else Debug.LogError($"Cannot find Field with name {fieldName}");
        }

        public static void TrackMyProperty(object caller, string fieldName, bool isConstant)
        {
            PropertyInfo fi = FirstPropByName(caller, fieldName);
            if (fi != null) Instance.objectLogs[caller].Track(fi, isConstant);
            else Debug.LogError($"Cannot find Field with name {fieldName}");
        }

        public static void UpdateMyInfo(object caller, string message)
        {
            if (Instance.acceptInput)
                Instance.objectLogs[caller].Text = message;
        }

        public static void AddToMyInfo(object caller, string message)
        {
            if (Instance.acceptInput)
                Instance.objectLogs[caller].AddText(message);
        }

        public static void AddLineToMyInfo(object caller, string message)
        {
            if (Instance.acceptInput)
                Instance.objectLogs[caller].AddText($"\n{message}");
        }

        public static void ClearMyInfo(object caller)
        {
            if (Instance.acceptInput)
                Instance.objectLogs[caller].Text = "";
        }

        void OnGUI()
        {
            #if UNITY_EDITOR
            Init();
            #endif

            rectNormal.y = width * yOffset;
            rectOI.y = width * yOffset;

            foreach (IGameLog message in _allLogs)
            {
                message.DrawSelf(rectNormal);
                rectNormal.y += (fontSize + lineOffset) * message.LinesCount + logOffset;
                if (rectNormal.y >= height) return;
            }


            foreach (KeyValuePair<object, ObjectLog> log in objectLogs)
            {
                if (log.Value.IsExpanded)
                {
                    log.Value.IsExpanded = GUI.Toggle(rectOI, log.Value.IsExpanded, log.Value.Title, ObjectLogTitleStyleActive);
                    rectOI.x += fontSize;
                    rectOI.y += fontSize + lineOffset;
                    log.Value.DrawSelf(rectOI);
                    rectOI.y += (fontSize + lineOffset + 1) * log.Value.LinesCount + logOffset;
                    rectOI.x -= fontSize;
                }
                else
                {
                    log.Value.IsExpanded = GUI.Toggle(rectOI, log.Value.IsExpanded, log.Value.Title, ObjectLogTitleStyleInactive);
                    rectOI.y += fontSize + logOffset + lineOffset;
                }

                if (rectOI.y >= height) return;
            }
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