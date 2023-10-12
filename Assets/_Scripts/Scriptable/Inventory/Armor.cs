using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/World Objects/Armor", order = 1)]
	public class Armor : WorldItem {
		[Header("Armor")]
		public AllEnums.ArmorType _armorType;
		//public AllEnums.Race _armorRace;
		public Mesh _armorMesh;
		public Material[] _armorMaterials;
		public bool _baseBodyEnabled;
	}
}