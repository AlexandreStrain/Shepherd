using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	public class Banner : MonoBehaviour {

		public bool _active = true;
		public BoxCollider _myCollider;
		public SpecialEventListener _myEvents;

		public List<GameObject> _pointsOfInterest;

		public float _areaOfInfluence = 10f;

		public GameObject[] _population;

		public CharacterStateController _owner;
		private GameObject _objectInScene;

		public void SetupOnGround() {
			Debug.Log ("Connecting Banner to flock and player...");
            _objectInScene = this.gameObject.transform.parent.gameObject;
            _myCollider.enabled = true;
			this.gameObject.transform.rotation = Quaternion.identity;
			_population = GameObject.FindGameObjectsWithTag ("Flock");
			_owner = GameObject.FindGameObjectWithTag ("Player").GetComponent<StateController>();
			for (int i = 0; i < _population.Length; i++) {
				_population [i].GetComponent<EnemyAIController> ()._home._spawnGroup = this.transform;
			}
		}

		public void DestroyUponPickup() {
			Debug.Log ("destroying currently placed banner");
			_myCollider.enabled = false;
			_active = false;
			Destroy (_objectInScene);
		}
	}
}