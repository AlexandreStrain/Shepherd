using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/Variables/Transform")]
	public class TransformVariable : ScriptableObject {

		public Transform _value;

		public void SetTransform(Transform toTransform) {
			_value = toTransform;
		}
	}
}