using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputStrings {
	/*CONTROLS*/
	public static string Vertical = "Vertical";
	public static string Horizontal = "Horizontal";

	public static string MoveLeft = "Move Left";
	public static string MoveRight = "Move Right";
	public static string MoveForward = "Move Forward";
	public static string MoveBackward = "Move Backward";

	public static string Jump = "Jump";
	public static string Sprint = "Sprint";
	public static string UseItem = "Use Item";
	public static string DodgeCancel = "Dodge/Cancel";
	public static string InteractConfirm = "Interact/Confirm";
	public static string Interact = "Interact";
	public static string Confirm = "Confirm";
	public static string Dodge = "Dodge";
	public static string Cancel = "Cancel";

	public static string LockOn = "LockOn";
	public static string LockOnLeft = "Cycle LockOn Left";
	public static string LockOnRight = "Cycle LockOn Right";
    public static string LockOnAxis = "Mouse ScrollWheel";

    public static string CameraVertical = "Mouse Y";
	public static string CameraHorizontal = "Mouse X";

	public static string WeakAttackLeft = "Weak Attack Left";
	public static string WeakAttackRight = "Weak Attack Right";
	public static string StrongAttackLeft = "StrongAttackLeft";
	public static string StrongAttackRight = "StrongAttackRight";
	public static string StrongAttackKeyboard = "Strong Attack Modifier";

	public static string PauseMenu = "PauseMenu";
	public static string InGameMenu = "InGameMenu";

	public static string ItemSpellCycle = "Items/Spells"; //d-pad up + d-pad down
	public static string CycleItems = "Cycle Items";
	public static string CycleCCommands = "Cycle Companion Commands";
	public static string ShortcutItem1 = "ShortcutItem 1";
	public static string ShortcutItem2 = "ShortcutItem 2";
	public static string ShortcutItem3 = "ShortcutItem 3";
	public static string ShortcutItem4 = "ShortcutItem 4";
	public static string ShortcutItem5 = "ShortcutItem 5";
	public static string ShortcutItem6 = "ShortcutItem 6";
	public static string ShortcutItem7 = "ShortcutItem 7";
	public static string ShortcutItem8 = "ShortcutItem 8";
	public static string ShortcutItem9 = "ShortcutItem 9";
	public static string ShortcutItem10 = "ShortcutItem 10";

	public static string HandSlotsCycle = "RightHand/LeftHand"; //d-pad right + d-pad left
	public static string CycleRightHand = "Cycle Right-Hand";
	public static string CycleLeftHand = "Cycle Left-Hand";

	/*Game Tags*/
	public static string TagCamera = "StupidCameras";
	public static string TagCameraTarget = "StupidCameraLookat";

	//input prompts
	public static string HoldPrompt = "(Hold)";

    //data functions
    public static string GameSaveFolder = "/GameSaves";
    public static string GameFile1 = "/GameFile1";
    public static string GameFile2 = "/GameFile2";
    public static string GameFile3 = "/GameFile3";
    public static string SaveSlot = "/SaveSlot";
    public static string SaveFormat = ".dat";
    public static string itemFolder = "/Items";

	public static string SaveLocation() {
		string returnValue = Application.streamingAssetsPath;
		if(!Directory.Exists(returnValue)) {
			Directory.CreateDirectory (returnValue);
		}
		return returnValue;
	}
}