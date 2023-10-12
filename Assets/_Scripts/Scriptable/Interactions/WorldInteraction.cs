using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	public class WorldInteraction : ScriptableObject {
		[Header("General Information")]
		public AllEnums.PromptType _prompt;
		public StringVariable _animation;
		public bool _enabled; //if interaction is currently turned on for scene or level
		[HideInInspector]
		public Vector3 _position; //position of interaction in world space
		public bool _repeatable;
		public GameEvent _specialEvent; //scriptable?

        public virtual void Init()
        {
            _enabled = true;
        }
	
		public virtual void Interact(CharacterStateController audience) {
			//_isPlaying = true;
			Debug.Log (this.name + " is playing!");
		}

		public virtual void ChangePromptToTalk() {
			_prompt = AllEnums.PromptType.Talk;
		}
		public virtual void ChangePromptToGive() {
			_prompt = AllEnums.PromptType.Give;
		}
		public virtual void ChangePromptToPickup() {
			_prompt = AllEnums.PromptType.Pickup;
		}
		public virtual void ChangePromptToExamine() {
			_prompt = AllEnums.PromptType.Examine;
		}
		public virtual void ChangePromptToUse() {
			_prompt = AllEnums.PromptType.Use;
		}
		public virtual void ChangePromptToOpen() {
			_prompt = AllEnums.PromptType.Open;
		}
		public virtual void ChangePromptToClose() {
			_prompt = AllEnums.PromptType.Close;
		}
		public virtual void ChangePromptToListen() {
			_prompt = AllEnums.PromptType.Listen;
		}
		public virtual void ChangePromptToContinue() {
			_prompt = AllEnums.PromptType.Continue;
		}
		public virtual void ChangePromptToSkip() {
			_prompt = AllEnums.PromptType.Skip;
		}
	}
}