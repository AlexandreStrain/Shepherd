using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shepherd;

[System.Serializable]
public class CharacterBody {
	[Header("General Wellness")]
	public StatusBar _health;
	public StatusBar _stamina;
	public StatusBar _courage;
	public StatusBar _poise;
	public StatusBar _immuneSystem;
	public StatusBar _carryWeight;

	[Header("Body Needs")]
	public StatusBar _thirst;
	public StatusBar _hunger;
	public StatusBar _sleep;
	public StatusBar _waste;
	public StatusBar _pleasure;
	public PriorityQueue<StatusBar> _myNeeds = new PriorityQueue<StatusBar>(); //possible rename

    [Header("Sensory Information")]
    public bool _underDuress;
	public bool _sensesActive;
	//what is deemed worthy of attention (target) and what can be ignored (obstacle)
	public LayerMask _targetMask;
	public LayerMask _obstacleMask;
	[HideInInspector]
	public Transform _bodyTransform;

	[Range (0f, 360f)]
	private float _sightAngle; //could be measured by perception
	private float _senseRadius; //could be measured by perception
	private float _senseDelay = 0.2f; //could be measured by perception
	private float _currentDelay;

	private List<Transform> _visibleObjects = new List<Transform> ();
	private List<Transform> _audibleObjects = new List<Transform> ();
    private List<Transform> _lockOnObjects = new List<Transform>();

    public void Init(WorldItemStats stats, Transform parent) {
		_myNeeds.Reset ();

		float maxHealth = 100f; //temp
		if (stats._vitality != 0) {
			maxHealth = 100f * stats._vitality;
		}
		_health.SetMaximum (maxHealth);
		_health.ResetToMaximum ();

		float maxStamina = 100f; //temp
		if (stats._endurance != 0) {
			maxStamina = 100f * stats._endurance;
		}
		_stamina.SetMaximum (maxStamina);
		_stamina.ResetToMaximum ();

		float maxCourage = 100f; //temp
		if (stats._courage != 0) {
			maxCourage = 100f * stats._courage;
		}
		_courage.SetMaximum (maxCourage);
		_courage.ResetToMaximum ();

		float maxPoise = 100f; //temp
		if (stats._endurance != 0) {
			maxPoise = 100f * stats._endurance;
		}
		_poise.SetMaximum (maxPoise);
		_poise.ResetToMinimum ();

		float maxImmuneSystem = 100f; //temp
		if (stats._vitality != 0) {
			maxImmuneSystem = 100f * stats._vitality;
		}
		_immuneSystem.SetMaximum (maxImmuneSystem);
		_immuneSystem.ResetToMaximum ();

		float maxCarryWeight = 50f; //temp
		if (stats._endurance != 0) {
            maxCarryWeight = 50f * stats._endurance;
		}
		_carryWeight.SetMaximum(maxCarryWeight);
		_carryWeight.ResetToMinimum ();

		_hunger.SetStatusType (AllEnums.BodyNeedsType.Hunger);
		_hunger.ResetToMaximum ();

		_thirst.SetStatusType (AllEnums.BodyNeedsType.Thirst);
		_thirst.ResetToMaximum ();

		_sleep.SetStatusType (AllEnums.BodyNeedsType.Sleep);
		_sleep.ResetToMaximum ();

		_waste.SetStatusType (AllEnums.BodyNeedsType.Waste);
		_waste.ResetToMinimum ();

		_pleasure.SetStatusType (AllEnums.BodyNeedsType.Pleasure);
		_pleasure.ResetToMaximum ();

		//test
		_bodyTransform = parent;

		if (stats._perception == 0) {
			//blind
			_sightAngle = 1f;
			_senseRadius = 5f; //for now.. so they aren't deaf too!
			_senseDelay = 0.5f; //for now.. so they still hear pretty well
		} else {
			_sightAngle = 36f * Mathf.Clamp(stats._perception, 0f, 10f); //HARDCODED, but should total 360 degree vision if perception is 10 
			_senseRadius = (0.75f * (Mathf.Clamp(stats._perception, 0f, 10f) * Mathf.Clamp(stats._perception, 0f, 10f))) + (2 * Mathf.Clamp(stats._perception, 0f, 10f)) + 5f; //No more than 100 m away -- otherwise too taxing to system?
			_senseDelay = 1f / Mathf.Clamp(stats._perception, 0.01f, 10f) + 0.1f; 
		}
		_sensesActive = true;
		_currentDelay = 0f;
		_visibleObjects.Clear ();
		_audibleObjects.Clear ();
        _lockOnObjects.Clear();
		//...
	}

    public void InitFromStats(WorldItemStats stats)
    {
        _health.ResetToMinimum();
        _health.Add(stats._health);

        _stamina.ResetToMinimum();
        _stamina.Add(stats._stamina);

        _courage.ResetToMinimum();
        _courage.Add(stats._baseCourage);

        _poise.ResetToMinimum();
        _poise.Add(stats._poise);

        _immuneSystem.ResetToMinimum();
        _immuneSystem.Add(stats._immuneSystem);

        _carryWeight.ResetToMinimum();
        _carryWeight.Add(stats._carryWeight);

        _hunger.ResetToMinimum();
        _hunger.Add(stats._hunger);

        _thirst.ResetToMinimum();
        _thirst.Add(stats._thirst);

        _sleep.ResetToMinimum();
        _sleep.Add(stats._sleep);

        _waste.ResetToMinimum();
        _waste.Add(stats._waste);

        _pleasure.ResetToMinimum();
        _pleasure.Add(stats._pleasure);
    }

