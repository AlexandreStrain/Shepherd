#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorUtilities {
	
	public static List<T> FindAssetByType<T>() where T : UnityEngine.Object {
		List<T> desiredAssets = new List<T> ();
		//Globally Unique Identifier (guid)
		string[] guids = AssetDatabase.FindAssets (string.Format ("t:{0}", typeof(T)));

		for (int i = 0; i < guids.Length; i++) {
			string assetPath = AssetDatabase.GUIDToAssetPath (guids [i]);
			T asset = AssetDatabase.LoadAssetAtPath<T> (assetPath);
			if (asset != null) {
				desiredAssets.Add (asset);
			}
		}

		return desiredAssets;
	}
}
#endif