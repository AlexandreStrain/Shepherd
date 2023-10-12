using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/User Interface/UI Icons")]
	public class UserInterfaceIcons : ScriptableObject {

		public List<Sprite> _allAttributeIcons = new List<Sprite> ();
		public List<Sprite> _allAttackDefenceIcons = new List<Sprite> ();
		public List<Sprite> _allResistancesIcons = new List<Sprite> ();
		public List<Sprite> _allBodyNeedsIcons = new List<Sprite> ();
		public List<Sprite> _allStatusBarIcons = new List<Sprite>();
		public List<Sprite> _allButtonPromptIcons = new List<Sprite> (); //0-3 = Xbox, 4 = Xbox,PC,Playstation, 5-8 = Playstation

		public Sprite _blankSpriteTemplate;
	}
}