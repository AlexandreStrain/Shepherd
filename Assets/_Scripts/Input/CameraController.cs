using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shepherd;
using Utilities;

public class CameraController : Singleton<CameraController> {

	public Cinemachine.CinemachineStateDrivenCamera _mainCamera;
	public Cinemachine.CinemachineFreeLook _playerCam;
	public Cinemachine.CinemachineFreeLook _lockOnCam;
	public Cinemachine.CinemachineTargetGroup _cameraGroupTransform;

	private CharacterStateController _stateManager;

	public void Init(CharacterStateController stateManager) {
		_stateManager = stateManager;

		_mainCamera = GameObject.FindGameObjectWithTag (InputStrings.TagCamera).GetComponent<Cinemachine.CinemachineStateDrivenCamera> ();
		_mainCamera.Follow = GameObject.FindGameObjectWithTag ("Player").transform;
		_mainCamera.LookAt = _stateManager._animator.GetBoneTransform (HumanBodyBones.RightShoulder);
		_mainCamera.m_AnimatedTarget = _stateManager._animator;

		_cameraGroupTransform = GameObject.FindGameObjectWithTag (InputStrings.TagCameraTarget).GetComponent<Cinemachine.CinemachineTargetGroup> ();
		_cameraGroupTransform.m_Targets [0].target = _stateManager._animator.GetBoneTransform (HumanBodyBones.RightShoulder).transform;
		_cameraGroupTransform.m_Targets [1].target = _stateManager._animator.GetBoneTransform (HumanBodyBones.RightShoulder).transform;

		_mainCamera.ChildCameras [1].LookAt = _cameraGroupTransform.transform;
	}

	public void PauseAllCameras() {
		_lockOnCam.enabled = false;
		_playerCam.enabled = false;
	}
	public void ResumeAllCameras() {
		_mainCamera.enabled = true;
		_lockOnCam.enabled = true;
		_playerCam.enabled = true;
	}

	public void ResetLookat() {
		_cameraGroupTransform.m_Targets [1].target = _cameraGroupTransform.m_Targets [0].target;
		_mainCamera.ChildCameras [1].LookAt = _cameraGroupTransform.transform;
		//Debug.Log (_cameraGroupTransform.m_Targets [1].target);
	}

	public void FixedTick(float delta) {
		if (_stateManager._lockOnTransform != null) {
			_cameraGroupTransform.m_Targets [1].target = _stateManager._lockOnTransform;
			_lockOnCam.LookAt = _cameraGroupTransform.transform;
		} else {
			_cameraGroupTransform.m_Targets [1].target = _cameraGroupTransform.m_Targets [0].target;
			_lockOnCam.LookAt = _cameraGroupTransform.transform;
		}
	}
}