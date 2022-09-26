using System;
using System.Collections.Generic;
using UnityEngine;


namespace KSRecs.Deprecated.BuildSystemSpace
{
    public class BuildSystemInputHandler : MonoBehaviour
    {
        public static BuildSystemInputHandler Instance { get; private set; }


        [SerializeField] private List<KeyBinding> bindings;
        private bool isBuilding = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            var obj = FindObjectOfType<BuildSystem>();
            if (obj == null || !obj.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("No Active Build System Found, Destroying Input Handler");
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }


        private void OnEnable() => BuildProbSO.OnBuildOver += StartUpdate;
        private void OnDisable() => BuildProbSO.OnBuildOver -= StartUpdate;
        private void StartUpdate(BuildProbSO probSo) => isBuilding = false;

        private void Update()
        {
            if (isBuilding) return;

            foreach (KeyBinding bind in bindings)
            {
                if (Input.GetKeyDown(bind.Shorcut))
                {
                    BuildSystem.Instance.StartProbSetup(bind.Prob);
                    isBuilding = true;
                }
            }
        }
    }


    [Serializable]
    public struct KeyBinding
    {
        public BuildProbSO Prob;
        public KeyCode Shorcut;
    }
}