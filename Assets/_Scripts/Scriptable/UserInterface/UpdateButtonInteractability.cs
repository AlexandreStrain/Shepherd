﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shepherd {
	public class UpdateButtonInteractability : UIPropertyUpdater
	{
	    public BoolVariable targetBool;
	    public Button targetButton;

	    public override void Raise() {
	        targetButton.interactable = targetBool.value;
	    }
	}
}