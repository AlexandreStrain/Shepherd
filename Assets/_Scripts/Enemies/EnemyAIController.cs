using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Shepherd;

public class EnemyAIController : InputModule {
	public EnemyAIAttacks[] _enemyAIAttacks;

	public AllEnums.EnemyAIState _aiSightState;
    public AllEnums.NpcState _currentState;


	public GameObject _myUIPrefab;
	private GameObject _myUI;
	public GameEvent _myGameEvent;
	public DelegateCalls.FirstSight _firstSight;

	public int _proximityCount;
	private int _proximityTimer;
	public int _attackCount = 30;
	private int _attackTimer;

	[Header("NavMesh Agent")]
	public NavMeshAgent _agent;
	public Vector3 _targetDestination;
	public SpawnPoint _home; //this will transfer (instantiate) from a spawner in future, and passed into each ai active in level or scene
    public Transform _carrot;

    private bool reset;
	private float _delta;

	//public float _attackRange; //might be useful for keeping target in range
	//public int _frameCount;
	//private int _frame;

	private float _distance;
	private float _angle;

    //test for pause
    private float _pHorizontal;
    private float _pVertical;

	private WorldResourceController _wRControl;
	private bool _isDrop;
	private CharacterModel _testing;

    [Header("Delete below if don't work")]
    public bool _enablePickup;
    public bool _receivedPickup;
    
	public NPCBehaviour _aiBehaviour;
    [SerializeField]
    public AIPattern _aiPattern;
    //TODO: MIght move AI pattern in here
    public AllEnums.NpcAiPattern _curPattern;
    public AllEnums.NpcAiPattern _prevPattern;

public void Init() {
		_wRControl = Resources.Load ("WorldResourceControl") as WorldResourceController;
		_character._wRControl = _wRControl;
		_character._myBrain = this;
		_character.Init ();

		_character._rigidbody.isKinematic = true;
		//_character.GetComponent<CapsuleCollider> ().isTrigger = true;

		if (_myUIPrefab != null) {
			_myUI = Instantiate (_myUIPrefab) as GameObject;
			UpdateNpcUI test = _myUI.GetComponent<UpdateNpcUI> ();
			test.SetCharacter (_character);
			_myUI.SetActive (false);
			//_myUI.transform.SetParent (_character._myTransform);
			_myUI.transform.position = new Vector3(_character._animator.transform.position.x, _character._myTransform.position.y, _character._animator.transform.position.z);
			_myUI.transform.localScale = Vector3.one;
		}

		_agent = GetComponent<NavMeshAgent> ();

		_testing = GetComponentInChildren<CharacterModel> ();

        if (_aiBehaviour != null)
        {
            //NPCBehaviour newInstance = ScriptableObject.Instantiate(_aiBehaviour) as NPCBehaviour;
            //_aiBehaviour = newInstance; //new to this instance
           // _aiBehaviour._isNpc = true;
            /*
             * Creating a new instance solves the multiple sheep problem, where they all behave the same way
             * HOWEVER
             * This breaks all interactions with NPCs since they change only the original and not the cloned version
             */
        }

        _aiBehaviour.Init(this);

		_character._body._myNeeds.Reset ();
        //test
        _aiPattern.Init();
		//...


		switch (_character._biography._race) {
		case AllEnums.Race.Human:
			//humans

			//hardcoded and adjusted for enemies, these will be determined by perception in characterbody, found in _states
			//_character._bodySenses.Init(_character._biography._statistics._perception); //Init (90f, 10f, 0.2f);
			break;
		case AllEnums.Race.Humanoid:
			//humanoids
			break;
		case AllEnums.Race.Animal:
			//_character._rigidbody.isKinematic = false;
			_firstSight = FirstInSight;

			//hardcoded and adjusted for enemies, these will be determined by perception in characterbody, found in _states
			//int modPerception = Random.Range (_character._statistics._perception-1, _character._statistics._perception+2);
			//_character._bodySenses.Init (_character._biography._statistics._perception);
			//_character._bodySenses.Init (90f, 10f, delay); 
			//animals

			_myUI.name = _character._biography._name + "'s UI";

			_character._biography._currentBioCard._hasRevealedName = true;
			ToggleInteract allForUI = ScriptableObject.Instantiate (_wRControl.GetInteraction ("Sheep")) as ToggleInteract;
			allForUI.name = _character._biography._currentBioCard._nameOfCharacter + "'s UI";
			_wRControl._worldInteractions.AddInteraction (allForUI);
			_character._interactControl._myActiveInteraction = allForUI;
			_character._interactControl._interactHook._myInteraction = allForUI;
			_character._interactControl._interactHook.Setup ();
			_character._interactControl.OpenInteractCollider ();
			break;
		case AllEnums.Race.Monster:
			break;
		case AllEnums.Race.Unknown:
			break;
		default:
			break;
		}
	}

	public void GetTarget() {
		if (_character._lockOnTransform != null) {
			CharacterStateController target = _character._lockOnTransform.GetComponentInChildren<CharacterStateController> ();
			if (target != null && target._body._health.IsEmpty ()) {

				//test
				if (_home.gameObject.tag == "Banner" || _home.gameObject.tag == "Player") {
					return;
				}
				//...

				_aiSightState = AllEnums.EnemyAIState.Far;
				_character._lockOnTransform = null;
				Debug.Log ("Job's done");
				_character._attackedBy = null;
                //_home.gameObject.SetActive (true);

                _currentState = AllEnums.NpcState.Passive; //TOOD: SHOULD THIS BE ALERT?


				SetDestination (_home._spawnGroup.position);

				//_character._lockOnTransform = _home; //might change, otherwise the character is locking on to their home!
				//test
				reset = false;
			}
			return;
		}

		List<Transform> visible = _character._body.GetVisibleObjects (); //_bodySenses

		for (int i = 0; i < visible.Count; i++) {
			if (visible [i] == null) {
				//prevents call on object that may or may not have been removed from scene (deleted or otherwise)
				continue;
			}
			CharacterStateController entity = visible [i].GetComponentInChildren<CharacterStateController> ();
			if (entity != null && !entity._states._isDead) {
				//if we see another character and they are alive
				if (_character._biography._enemies.Contains(entity._biography._faction)) {
					//if the character is a sworn enemy - becomes our target to attack
					_character._lockOnTransform = visible [i].transform;
					return;
				}
			}
		}
	}

