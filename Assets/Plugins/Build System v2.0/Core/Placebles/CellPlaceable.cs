using System;
using UnityEngine;

namespace CustomBuildSystem
{
    [Serializable]
    public class CellPlaceable : MonoBehaviour, IPlaceable
    {
        public IPlaceable Main;
    }
}