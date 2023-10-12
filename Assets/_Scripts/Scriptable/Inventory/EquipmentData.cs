using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/World Objects/Equipment Data", order = 0)]
	public class EquipmentData : ScriptableObject {

		public List<WorldItem> _data = new List<WorldItem>();
	}
}