using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/World Objects/NPC/Behaviour")]
	public class NPCBehaviour : ScriptableObject {

		public EnemyAIController _npc;
        WorldInteraction prevInteraction;

        public delegate void TaskStart ();
		public delegate void TaskEnd ();

		public TaskStart _taskStart;
		public TaskEnd _taskEnd;


        public void Init(InputModule module) {
			_npc = (EnemyAIController)module;
		}

		public void SwitchFactions(int toFaction) {
			AllEnums.Faction newFaction = (AllEnums.Faction)toFaction;
			switch (newFaction) {
			case AllEnums.Faction.None: //0
				_npc._character._biography._faction = newFaction;

				_npc._character._biography._enemies.Clear ();
				_npc._character._biography._enemies.Add (AllEnums.Faction.Fallen);
				break;
			case AllEnums.Faction.Citizen: //1
				_npc._character._biography._faction = newFaction;

				_npc._character._biography._enemies.Clear ();
				_npc._character._biography._enemies.Add (AllEnums.Faction.Wild);
				_npc._character._biography._enemies.Add (AllEnums.Faction.Fallen);
				break;
			case AllEnums.Faction.Saved: //2
				_npc._character._biography._faction = newFaction;

				_npc._character._biography._enemies.Clear ();
				_npc._character._biography._enemies.Add (AllEnums.Faction.Fallen);

				_npc._character._biography._alliances.Add (AllEnums.Faction.Saved);
				break;
			case AllEnums.Faction.Wild: //3
				_npc._character._biography._faction = newFaction;

				_npc._character._biography._enemies.Clear ();
				_npc._character._biography._enemies.Add (AllEnums.Faction.Citizen);
				_npc._character._biography._enemies.Add (AllEnums.Faction.Fallen);

				_npc._character._biography._alliances.Add (AllEnums.Faction.Wild);
				break;
			case AllEnums.Faction.Fallen: //4
				_npc._character._biography._faction = newFaction;

				_npc._character._biography._enemies.Clear ();
				_npc._character._biography._enemies.Add (AllEnums.Faction.Citizen);
				_npc._character._biography._enemies.Add (AllEnums.Faction.Wild);
				_npc._character._biography._enemies.Add (AllEnums.Faction.None);
				_npc._character._biography._enemies.Add (AllEnums.Faction.Saved);
				break;
			default:
				_npc._character._biography._faction = AllEnums.Faction.None;

				_npc._character._biography._enemies.Clear ();
                _npc._character._biography._alliances.Clear ();
                break;
			}
		}

		public void ChangeActiveInteraction(WorldInteraction toInteraction) {
			_npc._character._interactControl._myActiveInteraction._enabled = false;

			toInteraction._enabled = true;
			_npc._character._interactControl._myActiveInteraction = toInteraction;
		}

		public void AdvanceDialogueToIndex(int toIndex) {
			if (toIndex >= 0 && toIndex < _npc._character._biography._allDialogue.Length) {
				_npc._character._biography._currentBioCard._verbalIndex = toIndex;
			} else {
				Debug.Log ("Invalid Index");
			}
		}
        public void IncrementDialogueIndex()
        {
            if (_npc._character._biography._currentBioCard._verbalIndex < _npc._character._biography._allDialogue.Length)
            {
                _npc._character._biography._currentBioCard._verbalIndex++;
            }
            else
            {
                Debug.Log("Unable to Increment Index");
            }
        }

		//rename
		public void BringToDeathsDoor() {
			_npc._character._body._health.ResetToMaximum ();
			_npc._character._body._health.Subtract ((_npc._character._body._health.GetMaximum() - 1));
		}

		public void AwaitDeathFromPlayer() {
			_taskStart = null;
			_taskStart += BringToDeathsDoor;
			_taskStart += _npc._character._interactControl.DisableActiveInteraction;


			_taskEnd = null;
			_taskEnd += _npc._character._body._health.ResetToMaximum;
			_taskEnd += _npc._character._interactControl.EnableActiveInteraction;

			_npc.StartCoroutine (WaitForResponse (10f));
		}

        public void AllowWaypointDeparture()
        {
            _npc._aiPattern._waypoints[_npc._aiPattern._curWaypointIndex].canDepart = true;
        }

        public void IncrementDialogueAtWaypoint(Waypoint atWaypoint)
        {
            _taskStart = null;
            _taskStart += _npc._character._interactControl.DisableActiveInteraction;

            _taskEnd = null;
            _taskEnd += IncrementDialogueIndex;
            _taskEnd += _npc._character._interactControl.EnableActiveInteraction;

            _npc.StartCoroutine(WaitForWaypointReached(atWaypoint));
        }

        public void AwaitItemFromPlayer(WorldItem questItem)
        {
            _taskStart = null;
            _taskStart += _npc.EnablePickupItem;
            _npc._receivedPickup = false;

            _taskEnd = null;
            _taskEnd += _npc.DisablePickupItem;
            _taskEnd += IncrementDialogueIndex;

            _npc.StartCoroutine(WaitForItem(questItem));
        }

        public void AwaitTimeOfDay(float atTime)//, UnityEvent beforeTime, UnityEvent afterTime)
        {
            _taskStart = null;
            //_taskStart += beforeTime.Invoke;

            _taskEnd = null;
            //_taskEnd += afterTime.Invoke;
            _taskEnd += AllowWaypointDeparture;

            _npc.StartCoroutine(WaitForTimeOfDay(atTime));

        }

        public void GiveItemToPlayer(WorldItem thisItem)
        {
            _taskStart = null;
            _taskEnd = null;

            prevInteraction = _npc._character._interactControl._myActiveInteraction;

            _npc.EquipConsumableItem(thisItem);

            _taskStart += _npc.SetGiving;


            _npc.StartCoroutine(HandOverItem(thisItem));
        }

        public void GiveItemsToPlayer(EquipmentData theseItems)
        {
            _taskStart = null;
            _taskEnd = null;

            prevInteraction = _npc._character._interactControl._myActiveInteraction;

            Give giveInteract = (Give)_npc._character._wRControl._worldInteractions.GetInteraction(_npc._character._biography._name + "'s Offering");
            if (giveInteract == null)
            {
                Debug.Log("WHOOPS -- Didn't create a give interaction for current entity");
                return;
            }
            else
            {
                giveInteract._itemsToGive = theseItems;
            }

            _taskStart += _npc.SetGiving;



            _npc.StartCoroutine(HandOverItems(theseItems));
        }

        public void IncrementSpecialIndex(int index = -1)
        {
            if (index == -1)
            {
                _npc._character._biography._currentBioCard._specialEventIndex++;
            } else
            {
                _npc._character._biography._currentBioCard._specialEventIndex = index;
            }
        }

        public void IncrementSpecialIndexAndPlay(int index = -1)
        {
            if (index == -1)
            {
                _npc._character._biography._currentBioCard._specialEventIndex++;
            }
            else
            {
                _npc._character._biography._currentBioCard._specialEventIndex = index;
            }
            if (_npc._character._biography._allDialogue[_npc._character._biography._currentBioCard._specialEventIndex]._specialEvent != null)
            {
                _npc._character._biography._allDialogue[_npc._character._biography._currentBioCard._specialEventIndex]._specialEvent.Raise();
            }
        }

        IEnumerator HandOverItem(WorldItem thisItem)
        {
            if (_taskStart != null)
            {
                _taskStart();
            }
            yield return new WaitUntil(() => (_npc._character._invControl._currentConsumable._instance != thisItem));
            //yield return new WaitUntil(() => (_npc._character._interactControl.GetInteraction()._enabled != true));
            Debug.Log("Well, wherever the item went, it's out of my hands now.");
            _npc._character._states._isGivingItem = false;
            _npc._character._interactControl._myActiveInteraction = prevInteraction;
            _npc._character._interactControl._interactHook._myInteraction = prevInteraction;
            //_npc._character._interactControl._interactHook.Setup();
            if (_taskEnd != null)
            {
                _taskEnd();
            }
        }

        IEnumerator HandOverItems(EquipmentData thisItem)
        {
            if (_taskStart != null)
            {
                _taskStart();
            }
            //yield return new WaitUntil(() => (_npc._character._invControl._currentConsumable._instance != thisItem));
            yield return new WaitUntil(() => (_npc._character._interactControl._myActiveInteraction._enabled == false));
            Debug.Log("Well, wherever the item went, it's out of my hands now yo.");
            _npc._character._states._isGivingItem = false;
            _npc._character._interactControl._myActiveInteraction = prevInteraction;
            _npc._character._interactControl._interactHook._myInteraction = prevInteraction;
            //_npc._character._interactControl._interactHook.Setup();
            if (_taskEnd != null)
            {
                _taskEnd();
            }
        }

        IEnumerator WaitForItem(WorldItem questItem)
        {
            Debug.Log("waiting for..." + questItem._itemName);
            if (_taskStart != null)
            {
                _taskStart();
            }
            yield return new WaitUntil(() => _npc._character._wRControl.CompareWorldItem(_npc._character._invControl._currentConsumable._instance._itemName, questItem._itemName));
            Debug.Log("Oh, I got it");
            if (_taskEnd != null)
            {
                _taskEnd();
            }
        }

        IEnumerator WaitForResponse(float waitTime) {
			Debug.Log ("waiting...");
			if (_taskStart != null) {
				_taskStart ();
			}
			yield return new WaitForSecondsRealtime (waitTime);
			Debug.Log ("done waiting");
			if (_taskEnd != null) {
				_taskEnd();
			}
		}

        IEnumerator WaitForTimeOfDay(float waitUntilTime)
        {
            Debug.Log("Waiting for a certain time of day...");
            if (_taskStart != null)
            {
                _taskStart();
            }
            yield return new WaitUntil(() => GameSessionController.Instance._gameTimeOfDay >= waitUntilTime);
            Debug.Log("Oh, It's passed time");
            if (_taskEnd != null)
            {
                _taskEnd();
            }
        }

        IEnumerator WaitForWaypointReached(Waypoint atWaypoint)
        {
            Debug.Log("Waiting until I hit a waypoint...");
            if (_taskStart != null)
            {
                _taskStart();
            }
            yield return new WaitUntil(() => _npc.ReachedWaypoint());
            Debug.Log("Oh, I've reached my waypoint");
            if (_taskEnd != null)
            {
                _taskEnd();
            }
        }

		public void RevealName() {
            _npc._character._biography._currentBioCard._hasRevealedName = true;
		}
		public void ConcealName() {
            _npc._character._biography._currentBioCard._hasRevealedName = false;
		}

        public void SwitchAiPattern(int toPattern)
        {
            AllEnums.NpcAiPattern newPattern = (AllEnums.NpcAiPattern)toPattern;

            switch (newPattern)
            {
                case AllEnums.NpcAiPattern.Stationary: //0
                    _npc._prevPattern = _npc._curPattern;
                    _npc._curPattern = newPattern;
                    break;
                case AllEnums.NpcAiPattern.Flock: //1
                    _npc._prevPattern = _npc._curPattern;
                    _npc._curPattern = newPattern;
                    break;
                case AllEnums.NpcAiPattern.Waypoint: //2
                    _npc._prevPattern = _npc._curPattern;
                    _npc._curPattern = newPattern;
                    Debug.Log("SWITCHING TO WAYPOiNT");
                    break;
                case AllEnums.NpcAiPattern.BodyNeeds: //3
                    _npc._prevPattern = _npc._curPattern;
                    _npc._curPattern = newPattern;
                    break;
                case AllEnums.NpcAiPattern.Roam: //4
                    _npc._prevPattern = _npc._curPattern;
                    _npc._curPattern = newPattern;
                    break;
                default:
                    break;
            }
        }
	}
}