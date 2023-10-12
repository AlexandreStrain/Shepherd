#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (Senses))]
public class SensesEditor : Editor {

	[DrawGizmo(GizmoType.Selected)]
	void OnSceneGUI() {
		Senses fov = (Senses)target;
		Handles.color = Color.white;
		Handles.DrawWireArc (fov.transform.position, Vector3.up, Vector3.forward, 360f, fov._body.GetSenseRadius());
		Vector3 viewAngleA = fov.DirFromAngle (-fov._body.GetSightAngle() / 2f, false);
		Vector3 viewAngleB = fov.DirFromAngle (fov._body.GetSightAngle() / 2f, false);

		Handles.DrawLine (fov.transform.position, fov.transform.position + viewAngleA * fov._body.GetSenseRadius());
		Handles.DrawLine (fov.transform.position, fov.transform.position + viewAngleB * fov._body.GetSenseRadius());

		Handles.color = Color.red;
		foreach (Transform visibleTarget in fov.GetVisibleObjects()) {
			Handles.DrawLine (fov.transform.position, visibleTarget.position);
		}

		//useful to see which targets are within x radius from object -- will be used for audio later
		Handles.color = Color.yellow;
		foreach (Transform audibleTarget in fov.GetAudibleObjects()) {
			if (audibleTarget != null) {
				Handles.DrawLine (fov.transform.position, audibleTarget.position);
			}
		}
	}
}
#endif