	public List<Transform> GetVisibleObjects() {
		return this._visibleObjects;
	}

	public List<Transform> GetAudibleObjects() {
		return this._audibleObjects;
	}

    public List<Transform> GetLockOnObjects()
    {
        return this._lockOnObjects;
    }

    /*NOT SURE IF NEEDED*/
    public float GetSenseRadius() {
		return _senseRadius;
	}
	public float GetSightAngle() {
		return _sightAngle;
	}
	public float GetSenseDelay() {
		return _senseDelay;
	}

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal){
		if (!angleIsGlobal) {
			angleInDegrees += _bodyTransform.eulerAngles.y;
		}
		return new Vector3 (Mathf.Sin (angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos (angleInDegrees * Mathf.Deg2Rad));
	}

	public void Tick(float delta) {
		if (_sensesActive) {
			if (_currentDelay >= _senseDelay) {
				SenseSurroundings ();
				_currentDelay = 0f;
			} else {
				_currentDelay += delta;
			}
		}
        CheckVitals();
	}

	private void SenseSurroundings() {
		_visibleObjects.Clear ();
		_audibleObjects.Clear ();
        _lockOnObjects.Clear ();
		Collider[] targetsInViewRadius = Physics.OverlapSphere (_bodyTransform.position, _senseRadius, _targetMask);
		foreach (Collider target in targetsInViewRadius) {
			if (target.transform == _bodyTransform) {
				continue; //we hear/see ourselves, which doesn't matter
			}
			_audibleObjects.Add (target.transform);

            if(target.gameObject.layer == LayerMask.NameToLayer("Controllers"))
            {
                _lockOnObjects.Add(target.transform);
            }

			Vector3 dirToTarget = (target.transform.position - _bodyTransform.position).normalized;
			if (Vector3.Angle (_bodyTransform.forward, dirToTarget) < _sightAngle / 2f) {
				float disToTarget = Vector3.Distance (_bodyTransform.position, target.transform.position);
				if (!Physics.Raycast (_bodyTransform.position, dirToTarget, disToTarget, _obstacleMask)) {
					_visibleObjects.Add (target.transform);
				}
			}
		}
	}

    private void CheckVitals()
    {
        if (_health.GetMeter() <= 0)
        {
            _health.ResetToMinimum(); //not sure if needed, we check as we clamp between min and max on status bar
        }

        if (_underDuress || _myNeeds.Peek() != null)
        {
            _courage.Decay();
        }
        else
        {
            _courage.Regen();
        }

        if (_immuneSystem.GetMeter() < _immuneSystem.GetMaximum())
        {
            _immuneSystem.Regen();
        }
        else
        {
            _health.Regen();
        }
        
        _thirst.Decay();
        _waste.Regen();
        _hunger.Decay();
        _sleep.Decay();
        _pleasure.Decay();
    }
}

[System.Serializable]
public class StatusBar : IHeapItem<StatusBar> {
	private const float _minimum = 0f;
	private float _maximum = 255f; //doing so because it is like the alpha of the image

	[SerializeField] //TODO: temporary, just for editing purposes
	private float _current;
	private bool _empty = false;

	public float _decay;
	public float _regen;

	public AllEnums.BodyNeedsType _type; //temp?
	private int _heapIndex; //temp?

	public int HeapIndex {
		get {
			return _heapIndex;
		}
		set {
			this._heapIndex = value;
		}
	}

	public int CompareTo(StatusBar need) {
		//negative value means object comes before this in the sort order
		//positive value means object comes after this in sort order
		if (this._type < need._type) {
			if (this.GetMeter() > need.GetMeter()) {
				return 1;
			} else {
				return -1;
			}
		}

		if (this._type > need._type) {
			if (this.GetMeter() > need.GetMeter()) {
				return -1;
			} else {
				return 1;
			}
		}

		return 0;
	}

	public void SetStatusType(AllEnums.BodyNeedsType type) {
		_type = type;
	}
	public AllEnums.BodyNeedsType GetStatusType() {
		return this._type;
	}
	public float GetMeter() {
		return _current;
	}
	public void Add(float amount) {
		_current = Mathf.Clamp (_current + amount, _minimum, _maximum);
		if (_current == _maximum) {
			_empty = false;
		}
	}
	public void Subtract(float amount) {
		_current = Mathf.Clamp (_current - amount, _minimum, _maximum);
		if (_current == _minimum) {
			_empty = true;
		}
	}
	public void Decay() {
		_current = Mathf.Clamp (_current - _decay, _minimum, _maximum);
		if (_current == _minimum) {
			_empty = true;
		}
	}
	public void Regen() {
		_current = Mathf.Clamp (_current + _regen, _minimum, _maximum);
		if (_current == _maximum) {
			_empty = false;
		}
	}
	public float GetMaximum() {
		return _maximum;
	}
	public void SetMaximum(float newMax) {
		if (newMax > _minimum) {
			_maximum = newMax;
		}
	}
	public void ResetToMinimum() {
		_current = _minimum;
	}
	public void ResetToMaximum() {
		_current = _maximum;
	}

	public bool IsEmpty() {
		return _empty;
	}
}