using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/Single Instances/Left Hand Position")]
	public class LeftHandPosition : ScriptableObject {

		public Vector3 _position;
		public Vector3 _eulers;
	}
}