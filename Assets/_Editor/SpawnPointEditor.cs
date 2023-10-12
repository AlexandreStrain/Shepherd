#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Shepherd;

[CustomEditor(typeof(SpawnPoint))]
public class SpawnPointEditor : Editor {
	[DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
	void OnSceneGUI() {
		SpawnPoint fov = (SpawnPoint)target;
		Handles.color = Color.white;
		Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360f, fov._areaOfInfluence);
	}
}
#endif