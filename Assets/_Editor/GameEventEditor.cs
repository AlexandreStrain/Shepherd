#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Shepherd;

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor {

	public override void OnInspectorGUI() {
		//base.OnInspectorGUI ();

		GUI.enabled = Application.isPlaying;
		GameEvent gEvent = target as GameEvent;
		if(GUILayout.Button("Raise")) {
			gEvent.Raise();
		}
	}
}
#endif