	public void SetDestination(Vector3 toDestination) {
		if (_agent.isActiveAndEnabled) {
			_agent.SetDestination (toDestination);
			_targetDestination = toDestination;
			_agent.isStopped = false;
		}
	}

	public GameObject GetMyUI() {
		return _myUI;
	}

    public void SetGiving()
    {
        _character._states._isGivingItem = true;
    }
    public void EquipConsumableItem(WorldItem thisItem)
    {
        int targetIndex = 0;
        List<WorldItem> consumableSlots = _character._invControl._runtimeRefs._consumable._value; //_player._invControl._currentEquipment._consumable._value;
        List<RuntimeConsumable> rtConsumables = _wRControl._runtimeReferences.FindRuntimeConsumables(true);
        Shepherd.RuntimeConsumable currentConsumable = _character._invControl._currentConsumable;

        List<RuntimeWorldItem> unequippedConsumables = _character._invControl._runtimeRefs.FindRTConsumables(false);
        //TODO: SIMILAR TO RuntimeReferences UpdateCurrentRuntimeConsumable
        for (int i = 0; i < unequippedConsumables.Count; i++)
        {
            Debug.Log(unequippedConsumables[i]._instance._itemName);
            //find storage slot item
            if (_wRControl.CompareWorldItem(unequippedConsumables[i]._instance._itemName, thisItem._itemName))
            {
                Shepherd.RuntimeConsumable consumableToEquip = (Shepherd.RuntimeConsumable)unequippedConsumables[i];
                consumableToEquip._isEquipped = true;
                consumableToEquip._itemName = consumableToEquip._instance._itemName;

                //instantiate Consumable to be equipped to currentConsumable
                Shepherd.Consumable consumableToInstanitate = (Shepherd.Consumable)consumableToEquip._instance;
                GameObject gObject = Instantiate(consumableToInstanitate._modelPrefab) as GameObject;
                gObject.name = consumableToEquip._instance._itemName;
                consumableToEquip._rtModel = gObject;

                //Destroy currentConsumable Model and label it as unequipped
                currentConsumable._isEquipped = false;
                Destroy(currentConsumable._rtModel);
                currentConsumable._rtModel = null;

                //Setup newly equipped consumable
                _character._invControl._currentConsumable = consumableToEquip;
                currentConsumable = consumableToEquip;
                consumableSlots[targetIndex] = consumableToEquip._instance;
                _character._invControl.EquipConsumable(currentConsumable, _character._biography._mainHand);
                Debug.Log("Equipping Consumable into hands yo");
                return;
            }
            Debug.Log("COULD NOT FIND ITEM IN INVENTORY, Maybe it doesn't exist?");
        }
    }

	void OnTriggerEnter(Collider other) {
		if (_aiPattern._hasTask) {
			switch (_aiPattern._taskType) {
			case AllEnums.BodyNeedsType.Thirst:
				if (other.tag == "Drink") {
                        _aiPattern._atTask = true;
					return;
				}
				break;
			case AllEnums.BodyNeedsType.Hunger:
				if (other.tag == "Food") {
                        _aiPattern._atTask = true;
					return;
				}
				break;
			case AllEnums.BodyNeedsType.Sleep:
				if (other.tag == "Home" && other.gameObject.transform.position == _home._spawnGroup.position) {
                        _aiPattern._atTask = true;
					return;
				}
				break;
			case AllEnums.BodyNeedsType.Waste:
				if (_character._body._myNeeds.Peek ()._type == AllEnums.BodyNeedsType.Waste && _character._myTransform.position != _home._spawnGroup.position) {
                        _aiPattern._atTask = true;
					return;
				}
				break;
			case AllEnums.BodyNeedsType.Pleasure:
				if (other.tag == "Home" && other.gameObject.transform == _home._spawnGroup) {
                        _aiPattern._atTask = true;
					return;
				}
				break;
			}
            _aiPattern._atTask = false;
        } else {
			if (!reset && other.tag == "Home" && other.gameObject.transform == _home._spawnGroup) {
				reset = true;
				//_agent.ResetPath();
				//Debug.Log ("Honey, I'm home!");
				//Debug.Log (_agent.hasPath);
				//_character._input._vertical = 0f;
				if (_aiPattern._taskType == AllEnums.BodyNeedsType.None) {
					_targetDestination = Vector3.zero;
				}
				//_home.gameObject.SetActive (false);
				_character._lockOnTransform = null; //TODO: THIS SHOULD NOT BE
				_aiSightState = AllEnums.EnemyAIState.Far;
                _currentState = AllEnums.NpcState.Passive;
            }
		}
	}

	void OnTriggerStay(Collider other) {
		if (_aiPattern._hasTask) {
			switch (_aiPattern._taskType) {
			case AllEnums.BodyNeedsType.Thirst:
				if (other.tag == "Drink") {
                        _aiPattern._atTask = true;
					return;
				}
				break;
			case AllEnums.BodyNeedsType.Hunger:
				if (other.tag == "Food") {
                        _aiPattern._atTask = true;
					return;
				}
				break;
			case AllEnums.BodyNeedsType.Sleep:
				if (other.tag == "Home" && other.gameObject.transform.position == _home._spawnGroup.position) {
                        _aiPattern._atTask = true;
					return;
				}
				break;
			case AllEnums.BodyNeedsType.Waste:
				if (_character._body._myNeeds.Peek ()._type == AllEnums.BodyNeedsType.Waste && _character._myTransform.position != _home._spawnGroup.position) {
                        _aiPattern._atTask = true;
					return;
				}
				break;
			case AllEnums.BodyNeedsType.Pleasure:
				if (other.tag == "Home" && other.gameObject.transform == _home._spawnGroup) {
                        _aiPattern._atTask = true;
					return;
				}
				break;
			}
		} 
	}

