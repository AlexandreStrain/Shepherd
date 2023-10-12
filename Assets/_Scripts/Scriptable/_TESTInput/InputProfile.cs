using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shepherd/Single Instances/Input/Input Profile")]
public class InputProfile : ScriptableObject {

	public List<GameInputKeys> _keyboardInput;
	public List<GameInputKeys> _gamepadInput;
}