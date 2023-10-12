using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/World Interactions/Conversation Interaction")]
	public class Conversation : WorldInteraction {

		public CharacterStateController _source;

		public override void Interact (CharacterStateController audience) {
			if ((audience._myBrain is InputController) == false) {
				return; //TODO: Temporary, but if you aren't the player, Ignore
			}
			audience._states._isInConversation = true;
			_source._states._isInConversation = true;

            if (_source._myBrain is EnemyAIController)
            {
                if (_source._biography._currentBioCard._hasRevealedName)
                {
                    DialogueController._singleton._speakerOfDialogueText.text = _source._biography._name + " :";
                }
                else
                {
                    DialogueController._singleton._speakerOfDialogueText.text = "??? :";
                }
            }


            //if (audience._network.ContainsKey (_source._biography)) {
            if (audience._wRControl._runtimeReferences._runtimeNetwork.ContainsKey(_source._biography)) { 
				Debug.Log ("Continuing our conversation");
				DialogueController._singleton.InitDialogue (_source._myTransform, _source._biography, audience);
                
            } else {
				Debug.Log ("We've never met before!");

				//audience._network.Add (_source._biography, _source._biography._currentBioCard);
                audience._wRControl._runtimeReferences._runtimeNetwork.Add(_source._biography, _source._biography._currentBioCard);
                DialogueController._singleton.InitDialogue (_source._myTransform, _source._biography, audience);
			}
            AddToFeed(_source._biography);
			base.Interact (audience);
        }

        private void AddToFeed(Biography toChatWith)
        {
            UserInterfaceController.Instance.OpenInfoFeed();
            GameObject gObject = Instantiate(UserInterfaceController.Instance._informationFeed._infoGrid._template) as GameObject;
            gObject.transform.SetParent(UserInterfaceController.Instance._informationFeed._infoGrid._grid);
            InfoFeedData message = gObject.GetComponentInChildren<InfoFeedData>();
            message._infoImage.sprite = toChatWith._portrait;
            message._description.text = (toChatWith._currentBioCard._hasRevealedName) ? toChatWith._name : "???";
            message._action.text = _prompt.ToString();
            message._timeStamp.text = GameSessionController.Instance._dayNightCycle._timeText.text;
            gObject.SetActive(true);
            UserInterfaceController.Instance._informationFeed.AddToInfoFeed(message);
        }
    }
}