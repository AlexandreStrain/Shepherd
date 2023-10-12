using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Utilities;

namespace Shepherd
{
    //TODO: Is this needed to be a singleton?
    public class DialogueController : Singleton<DialogueController>
    {

        public static DialogueController _singleton;
        public WorldResourceController _wRControl;

        public Text _dialogueText;
        public Text _speakerOfDialogueText;
        public GameObject _overlay;
        public bool _dialogueActive;
        public AudioClip _testDialoguePutt;
        public bool _subtitlesOn;

        public InteractionPrompt _buttonContinue;
        public InteractionPrompt _buttonSkip;

        public float _secondsPerLine;
        public float _pauseTime;
        private bool _lineCompleted;
        private string[] _currentDialogue;
        private string _currentLine;

        private Transform _origin;
        private bool _updateDialogue;
        private int _lineIndex;

        private Dialogue[] _npcDialogue;
        private int _dialogueIndex;

        private Biography _currentBio;
        private CharacterStateController _currentCardHolder; //typically the player, but i guess it could also be npc

        public Transform _player;
        List<float> _letterTimings = new List<float>();

        public void Init(Transform player)
        {
            if (_singleton == null)
            {
                _singleton = this;
            }
            else if (_singleton != this)
            {
                Destroy(this);
            }

            _player = player;

            _overlay.SetActive(false);

            _buttonContinue.CloseHUDPrompt();
            _buttonSkip.CloseHUDPrompt();
        }

        public void InitDialogue(Transform origin, Biography fromBio, CharacterStateController fromCardHolder)
        {
            _origin = origin;

            _currentBio = fromBio;
            _npcDialogue = fromBio._allDialogue;
            //_dialogueIndex = fromCardHolder._network[fromBio]._verbalIndex;
            _dialogueIndex = fromCardHolder._wRControl._runtimeReferences._runtimeNetwork[fromBio]._verbalIndex;
            _currentCardHolder = fromCardHolder;
            _dialogueActive = true;
            _overlay.SetActive(true);
            _updateDialogue = false;
            _lineIndex = 0;

            //added to make it appear on screen
            _lineCompleted = false;
            _currentDialogue = null;
            _currentLine = "";
            _dialogueText.text = "";

            if (fromBio._testVoice != null)
            {
                _testDialoguePutt = fromBio._testVoice;
            }

            _buttonContinue.CloseHUDPrompt();
            _buttonSkip.OpenHUDPrompt(AllEnums.PromptType.Skip, true);
        }

