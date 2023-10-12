using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd
{
    [CreateAssetMenu(menuName = "Shepherd/World Objects/Waypoint")]
    public class Waypoint : ScriptableObject
    {
        public Vector3 coord;
        public bool arrived;
        public bool waitToDepart;
        public bool canDepart;
    }
}
