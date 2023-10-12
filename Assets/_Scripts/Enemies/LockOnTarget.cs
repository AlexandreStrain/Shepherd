using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shepherd;

public class LockOnTarget : MonoBehaviour {

	public int _index;
	public List<Transform> _bodyTargets;
	public List<HumanBodyBones> _humanBodyTargets = new List<HumanBodyBones> ();

	private Animator _animator;

	public Transform GetTarget(bool negative = false) {
		if (_bodyTargets.Count == 0) {
			return transform;
		}

		int targetIndex = _index;

		if (!negative) {
			if (_index < _bodyTargets.Count - 1) {
				_index++;
			} else {
				_index = 0;
			}
		} else {
			if (_index <= 0) {
				_index = _bodyTargets.Count - 1;
			} else {
				_index--;
			}
		}

		_index = Mathf.Clamp (_index, 0, _bodyTargets.Count);

		return _bodyTargets [targetIndex];
	}

	public void Init(CharacterStateController character) {
		_animator = character._animator;
		if (!_animator.isHuman) {
			return;
		} else {
			for (int i = 0; i < _humanBodyTargets.Count; i++) {
				_bodyTargets.Add (_animator.GetBoneTransform (_humanBodyTargets [i]));
			}

			LockOnController._singleton._enemyTargets.Add (this);
		}
	}
}