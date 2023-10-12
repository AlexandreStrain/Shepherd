using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd
{
    [CreateAssetMenu(menuName = "Shepherd/World Interactions/Door Interaction")]
    public class DoorInteraction : Environment
    {
        public StringVariable _openAnimation;
        public StringVariable _closeAnimation;

        //TODO: May need this when loading in from save file
        public override void Init()
        {
            base.Init();
        }

        public override void Interact(CharacterStateController audience)
        {
            _animator = _worldObject.GetComponentInParent<Animator>();
            if (_animator != null && _animator.gameObject.activeSelf)
            {
                //TODO: Hardcoded isOpen should be a variable
                if (!_animator.GetBool("isOpen"))
                {
                    _animator.SetTrigger(_openAnimation._variable);
                    ChangePromptToClose();
                    _animator.SetBool("isOpen", true);
                    _toggle = true;
                    //Debug.Log ("Opened Door");
                }
                else if (_animator.GetBool("isOpen"))
                {
                    _animator.SetTrigger(_closeAnimation._variable);
                    ChangePromptToOpen();
                    _animator.SetBool("isOpen", false);
                    _toggle = false;
                    //Debug.Log ("Closed Door");
                }

            }
        }
    }
}