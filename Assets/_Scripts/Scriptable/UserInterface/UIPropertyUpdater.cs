﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shepherd {
	public class UIPropertyUpdater : GameEventListener {
	    /// <summary>
	    /// Use this to update the UI element as soon as THIS gameObject is enabled
	    /// </summary>
	    public bool raiseOnEnable;
	    /// In the off chance you need to update a UI element when disabled, just add the OnDisable() method

	    /// <summary>
	    /// If there's a gameEvent assigned it will automatically call the Raise() method.
	    /// </summary>

	    public override void Response() {
			if (_gameEvent != null) {
				Raise ();
			}
	    }

	    public virtual void Raise() {
	        
	    }

	    public override void OnEnableLogic() {
	        base.OnEnableLogic();
	        if(raiseOnEnable) {
	            Raise();
	        }
	    }
	}
}