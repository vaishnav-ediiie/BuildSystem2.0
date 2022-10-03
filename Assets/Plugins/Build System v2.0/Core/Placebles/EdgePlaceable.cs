using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomBuildSystem
{
    public class EdgePlaceable : MonoBehaviour, IPlaceable
    {
        public IPlaceable Main;
        public List<IPlaceable> Decorators;
    }
}