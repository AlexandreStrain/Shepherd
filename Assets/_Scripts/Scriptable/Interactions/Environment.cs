using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	[CreateAssetMenu(menuName = "Shepherd/World Interactions/Environment Interaction")]
	public class Environment : WorldInteraction {

        [SerializeField]
		public GameObject _worldObject; //such as door or boulder
        public Animator _animator;
        public bool _toggle;
		public StringVariable _environmentAnimation;

        //TODO: May need this when loading in from save file
        public override void Init()
        {
            base.Init();
        }

        public override void Interact (CharacterStateController audience) {
            _toggle = !_toggle;
            base.Interact(audience);
		}
	}
}