        public void Tick(bool talkInput, bool skipInput, float delta)
        {

            if (!_dialogueActive || _origin == null)
            {
                return;
            }

            float distance = Vector3.Distance(_player.transform.position, _origin.transform.position);
            if (distance > 3f)
            { //TODO: Hardcoded... should be based on player 
                CloseDialogue();
            }
            _overlay.SetActive(true);

            if (!_updateDialogue)
            {
                _updateDialogue = true;

                _lineCompleted = false;
                _currentDialogue = _npcDialogue[_dialogueIndex]._dialogueText.Split("@".ToCharArray());
                _currentLine = _currentDialogue[_lineIndex];
                _letterTimings = GetCharacterTime(_currentLine.ToCharArray());


                if (_npcDialogue[_dialogueIndex]._playAnimation)
                {
                    Animator npcAnimator = _origin.GetComponentInChildren<Animator>();
                    npcAnimator.Play(_npcDialogue[_dialogueIndex]._animation._variable);
                }

                StartCoroutine("ScrollText");
            }

            _dialogueText.gameObject.SetActive(_subtitlesOn);

            if (skipInput)
            {
                StopAllCoroutines();

                _dialogueText.text = _currentLine;
                _lineCompleted = true;
                GetComponent<AudioSource>().Stop();

                _buttonContinue.OpenHUDPrompt(AllEnums.PromptType.Continue);
            }

            if (talkInput && _lineCompleted)
            {
                StopAllCoroutines();
                _lineCompleted = false;
                _dialogueText.text = ""; //or += "\n";

                _buttonContinue.CloseHUDPrompt();

                _updateDialogue = false;
                _lineIndex++;

                if (_lineIndex > (_currentDialogue.Length - 1))
                {
                    if (_npcDialogue[_dialogueIndex]._specialEvent != null)
                    {
                        Debug.Log("Conversation Ended, now a special event plays");
                        if (_npcDialogue[_dialogueIndex]._advanceSpecialEvent)
                        {
                            //_npcDialogue [_dialogueIndex]._specialEvent.RaiseResponse (_currentCardHolder._network [_currentBio]._specialEventIndex);
                            //_currentCardHolder._network [_currentBio]._specialEventIndex++;
                            _npcDialogue[_dialogueIndex]._specialEvent.RaiseResponse(_currentCardHolder._wRControl._runtimeReferences._runtimeNetwork[_currentBio]._specialEventIndex);
                            _currentCardHolder._wRControl._runtimeReferences._runtimeNetwork[_currentBio]._specialEventIndex++;
                        }
                        else
                        {
                            _npcDialogue[_dialogueIndex]._specialEvent.Raise();
                        }
                    }
                    if (_npcDialogue[_dialogueIndex]._advanceDialogue)
                    {
                        _dialogueIndex++;
                        //_currentCardHolder._network [_currentBio]._verbalIndex++; //update audience card
                        _currentCardHolder._wRControl._runtimeReferences._runtimeNetwork[_currentBio]._verbalIndex++;

                        if (_dialogueIndex > (_npcDialogue.Length - 1))
                        {
                            _dialogueIndex = _npcDialogue.Length - 1;
                        }
                    }
                    CloseDialogue();
                }
            }
        }

        IEnumerator ScrollText()
        {
            for (int i = 0; i < _currentLine.Length; i++)
            {
                _dialogueText.text = _currentLine.Substring(0, i);
                if (_testDialoguePutt != null)
                {
                    GetComponent<AudioSource>().PlayOneShot(_testDialoguePutt);
                }
                yield return new WaitForSeconds(_letterTimings[i]);
            }
            _dialogueText.text = _currentLine;

            _lineCompleted = true;

            _buttonContinue.OpenHUDPrompt(AllEnums.PromptType.Continue);
        }

        /// <summary>
        /// For better sentence flow, certain characters are given a delay when they appear on screen.
        /// </summary>
        /// <returns>A list of each character delay times (a float value) within a char array.</returns>
        /// <param name="fromString">From string, passed through as a char[] array.</param>
        public List<float> GetCharacterTime(char[] fromString)
        {
            List<float> returnValue = new List<float>();
            for (int i = 0; i < fromString.Length; i++)
            {
                if (string.Equals(fromString[i].ToString(), ".") || string.Equals(fromString[i].ToString(), "-"))
                {
                    returnValue.Add(_pauseTime);
                }
                else if (string.Equals(fromString[i].ToString(), ",") || string.Equals(fromString[i].ToString(), "!"))
                {
                    returnValue.Add(_pauseTime / 2f); //half the pause time seems to work
                }
                else if (string.Equals(fromString[i].ToString(), "?"))
                {
                    returnValue.Add(_pauseTime / 3f); //third the pause time seems to work
                }
                else
                {
                    returnValue.Add(_secondsPerLine);
                }
            }
            return returnValue;
        }

        public void CloseDialogue()
        {
            _currentCardHolder._states._isInConversation = false;

            //test
            CharacterStateController source = _origin.GetComponent<CharacterStateController>();
            source._states._isInConversation = false;
            GetComponent<AudioSource>().Stop();
            //...

            _dialogueActive = false;
            _overlay.SetActive(false);
            _buttonContinue.CloseHUDPrompt();
            _buttonSkip.CloseHUDPrompt();
        }


    }
}