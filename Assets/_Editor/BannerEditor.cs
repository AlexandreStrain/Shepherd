#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Shepherd;

[CustomEditor(typeof(Banner))]
public class BannerEditor : Editor {
	[DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
	void OnSceneGUI() {
	Banner fov = (Banner)target;
	Handles.color = Color.cyan;
	Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360f, fov._areaOfInfluence);
	}
}
#endif