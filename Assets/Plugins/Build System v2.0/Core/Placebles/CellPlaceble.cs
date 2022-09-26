using System;
using UnityEngine;

namespace CustomBuildSystem
{
    [Serializable]
    public class CellPlaceble : MonoBehaviour, IPlaceble
    {
        public IPlaceble Main;
    }
}