	void FixedUpdate() {
        if (!GameSessionController.Instance._successfulLaunch){
            return;
        }
		if (_isDrop) {
			return;
		}

        if (GameSessionController.Instance._curGameState == AllEnums.InputControlState.OutGame)
        {
            _character._animator.enabled = false;
            _pHorizontal = _character._input._horizontal;
            _pVertical = _character._input._vertical;
            _character._animator.SetFloat(InputStrings.Horizontal, 0);
            _character._animator.SetFloat(InputStrings.Vertical, 0);
            _agent.isStopped = true;
            return;
        }
        else
        {
            _character._animator.enabled = true;
            _character._input._horizontal = _pHorizontal;
            _character._input._vertical = _pVertical;
            _agent.isStopped = false;
        }

        _myUI.transform.position = new Vector3(_character._animator.transform.position.x, _character._animator.transform.position.y + 2f, _character._animator.transform.position.z);

		_delta = Time.deltaTime;
		_character.FixedTick (_delta);
		_character.MonitorStats ();

		if (_myGameEvent != null) {
			if (_character._attackedBy != null) {
				_character._states._isInConversation = false;
				_myUI.SetActive (true);
				if (_character._biography._faction == AllEnums.Faction.Saved) {
					OpenNameTag ();
				} else {
					CloseNameTag ();
				}
				OpenHealthPanel ();
				CloseBodyNeedsUI ();
				_myUI.transform.LookAt (Camera.main.transform);
			} else if (_character._attackedBy == null && _character._states._isInConversation) {
				//_myUI.SetActive (true);
				//OpenNameTag ();

				if (_character._biography._race == AllEnums.Race.Animal) {
					_character._attackedBy = null;
					OpenHealthPanel ();
					OpenBodyNeedsUI ();
					if (_character._biography._faction == AllEnums.Faction.Saved) {
						if (_wRControl._runtimeReferences._flockAlertSystem == AllEnums.FlockAlert.Danger) {
							_wRControl._runtimeReferences._flockAlertSystem = AllEnums.FlockAlert.None;
						}
					} 
				} else {
					CloseHealthPanel ();
					CloseBodyNeedsUI ();
				}

				_myUI.transform.LookAt (Camera.main.transform);
			} else {
				_character._states._isInConversation = false;
				_myUI.SetActive (false);
			}
			_myGameEvent.Raise ();
		}

		//temp
		if (_character._states._isDead) {
			//this.gameObject.layer = 0; // 11 = world layer  0 = default
			InitGiveInteraction ();
			Destroy (_agent);
			Destroy (_myUI);
			Destroy (this.GetComponent<Rigidbody> ());

			if (_character._biography._faction == AllEnums.Faction.Saved) {
				_wRControl._runtimeReferences._flock.Remove (_character);
			}

			//Destroy (this);
			return;
		}
		//...
	}

    void MyUpdate() {
        if (_isDrop) {
            CheckForPickup();
            return;
        }

        if (GameSessionController.Instance._curGameState == AllEnums.InputControlState.OutGame) {
            return;
        }

        _delta = Time.deltaTime;

        _character._states._isGrounded = true;


        //if I am not in a situation where I am performing an irreversible animation/action...
        if (!_character._states._isAnimatorInAction)
        {
            if (_character._attackedBy == null && !_aiPattern._hasTask)
            {
                //if not attacked by anything and has no task assigned...
                if (_character._body._myNeeds.Peek() != null)
                {
                    //StopCoroutine("Flock");
                    StopAllCoroutines();
                    //if a bodily function exists, go and prepare to do that (should it supercede all?)
                    SetupBodyTask();
                }
            }
            else if (_currentState != AllEnums.NpcState.Aggressive && _character._attackedBy != null)
            {

                //if you have a task and are being attacked
                _aiPattern._hasTask = false;
                if (_prevPattern != AllEnums.NpcAiPattern.BodyNeeds)
                {
                    _prevPattern = _curPattern; //body needs always gets checked, so once out of danger it will be called - no need to save it away and lose more important ai patterns
                }
                //TODO: STORE TASK SOMEWHERE?

                _currentState = AllEnums.NpcState.Aggressive;
                Debug.Log("Dropped task, being attacked");
                _aiSightState = AllEnums.EnemyAIState.InSight;
                _character._lockOnTransform = _character._attackedBy._myTransform;
                _character._animator.SetBool(AnimationStrings.IsLockOn, true);
                _character._states._isAbleToRotate = true;
                SetDestination(_character._lockOnTransform.position);
            }

            //Now, based off of the ai pattern states... which action should I perform?
            switch (_curPattern)
            {
                case AllEnums.NpcAiPattern.BodyNeeds:
                    //TODO: PUT IN SEPERATE FUNCTION...

                    //if I have reached my bodily function task, I should perform it
                    if (_aiPattern._atTask)
                    {
                        if (PerformBodyTask())
                        {
                            _aiPattern._hasTask = false;
                            _aiPattern._atTask = false;
                            _curPattern = _prevPattern; //if did body function, resume previous ai pattern
                            //test
                            reset = false;
                        }
                    }
                    break;
                case AllEnums.NpcAiPattern.Flock:
                    //TODO: PUT IN SEPERATE FUNCTION...

                    //if I am flocking... this usually means I stay around where I spawned...
                    if (_character._biography._faction == AllEnums.Faction.Saved)
                    {
                        //test --  if part of the saved faction -- a sheep or a saved person
                        Transform dest;
                        try
                        {
                            /*
                             * TODO:
                             * CURRENTLY, SHEEP SPAWN AROUND A BANNER, MAKING IT THEIR DEFAULT HOME.
                             * SO IF PLAYER PICKS UP BANNER, THEY CAUSE THEIR HOME TO NOT EXIST AND THROW AN ERROR.
                             * THIS DIRECTS EVERYONE TO THE PLAYER INSTEAD.
                             * Instead, this should be to parent/mother/father of sheep, or should not override home spawn point...
                             */
                            dest = GameObject.FindGameObjectWithTag("Banner").transform;
                        }
                        catch
                        {
                            dest = GameObject.FindGameObjectWithTag("Player").transform;
                        }
                        _home._spawnGroup = dest;

                        if (_home._spawnGroup.gameObject.tag != "Banner")
                        {
                            SetDestination(_home._spawnGroup.position);
                        }
                        else if (_home == null || (Vector3.Distance(transform.position, _home._spawnGroup.position) >= _home._areaOfInfluence))
                        {
                            StopCoroutine("Flock");
                            SetDestination(_home._spawnGroup.position);
                        }
                        else
                        {
                            StartCoroutine("Flock");
                        }
                    }
                    else
                    {
                        //if you don't have a home or you are outside of your home's area of influence, go home!
                        if (_home == null || (Vector3.Distance(transform.position, _home._spawnGroup.position) >= _home._areaOfInfluence))
                        {
                            StopCoroutine("Flock");
                            SetDestination(_home._spawnGroup.position);
                        }
                        else
                        {
                            //since you are within your home's area of influence, flock around inside.
                            StartCoroutine("Flock");
                        }
                    }
                    //look out for targets
                    GetTarget();
                    break;
                case AllEnums.NpcAiPattern.Stationary:
                    //currently, does nothing... should remain still where I spawned and turn occasionally
                    //Debug.Log("STATIONARY");
                    //look out for targets
                    GetTarget();
                    break;
                case AllEnums.NpcAiPattern.Waypoint:
                    //currently, should cause character to follow a path and stop at points to search around (possible triggers to advance)
                    Debug.Log("WAYPOINT");
                    DoWaypointStuff();
                    GetTarget();
                    break;
                case AllEnums.NpcAiPattern.Roam:
                    //from initial spawn, characters keep moving in a direction until something changes (a timer is set to avoid stagnated movement and alter direction)
                    break;
            }
        }

        _character._moveAmount = Mathf.Clamp(_agent.desiredVelocity.sqrMagnitude, 0f, 0.5f);

        _character._states._isCrouching = false; //for some reason if this isn't set the character crouches... TODO

        

        _distance = DistanceFromTarget();
        _angle = AngleToTarget();

        //if you have a target locked in, your move direction (where to point the character towards)
        if (_character._lockOnTransform != null)
        {
            _character._moveDirection = _character._lockOnTransform.position - transform.position;
        }
        else
        {
            _character._moveDirection = Vector3.zero;
        }

        switch (_aiSightState)
        {
            case AllEnums.EnemyAIState.Near:
                HandleNearSight(); //Near...
                break;
            case AllEnums.EnemyAIState.Far:
                HandleFarSight(); //Far...
                break;
            case AllEnums.EnemyAIState.InSight:
                HandleInSight(); //...Wherever you are!
                GetTarget();
                break;
            case AllEnums.EnemyAIState.Attacking:
                HandleAIStateAttacking();
                break;
            default:
                break;
        }

        //
        _character.Tick(_delta);

        switch (_currentState)
        {
            case AllEnums.NpcState.Aggressive:
                break;
            case AllEnums.NpcState.Passive:
                HandleWorldInteractions();
                break;
            case AllEnums.NpcState.Alert:
                HandleWorldInteractions();
                break;
        }


        //if the character is an animal, they do not have a stored inventory, so they must use the object they pick up right away!
        if (_character._biography._race == AllEnums.Race.Animal)
        {
            //automatically consume item if it's an animal
            if (!_wRControl.CompareWorldItem(_character._invControl._currentConsumable._instance._itemName, _character._invControl._emptyItemsDefault._itemName))
            {
                _character._states._isUsingItem = true;
            }
        }
    }

