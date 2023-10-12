using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnController : MonoBehaviour {
	public static LockOnController _singleton;

	public List<LockOnTarget> _enemyTargets = new List<LockOnTarget> ();

	/*CHANGE MIN DISTANCE TO THE PLAYER'S VISION RADIUS*/

	public LockOnTarget GetEnemy(Vector3 fromPosition) {
		LockOnTarget returnTarget = null;

		float minDistance = float.MaxValue;
		for (int i = 0; i < _enemyTargets.Count; i++) {
			float tempDistance = Vector3.Distance (fromPosition, _enemyTargets [i].GetTarget ().position);
			if (tempDistance < minDistance) {
				minDistance = tempDistance;
				returnTarget = _enemyTargets [i];
			}
		}
		return returnTarget;
	}

	void Awake() {
        Debug.Log(this.name);
        _singleton = this;
	}
}