using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Similar to transform variables but this one is for keeping references to gameObjects or 
/// better yet, prefabs. So you don't waste memory allocating the same prefab on multiple objects.
/// </summary>
namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/Variables/GameObject")]
	public class GameObjectVariable : ScriptableObject
	{
	    public GameObject value;
	}
}