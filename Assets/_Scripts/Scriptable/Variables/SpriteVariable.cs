﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/Variables/Sprite")]
	public class SpriteVariable : ScriptableObject {
	    public Sprite value;

	    public void Set(Sprite v) {
	        value = v;
	    }

	    public void Set(SpriteVariable v) {
	        value = v.value;
	    }
	}
}