using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	public class DelegateCalls {
		public delegate void SpellCastStart();
		public delegate void SpellCastLoop();
		public delegate void SpellCastStop();
		public delegate void ItemEffectLoop();

		//rename
		public delegate void FirstSight();
	}
}