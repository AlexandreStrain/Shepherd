using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	public class WorldPickup : MonoBehaviour {


		public Collider _myCollider;
		public SpecialEventListener _myEvents; //not sure if needed

		public GameObject _objectInScene;

		public void DestroyUponPickup() {
			Debug.Log ("destroying currently placed item");
			_myCollider.enabled = false;
			Destroy (_objectInScene);
		}
	}
}