using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[System.Serializable]
	public class Dialogue {
		public string _name;

		public bool _playAnimation;
		public StringVariable _animation;

		[TextArea(1,10)]
		public string _dialogueText;
		public AudioClip _dialogueVoice;
		//rename this
		public bool _advanceDialogue;

		public GameEvent _specialEvent;
		public bool _advanceSpecialEvent;
	}
}