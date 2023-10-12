using System.Collections.Generic;
using UnityEngine;

namespace Shepherd
{
    [System.Serializable]
    public class AIPattern
    {
        public bool _fullReset;
        [Header("Stationary")]
        public float _idleTimer;


        [Header("Flock")]
        public Transform _carrot;
        public int _flockCount; //random amount of time to flock, remain still...
        public int _flockTimer;
        public bool _isRoaming; //toggle roaming/flocking
        public bool _isTurning; //temp... adds variety to roaming
        public Vector3 _targetRotated; //temp... random vector3 to turn towards while roaming

        public float _neighbourCling;
        public float _personalSpace;
        public Banner _myBanner;

        [Header("Waypoint")]
        public Waypoint[] _waypoints;
        public int _curWaypointIndex;
        public bool _patrolWaypoints;

        [Header("BodyNeeds")]
        public bool _hasTask; //related to BodyNeeds
        public bool _atTask; //related to BodyNeeds
        public AllEnums.BodyNeedsType _taskType;
        public Dictionary<AllEnums.BodyNeedsType, List<Vector3>> _poi = new Dictionary<AllEnums.BodyNeedsType, List<Vector3>>();

        [Header("Roam")]
        public float _roamCount;


        public void Init()
        {
            //test
            _poi.Clear();

            _poi.Add(AllEnums.BodyNeedsType.Hunger, new List<Vector3>());
            _poi.Add(AllEnums.BodyNeedsType.Thirst, new List<Vector3>());
            _poi.Add(AllEnums.BodyNeedsType.Sleep, new List<Vector3>());
            _poi.Add(AllEnums.BodyNeedsType.Waste, new List<Vector3>());
            _poi.Add(AllEnums.BodyNeedsType.Pleasure, new List<Vector3>());
            //...

            for(int i = 0; i < _waypoints.Length; i++)
            {
                _waypoints[i].canDepart = false;
                _waypoints[i].arrived = false;
            }
            _curWaypointIndex = 0;
        }
    }
}
