using UnityEngine;

namespace KSRecs.Deprecated.BuildSystemSpace
{
    public class LiveProb : MonoBehaviour
    {
        [SerializeField, ReadOnly] private BuildProbSO parentProb;

        public BuildProbSO ParentProb
        {
            get => parentProb;
            set
            {
                if (parentProb == null)
                {
                    parentProb = value;
                }
                else
                {
                    Debug.LogWarning("Cannot change the parent prob of an object");
                }
            }
        }
    }
}