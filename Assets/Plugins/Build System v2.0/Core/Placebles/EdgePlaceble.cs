using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomBuildSystem
{
    public class EdgePlaceble : MonoBehaviour, IPlaceble
    {
        public IPlaceble Main;
        public List<IPlaceble> Decorators;
    }
}