    void Update()
    {
        //test
        MyUpdate();
        return;
    }

	IEnumerator Flock(){
		if (_aiPattern._flockTimer > _aiPattern._flockCount) {
            _aiPattern._flockTimer = 0;
            _aiPattern._isRoaming = !_aiPattern._isRoaming;
			yield return null;
		}
        _aiPattern._flockTimer++;

		if (_aiPattern._isRoaming) {
			if (Random.Range (0, 250) < 5) {
                _aiPattern._isTurning = true;
                _aiPattern._targetRotated = new Vector3 (Random.Range (-180f, 180f), 0f, Random.Range (-180f, 180f));
			}
				
			if (!_aiPattern._isTurning) {
				if (Random.Range (0, 5) < 1) {
					GoFlock ();
				}
				_character._moveAmount = Time.deltaTime * _character._myControlStats._moveSpeed;
				if (_carrot != null) {
					_agent.SetDestination (_carrot.position);
				}
				//_character._rigidbody.transform.Translate (0f, 0f, Time.deltaTime * _character._myControlStats._moveSpeed);
				//_myBody.velocity = new Vector3 (0f, 0f, Time.deltaTime * _speed);
			} else {
				while (Quaternion.Angle (transform.rotation, Quaternion.LookRotation (_aiPattern._targetRotated)) > 0.01f) {
					if (_agent != null) {
						_agent.SetDestination (_character._myTransform.position);
					}
					//_character._rigidbody.transform.Translate (0f, 0f, 0f);
					//_myBody.velocity = Vector3.zero;
					_character._myTransform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (_aiPattern._targetRotated), _character._myControlStats._rotateSpeed * Time.deltaTime);
					yield return null;
				}
                _aiPattern._isTurning = false;
			}
		}
		yield return null;
	}

	private void GoFlock() {
		GameObject[] gos;
		//collection of everyone within the flock gos = gameObjects
		gos = _home._population;

		//calculate the center of the group as well as an avoidance for hitting others
		Vector3 vCentre = _home._spawnGroup.position; // _mySoul.getBanner().transform.position;
		Vector3 vAvoid = _home._spawnGroup.position; //_mySoul.getBanner().transform.position;
		//group speed
		float gSpeed = 0f;


		//acquire goal position
		Vector3 goalPos = _home._spawnGroup.position; //_mySoul.getBanner().transform.position;

		float distance;

		//groupSize depends on neighbour distance (closer gameObjects will be placed together under a group or flock -- flock within a flock?)
		int groupSize = 0;

		for (int i = 0; i < gos.Length; i++) {
			if (gos [i] != this.gameObject) {
				distance = Vector3.Distance (gos [i].transform.position, this.transform.position);
				if (distance <= _aiPattern._neighbourCling) {
					vCentre += gos [i].transform.position;
					groupSize++;

					//if a gameobject is too close, move outta the way to avoid collision with another gameobject
					if (distance < _aiPattern._personalSpace) {
						vAvoid += (this.transform.position - gos [i].transform.position);
					}
					//calculate average speed of flock
					CharacterStateController anotherFlock = gos [i].GetComponent<CharacterStateController> ();
					if (anotherFlock != null) {
						gSpeed += anotherFlock._moveAmount;
					}
				}
			}
		}
		//if we are in a group
		if (groupSize > 0) {
			//calculate average center of group and avg speed
			vCentre = vCentre / groupSize + (goalPos - _home._spawnGroup.position);//_mySoul.getBanner().transform.position);
			//vCentre = vCentre / groupSize;
			//_speed = gSpeed / groupSize;
			_character._moveAmount = gSpeed / groupSize;

			//use vCenter and vAvoid to give direction to turn away form
			Vector3 direction = (vCentre + vAvoid) - transform.position;
			direction.y = 0;

			//if we have to move off 
			if (direction != Vector3.zero) {
				_character._myTransform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (direction), _character._myControlStats._rotateSpeed * Time.deltaTime); //_myBody.transform.rotation   ... _turnSpeed
			}
			//_myBody.velocity = new Vector3 (direction.x, 0f, direction.z);
		}
	}

	private void SetupBodyTask() {
        _aiPattern._hasTask = true;
        _aiPattern._atTask = false;

        _prevPattern = _curPattern;
        _curPattern = AllEnums.NpcAiPattern.BodyNeeds;

		List<Transform> _placesNearby = _character._body.GetAudibleObjects (); //prev _bodySenses

		switch (_character._body._myNeeds.Peek ().GetStatusType()) {
		case AllEnums.BodyNeedsType.Hunger:
			if (_character._invControl._currentConsumable._unbreakable != true) {
				//if we have a consumable in our inventory, it might satisfy our need
			} else if (_placesNearby.Count > 1) {
				//if we do not have a consumable in our inventory, there may be a location nearby we can satisfy our need
				for (int i = 0; i < _placesNearby.Count; i++) {
					if (_placesNearby[i] != null && _placesNearby[i].gameObject.tag == "Food") {
						_character._lockOnTransform = null;
						SetDestination (_placesNearby [i].transform.position);

                            _aiPattern._taskType = AllEnums.BodyNeedsType.Hunger;

                            _aiPattern._hasTask = true;
						return;
					}
				}
			}

            if (_aiPattern._poi [AllEnums.BodyNeedsType.Hunger].Count > 0) {
				//is there any place i remember from memory that I can go to?
				_character._lockOnTransform = null;
				SetDestination (_aiPattern._poi [AllEnums.BodyNeedsType.Hunger][0]);
				Debug.Log ("Recalling from memory a location to eat :)");

                    _aiPattern._taskType = AllEnums.BodyNeedsType.Hunger;

                    _aiPattern._hasTask = true;
				return;
			}
            //if there is nothing around and no location previous known that solved this issue, TODO: SHOULD ROAM?
			break;
		case AllEnums.BodyNeedsType.Thirst:
			if (_character._invControl._currentConsumable._unbreakable != true) {
				//if we have a consumable in our inventory, it might satisfy our need
			} else if (_placesNearby.Count > 1) {
				for (int i = 0; i < _placesNearby.Count; i++) {
					if (_placesNearby[i] != null && _placesNearby [i].gameObject.tag == "Drink") {
						SetDestination (_placesNearby [i].transform.position);

                            _aiPattern._taskType = AllEnums.BodyNeedsType.Thirst;

                            _aiPattern._hasTask = true;
						return;
					}
				}
			}

            if (_aiPattern._poi [AllEnums.BodyNeedsType.Thirst].Count > 0) {
				//is there any place i remember from memory that I can go to?
				_character._lockOnTransform = null;
				SetDestination (_aiPattern._poi [AllEnums.BodyNeedsType.Thirst][0]);
				Debug.Log ("Recalling from memory a location to drink :)");
                    //_agent.isStopped = false; //?

                    _aiPattern._taskType = AllEnums.BodyNeedsType.Thirst;

                    _aiPattern._hasTask = true;
				return;
			}
                //if there is nothing around and no location previous known that solved this issue, TODO: SHOULD ROAM?
                break;
		case AllEnums.BodyNeedsType.Sleep:
                _aiPattern._taskType = AllEnums.BodyNeedsType.Sleep;

			SetDestination(_home._spawnGroup.position);
			return;
		case AllEnums.BodyNeedsType.Waste:
                _aiPattern._taskType = AllEnums.BodyNeedsType.Waste;

			SetDestination(_character._myTransform.position);
			return;
		case AllEnums.BodyNeedsType.Pleasure:
                _aiPattern._taskType = AllEnums.BodyNeedsType.Pleasure;

			SetDestination(_home._spawnGroup.position);
			return;
		}
        //nothing I can do, so I am going home to figure it out from there
        _aiPattern._hasTask = false;
        _curPattern = _prevPattern;

        //SetDestination(_home._spawnGroup.position); throws error if home does not exist (deleted or home is null)

        _aiPattern._taskType = AllEnums.BodyNeedsType.None;
	}

	private bool PerformBodyTask() {
		//returns true if task is completed
		switch (_character._body._myNeeds.Peek ().GetStatusType()) {
		case AllEnums.BodyNeedsType.Hunger:
			Debug.Log ("Eating...");
			_character._body._hunger.Add (3f); //TODO: Hardcoded value!
			_character._body._thirst.Add(0.03f); //TODO: Hardcoded value!
			_character._body._waste.Add(0.5f); //TODO: Hardcoded value!
			if (_character._body._hunger.GetMeter() >= _character._body._hunger.GetMaximum()) {
				_character._body._myNeeds.Dequeue ();
				_character._body._myNeeds.Reset ();
				if (!_aiPattern._poi [AllEnums.BodyNeedsType.Hunger].Contains (_targetDestination)) {
                        _aiPattern._poi [AllEnums.BodyNeedsType.Hunger].Add (_targetDestination);
				}
                    _aiPattern._taskType = AllEnums.BodyNeedsType.None;

				_character._biography._currentBioCard._status = AllEnums.Status.Normal;
				return true;
			} else {
				return false;
			}
		case AllEnums.BodyNeedsType.Thirst:
			Debug.Log ("Drinking...");
			_character._body._thirst.Add (5f); //TODO: Hardcoded value!
			_character._body._waste.Add (2f); //TODO: Hardcoded value!
			if (_character._body._thirst.GetMeter() >= _character._body._thirst.GetMaximum()) {
				_character._body._myNeeds.Dequeue ();
				_character._body._myNeeds.Reset ();
				if (!_aiPattern._poi [AllEnums.BodyNeedsType.Thirst].Contains (_targetDestination)) {
                        _aiPattern._poi [AllEnums.BodyNeedsType.Thirst].Add (_targetDestination);
				}
                    _aiPattern._taskType = AllEnums.BodyNeedsType.None;

				_character._biography._currentBioCard._status = AllEnums.Status.Normal;
				return true;
			} else {
				return false;
			}
		case AllEnums.BodyNeedsType.Sleep:
			Debug.Log ("Sleeping...");
			_character._body._sleep.Add (1f);
			_character._body._hunger.Subtract (0.05f); //TODO: Hardcoded value!
			_character._body._thirst.Subtract (0.05f); //TODO: Hardcoded value!
			_character._body._pleasure.Add (0.05f); //TODO: Hardcoded value!
			if (_character._body._sleep.GetMeter() >= _character._body._sleep.GetMaximum()) {
				_character._body._myNeeds.Dequeue ();
				_character._body._myNeeds.Reset ();
                    _aiPattern._taskType = AllEnums.BodyNeedsType.None;

				_character._biography._currentBioCard._status = AllEnums.Status.Normal;
				return true;
			} else {
				return false;
			}
		case AllEnums.BodyNeedsType.Waste:
			Debug.Log ("Bathroom...");
			_character._body._waste.Subtract (5f);
			if (_character._body._waste.GetMeter() <= 0) {
				_character._body._myNeeds.Dequeue ();
				_character._body._myNeeds.Reset ();
                    _aiPattern._taskType = AllEnums.BodyNeedsType.None;

				_character._biography._currentBioCard._status = AllEnums.Status.Normal;
				return true;
			} else {
				return false;
			}
		case AllEnums.BodyNeedsType.Pleasure:
			Debug.Log ("Uh... XD");
			_character._body._pleasure.Regen ();
			if (_character._body._pleasure.GetMeter() >= _character._body._hunger.GetMaximum()) {
				_character._body._myNeeds.Dequeue ();
				_character._body._myNeeds.Reset ();
                    _aiPattern._taskType = AllEnums.BodyNeedsType.None;

				_character._biography._currentBioCard._status = AllEnums.Status.Normal;
				return true;
			} else {
				return false;
			}
		default:
			_character._biography._currentBioCard._status = AllEnums.Status.Normal;
			return true;
		}
	}

	private void CheckForPickup() {
		//InteractionContainer myInteractionContainer = GetComponent<InteractionContainer> ();


		//if (myInteractionContainer != null && myInteractionContainer._myInteraction._enabled == false) {
		if (_character._interactControl._myActiveInteraction != null && _character._interactControl._myActiveInteraction._enabled == false) {
			//this.gameObject.layer = 0; // 11 = world layer  0 = default
			_testing.UnequipAll ();
			Destroy (_testing);

			_character._interactControl.CloseInteractCollider();
			_character._interactControl._myActiveInteraction._enabled = false;
			_character._invControl.DestroyColliders();
			Destroy (_character._invControl._currentLeftWeapon._rtModel);
			Destroy (_character._invControl._currentRightWeapon._rtModel);
			Destroy (_character._invControl._currentConsumable._rtModel);
			Destroy (_character);
			Destroy (this);
		}
	}

	void HandleNearSight() {
		_proximityTimer++; //Time.DeltaTime?
		if (_proximityTimer > _proximityCount) {
			_proximityTimer = 0;

			if (!_character._body.GetVisibleObjects ().Contains (_character._lockOnTransform) && !_character._body.GetAudibleObjects ().Contains (_character._lockOnTransform)) { //prev _bodySenses
				_aiSightState = AllEnums.EnemyAIState.Far;
                _currentState = AllEnums.NpcState.Passive;
                _character._attackedBy = null; //test
                //TODO: LOG ATTACKER IN THE BIOCARD AND NEGATE RELATIONS TO THEM SLIGHTLY, SO NEXT TIME I'LL BE MORE CAUTIOUS AROUND THEM
			} else if (_character._body.GetVisibleObjects ().Contains (_character._lockOnTransform) || _character._states._isBackstabbed) { //previous _bodySenses
				_aiSightState = AllEnums.EnemyAIState.InSight;
                _currentState = AllEnums.NpcState.Aggressive;
                _character._animator.SetBool (AnimationStrings.IsLockOn, true);
				_character._states._isAbleToRotate = true;
				SetDestination (_character._lockOnTransform.position);
				if (_firstSight != null) {
					_firstSight ();
				}
			}



			/*if (_distance > _sight.GetSenseRadius () || _angle > _sight.GetSightAngle ()) {
				_aiSightState = AllEnums.EnemyAIState.Far;
				return;
			}*/
		} 

		if (_character._lockOnTransform != null && _carrot != null) {
			//temp workaround allowing enemy to stay fixated slightly onto their target (they move to their right)
			SetDestination (new Vector3(_carrot.position.x + 1f, _carrot.position.y, _carrot.position.z));
		}


		//RaycastToTarget (); //not sure if needed;
	}

	void GoToTarget() {
		SetDestination (_character._lockOnTransform.position);
	}

	void HandleInSight() {
		HandleCooldowns ();

		if (_character._lockOnTransform == null) { //prev _bodySenses
			// && _character._bodySenses.GetAudibleObjects().Contains(_character._lockOnTransform)

			//if we don't have a target, then it is no longer in our sight!
			_aiSightState = AllEnums.EnemyAIState.Far;
            _currentState = AllEnums.NpcState.Passive;
			return;
		}

		if (!_character._body.GetVisibleObjects ().Contains (_character._lockOnTransform) && _character._body.GetAudibleObjects ().Contains (_character._lockOnTransform)) {
			//if we do not visually see the enemy, that means they are nearby
			_aiSightState = AllEnums.EnemyAIState.Near;
			return;
		} else if (!_character._body.GetVisibleObjects ().Contains (_character._lockOnTransform) && !_character._body.GetAudibleObjects ().Contains (_character._lockOnTransform)) {
			_aiSightState = AllEnums.EnemyAIState.Far;
            _currentState = AllEnums.NpcState.Passive;
            _character._attackedBy = null; //TODO
        }

        float distanceToDestination = DistanceFromTarget();//Vector3.Distance (_targetDestination, _character._lockOnTransform.position);

        if (distanceToDestination > 1.5f && _distance < _character._body.GetSenseRadius ()) { //prev _bodySenses
			GoToTarget ();
		} else if (_distance <= 1.5f || _aiSightState == AllEnums.EnemyAIState.Attacking) { //TODO: Hardcoded value!!
            Debug.Log("in range");
			_agent.isStopped = true;
			//_character._rigidbody.isKinematic = false;
		}

        /*else if (!_character._states._isAnimatorInAction && !_character._states._isDelayed) {
			_agent.isStopped = false;
			_character._rigidbody.isKinematic = true;
		}*/

		if (_attackTimer > 0) {
			_attackTimer--;
			return;
		}
		_attackTimer = _attackCount;

		EnemyAIAttacks attack = WillAttack ();

		if (attack != null) {
			_aiSightState = AllEnums.EnemyAIState.Attacking;
			_character._states._isAbleToAttack = true;
			SetCurrentAttack (attack);
			_character.CheckForAttackInput ();

			attack._cool = attack._cooldown;
			_agent.isStopped = true;
			_character._states._isAbleToRotate = false;
			//_agent.enabled = false;
		} else {
			_character._actionControl._actionIndex = 0;
		}

		_character._input._weakRightButton = false;
		_character._input._weakLeftButton = false;
		_character._input._strongRightButton = false;
		_character._input._strongLeftButton = false;

		/*
		_stateControl.SetCurrentAttack(attack);
		if (attack != null) {
			_aiSightState = AllEnums.EnemyAIState.Attacking;
			_stateControl._animator.Play (attack._targetAnimation._variable);
			_stateControl._animator.SetBool (AnimationStrings.IsMirrored, attack._isMirrored);
			_stateControl._animator.SetBool (AnimationStrings.IsInAction, true); //false
			_stateControl._states._isAbleToMove = false;
			attack._cool = attack._cooldown;

			_stateControl._body._stamina.Subtract (attack._staminaCost);

			_agent.isStopped = true;
			_stateControl._states._isAbleToRotate = false;
			_agent.enabled = false;

			return;
		}
		*/
	}

	void SetCurrentAttack(EnemyAIAttacks attack) {
		if (attack == null) {
			return;
		}
		switch (attack._inputType) {
		case AllEnums.ActionInputType.RightWeak:
			_character._input._weakRightButton = true;
			break;
		case AllEnums.ActionInputType.RightStrong:
			_character._input._strongRightButton = true;
			break;
		case AllEnums.ActionInputType.LeftWeak:
			_character._input._weakLeftButton = true;
			break;
		case AllEnums.ActionInputType.LeftStrong:
			_character._input._strongLeftButton = true;
			break;
		default:
			break;
		}
	}

	void HandleAIStateAttacking() {
		//if (_stateControl._states._isAbleToMove) {
		if (!_character._states._isAnimatorInAction) {
			_aiSightState = AllEnums.EnemyAIState.InSight;
			_character._states._isAbleToRotate = true;
			//_agent.enabled = true;
			_agent.isStopped = true; //false
		}
	}

	void HandleCooldowns() {
		for (int i = 0; i < _enemyAIAttacks.Length; i++) {
			EnemyAIAttacks attack = _enemyAIAttacks [i];
			if (attack._cool > 0) {
				attack._cool -= _delta;
				if (attack._cool < 0) {
					attack._cool = 0;
				}
			}
		}
	}

	/*void RaycastToTarget() {
		RaycastHit hit;
		Vector3 origin = transform.position;
		origin.y += 0.5f;
		Vector3 direction = _moveDirection;
		direction.y += 0.5f;
		if (Physics.Raycast(origin, direction, out hit, _stateControl._bodySenses.GetSenseRadius(), _states._ignoreLayers)) {
			StateController st = hit.transform.GetComponentInParent<StateController> ();
			if (st != null) {
				_aiSightState = AllEnums.EnemyAIState.InSight;
			}
		}
	}*/

	void HandleFarSight() {
		//_stateControl._bodySenses.Tick (_delta);
		if (_character._lockOnTransform == null) {
			_character._states._isAbleToRotate = false;
			return;
		}
		if (_character._body.GetVisibleObjects ().Contains (_character._lockOnTransform)) { //prev _bodySenses
			_aiSightState = AllEnums.EnemyAIState.InSight;
            _currentState = AllEnums.NpcState.Aggressive;
            if (_firstSight != null) {
				_firstSight ();
			}
		} else if (_character._body.GetAudibleObjects ().Contains (_character._lockOnTransform)) { //prev _bodySenses
			_aiSightState = AllEnums.EnemyAIState.Near;
			Debug.Log ("Lost sight");
		} else {
			Debug.Log ("Outta range, outta mind");
            _currentState = AllEnums.NpcState.Passive;
            _character._states._isAbleToRotate = false;

			_character._lockOnTransform = null; //we lost tracking on our target... oh well
			_character._attackedBy = null;

            //should I go home?
            SetDestination(_home._spawnGroup.position);
		}
	}

	void FirstInSight() {
		_character._animator.Play ("taunt"); //hard coded will only work with spider currently
		_character._states._isAbleToMove = false;
		_character._states._isAnimatorInAction = true; //false
		_character._animator.SetBool (AnimationStrings.IsInAction, true);
		_character._states._isDelayed = true;
	}

	//rename
	public EnemyAIAttacks WillAttack() {
		int weight = 0;
		List<EnemyAIAttacks> list = new List<EnemyAIAttacks> ();
		for (int i = 0; i < _enemyAIAttacks.Length; i++) {
			EnemyAIAttacks attack = _enemyAIAttacks [i];
			/*if (attack._staminaCost > _states._body._stamina.GetMeter ()) {
				continue;
			}*/
			if (attack._cool > 0) {
				continue;
			}
			if (_distance > attack._minDistance) { // > _distance
				continue;
			}
			if (_angle < attack._minAngle) {
				continue;
			}
			if (_angle > attack._maxAngle) { //_stateControl._bodySenses.GetSightAngle ()) {
				continue;
			}
			if (attack._weight == 0) {
				continue;
			}

			weight += attack._weight;
			list.Add (attack);
		}
		if(list.Count == 0) {
			return null;
		}

		int randomInt = Mathf.RoundToInt(Random.Range(0, weight+1));
		int counterWeight = 0;
		for (int i = 0; i < list.Count; i++) {
			counterWeight += list [i]._weight;
			if (counterWeight > randomInt) {
				return list[i];
			}
		}
		return null;
	}

	//don't like this, it might not be neccessary
	private float DistanceFromTarget() {
		if (_character._lockOnTransform == null) {
			return 100f;
		}
		return Vector3.Distance (_character._lockOnTransform.position, transform.position);
	}

	private float AngleToTarget() {
		float a = 180f;
		if (_character._lockOnTransform != null) {
			Vector3 d = _character._moveDirection;
			a = Vector3.Angle (d, transform.forward);
		}
		return a;
	} //might be useful for keeping target in sight

	private void InitGiveInteraction() {
		if (_isDrop) {
			return; //already dropped items
		}
		_isDrop = true;
		//_wRControl = _eStates._wResourceControl;
		Pickup itemToGive = ScriptableObject.CreateInstance<Pickup>(); //will this save somewhere or only create/dump this at runtime?
		itemToGive.name = "testItemDrop";
		itemToGive._enabled = true;
		itemToGive._repeatable = false;
		StringVariable animation = ScriptableObject.CreateInstance<StringVariable> ();
		animation._variable = AnimationStrings.StatePickupItem;
		itemToGive._animation = animation;
		itemToGive._prompt = AllEnums.PromptType.Examine;
		itemToGive._items = _character._invControl._equippedItems;
		//_wRControl._worldInteractions._allInteractions.Add (itemToGive);
		_wRControl._worldInteractions.AddInteraction(itemToGive);
		StartCoroutine ("WaitForDrop");

		_character._interactControl._myActiveInteraction = itemToGive;
		if (_character._interactControl._interactHook != null) {
			_character._interactControl._interactHook._myInteraction = itemToGive;
			_character._interactControl.OpenInteractCollider ();
			Debug.Log ("dropping Items");
		}
		//InteractionContainer myInteractionContainer = GetComponent<InteractionContainer> ();
		//if (myInteractionContainer != null) {
		//	myInteractionContainer._myInteraction = itemToGive;


		//}
	}

    private void HandleWorldInteractions()
    {
        //if character is aggressive, they cannot pickup anything around them, but if they are in the other states, they can and immediately will (TODO: They should discern what they are picking up, but for now they are cleptomaniacs)
        if (_currentState == AllEnums.NpcState.Aggressive)
        {


        }
        else
        { 
            if (_character._interactControl._interactions.Count != 0)
            {
                WorldInteraction wInteract = _character._interactControl._interactions[_character._interactControl._index]._myInteraction;
                if (wInteract is Give)
                {
                    Give gInteract = (Give)wInteract;
                    if (_character._biography._race == AllEnums.Race.Animal)
                    {
                        
                        if (_character._biography._alliances.Contains(gInteract._source._biography._faction))
                        {
                            _character.HandleInteractions();
                        }
                    } else if (_character._biography._race != AllEnums.Race.Animal && _enablePickup)
                    {
                        _character.HandleInteractions();
                    }
                } //HERE IS WHERE WE CAN DETERMINE WHAT THE AI INTERACTS WITH IN THE ENVIRONMENT (ie, open doors, pickup objects, able to recieve objects, etc)

                if (_character._biography._race == AllEnums.Race.Human && wInteract is Give)
                {
                    _character.HandleInteractions();
                }
            }
            else
            {
                _character._input._interactConfirmButton = false;
            }
        }
    }

    private void DoWaypointStuff()
    {
        if(_aiPattern._waypoints.Length == 0)
        {
            return;
        }

        if (_aiPattern._curWaypointIndex >= _aiPattern._waypoints.Length && !_aiPattern._patrolWaypoints)
        {
            return;
        }
        else if (_aiPattern._curWaypointIndex >= _aiPattern._waypoints.Length && _aiPattern._patrolWaypoints)
        {
            _aiPattern._curWaypointIndex = 0; //back to start
        }
        else
        {
            ReachedWaypoint();
        }

        if (_aiPattern._curWaypointIndex < _aiPattern._waypoints.Length)
        {
            SetDestination(_aiPattern._waypoints[_aiPattern._curWaypointIndex].coord);
        }
    }

    public bool ReachedWaypoint()
    {
        if (!_agent.pathPending && _aiPattern._waypoints[_aiPattern._curWaypointIndex].waitToDepart && _agent.remainingDistance < 0.5f)
        {
            if (_aiPattern._waypoints[_aiPattern._curWaypointIndex].canDepart)
            {
                Debug.Log("DEPARTING FROM QAYPONTIN");
                _aiPattern._curWaypointIndex++;
                _agent.ResetPath();
            }
            return true;
        }
        else if (!_agent.pathPending && !_aiPattern._waypoints[_aiPattern._curWaypointIndex].waitToDepart && _agent.remainingDistance < 0.5f)
        {
            Debug.Log("ONTO THE NEXT POINT");
            _aiPattern._curWaypointIndex++;
            _agent.ResetPath();

            return true;
        } else
        {
            return false;
        }
    }

	IEnumerator WaitForDrop() {
		yield return new WaitForSeconds (2f);
		if (_character._interactControl._interactHook != null) {
			_character._interactControl._interactHook.enabled = true;
		}
	}

    public void EnablePickupItem()
    {
        _enablePickup = true;
    }
    public void DisablePickupItem()
    {
        _enablePickup = false;
    }

    public void OpenNameTag() {
		_myUI.SetActive (true);
		_myUI.GetComponent<UpdateNpcUI> ()._nameTag.SetActive (true);
	}
	public void CloseNameTag() {
		_myUI.GetComponent<UpdateNpcUI> ()._nameTag.SetActive (false);
	}

	public void OpenHealthPanel() {
		_myUI.GetComponent<UpdateNpcUI> ()._healthPanel.SetActive (true);
	}
	public void CloseHealthPanel() {
		_myUI.GetComponent<UpdateNpcUI> ()._healthPanel.SetActive (false);
	}

	public void OpenBodyNeedsUI() {
		_myUI.GetComponent<UpdateNpcUI> ()._bodyNeedsPanel.SetActive (true);
	}
	public void CloseBodyNeedsUI() {
		_myUI.GetComponent<UpdateNpcUI> ()._bodyNeedsPanel.SetActive (false);
	}

	public void OpenTargetCursorUI() {
		_myUI.SetActive (true);
		_myUI.GetComponent<UpdateNpcUI> ()._targetCursor.SetActive (true);
	}

	public void CloseTargetCursorUI() {
		_myUI.SetActive (false);
		_myUI.GetComponent<UpdateNpcUI> ()._targetCursor.SetActive (false);
	}
}

[System.Serializable]
public class EnemyAIAttacks {
	public int _weight;
	public float _minDistance; //for attack to go off
	public float _minAngle;
	public float _maxAngle;

	public float _cooldown = 2f;
	public float _cool;

	public AllEnums.ActionInputType _inputType;
}