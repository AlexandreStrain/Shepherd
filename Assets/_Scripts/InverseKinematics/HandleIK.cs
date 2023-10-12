using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleIK : MonoBehaviour {

	public float _weight;
	public float _speedIK = 3f;

	public Vector3 _defaultHeadPosition;

	private Animator _animator;
	private Transform _handHelper;
	private Transform _bodyHelper;
	private Transform _headHelper;
	private Transform _headTransform;
	private Transform _shoulderHelper;
	private Transform _animatorShoulder;

	public IKSnapShot[] _IKSnapShots;

	public void Init(Animator animator) {
		_animator = animator;

		_headHelper = new GameObject ().transform;
		_headHelper.name = "IK Head Helper";

		_handHelper = new GameObject ().transform;
		_handHelper.name = "IK Hand Helper";

		_bodyHelper = new GameObject ().transform;
		_bodyHelper.name = "IK Body Helper";

		_shoulderHelper = new GameObject ().transform;
		_shoulderHelper.name = "IK Shoulder Helper";

		_shoulderHelper.parent = transform.parent;
		_shoulderHelper.localPosition = Vector3.zero;
		_shoulderHelper.localRotation = Quaternion.identity;

		_headHelper.parent = _shoulderHelper;
		_headTransform = _animator.GetBoneTransform (HumanBodyBones.Head);
		_bodyHelper.parent = _shoulderHelper;
		_handHelper.parent = _shoulderHelper;
	}

	public void UpdateIKTargets(IKSnapShotType type, bool isLeftHand) {
		IKSnapShot desiredSnapShot = GetIKSnapShot (type);

		Vector3 targetBodyPos = desiredSnapShot._IKBodyPosition;

		//test
		//if (isLeftHand) {
		//	targetBodyPos.x = -targetBodyPos.x;
		//}
		//...
		_bodyHelper.localPosition = targetBodyPos;

		_handHelper.localPosition = desiredSnapShot._IKHandPosition;
		_handHelper.localEulerAngles = desiredSnapShot._IKHandEulers;

		if (desiredSnapShot._overwriteHeadPos) {
			_headHelper.localPosition = desiredSnapShot._IKHeadPosition;
			_headHelper.localEulerAngles = desiredSnapShot._IKHeadEulers;
		} else {
			_headHelper.localPosition = _defaultHeadPosition;
		}
	}

	public void IKTick(AvatarIKGoal aGoal, float weight) {
		_weight = Mathf.Lerp (_weight, weight, Time.deltaTime * _speedIK);

		_animator.SetIKPositionWeight (aGoal, _weight);
		_animator.SetIKRotationWeight (aGoal, _weight);

		_animator.SetIKPosition (aGoal, _handHelper.position);
		_animator.SetIKRotation (aGoal, _handHelper.rotation);

		_animator.SetLookAtWeight (_weight, 0.8f, 1f, 1f, 1f); //hardcoded values!!
		_animator.SetLookAtPosition(_bodyHelper.position);
	}

	public void OnAnimatorMoveTick(bool isLeftHand) {
		Transform shoulder = _animator.GetBoneTransform ((isLeftHand) ? HumanBodyBones.LeftShoulder : HumanBodyBones.RightShoulder);
		_shoulderHelper.transform.position = shoulder.position;
	}

	public void LateTick() {
		if (_headTransform != null && _headHelper != null) {
			Vector3 direction = _headHelper.position - _headTransform.position;
			if (direction == Vector3.zero) {
				direction = _headTransform.forward;
			}

			//Quaternion targetRotation = Quaternion.LookRotation (direction);
			Quaternion targetRotation = _headHelper.rotation;
			Quaternion currentRotation = Quaternion.Slerp (_headTransform.rotation, targetRotation, _weight);

			_headTransform.rotation = currentRotation;
		}
	}

	private IKSnapShot GetIKSnapShot(IKSnapShotType snapshotType) {
		for (int i = 0; i < _IKSnapShots.Length; i++) {
			if (_IKSnapShots [i]._type == snapshotType) {
				return _IKSnapShots [i];
			}
		}
		return null;
	}
}

public enum IKSnapShotType {
	Breath, ShieldRight, ShieldLeft
};

//TODO: Make this scriptable, so we can change this on runtime...
[System.Serializable]
public class IKSnapShot {
	public IKSnapShotType _type;

	public Vector3 _IKHandPosition;
	public Vector3 _IKHandEulers;

	public Vector3 _IKBodyPosition;
	public bool _overwriteHeadPos;
	public Vector3 _IKHeadPosition;
	public Vector3 _IKHeadEulers;
}