using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class AllEnums {
	public enum InputControlState { OutGame, Game, InGameMenu };
	public const int InputControlStateNumber = 3;

	public enum InventoryUIState { None, Equipment, Inventory, Overview, Flock, Gestures, Report, Campfire };
	public const int InventoryUINumber = 8;

	public enum OutGameUIState { None, TitleScreen, LoadSaveGame, Options };
	public const int OutGameUINumber = 4;

	public enum OptionsUIState { Gameplay, Video, Audio, Controls };
	public const int OptionsUINumber = 4;

	public enum LoadSaveUIState { LoadGame, SaveGame, DeleteSaves };
	public const int LoadSaveUINumber = 3;

    public enum GameFileState { None, GameFile1, GameFile2, GameFile3 };
    public const int GameFileNumber = 4;

    public enum StoryActs { Prologue, Act1, Act2, Act3, Act4, Act5, Act6, Act7, Epilogue };
    public const int StoryActNumber = 9;

    public enum StoryChapters { Chapter1, Chapter2, Chapter3, Chapter4, Chapter5 };
    public const int StoryChapterNumber = 5;

    public enum StoryScenes { Scene1, Scene2, Scene3 };
    public const int StoryScenesNumber = 3;

	public enum AttributeType { Strength, Endurance, Dexterity, Intelligence, Vitality, Perception, Courage, Luck };
	public const int AttributeNumber = 8;

	public enum StatusType { None, Health, Stamina, Courage, ImmuneSystem };
	public const int StatusTypeNumber = 5;

	public enum Status { Normal, Injured, Dying, Hungry, Thirsty, Tired, Sleeping, Waste, Unhappy, Happy, Breeding, Bleeding, Poisoned, Hot, OnFire, Sick, Diseased, Dizzy, Stunned, Cold, Frozen, Nervous, Scared };
	public const int StatusNumber = 22;

	public enum AttackDefenceType { Physical, Finesse, Magical, Fire, Wind, Light, Water, Earth, Dark };
	public const int AttackDefenceNumber = 9;

	public enum ResistanceType { Bleed, Poison, Heat, Disease, Dizzy, Freeze };
	public const int ResisistancesNumber = 6;

	public enum BodyNeedsType { None, Thirst, Waste, Hunger, Sleep, Pleasure };
	public const int BodyNeedsNumber = 6;

	public enum ItemType { Weapon, Spell, Consumable, Armor };
	public const int ItemTypeNumber = 4;

	public enum ArmorType { Accessory1, Accessory2, Accessory3, LeftArm, Head, RightArm, LeftHand, Torso, RightHand, LeftFoot, Legs, RightFoot, Weapon };
	public const int ArmorTypeNumber = 13;

	public enum WeaponType { Unarmed, Dagger, Staff, Rapier, Sword, Axe, Hammer, Shield, Bow };
	public const int WeaponTypeNumber = 9;

	public enum SpellClass { None, Fire, Water, Wind, Earth, Light, Dark, Magic };
	public const int SpellClassNumber = 8;

	public enum SpellType { None, Projectile, Buff, Looping };
	public const int SpellTypeNumber = 4;

	public enum ActionType { Attack, Block, Spell, Parry };
	public const int ActionTypeNumber = 4;

	public enum ActionInputType { RightWeak, LeftWeak, RightStrong, LeftStrong };
	public const int ActionInputTypeNumber = 4;

	public enum PromptType { Talk, Give, Pickup, Examine, Take, Use, Open, Close, Listen, Continue, Skip };
	public const int PromptNumber = 7;

	public enum InfoFeedType { Acquired, Used, Dropped, Given, Dead, Missing, Killed, Pregnant, Born, Completed, Talked }
	public const int InfoFeedNumber = 10;

	public enum EnemyAIState { Far, Near, InSight, Attacking };
	public const int EnemyAIStateNumber = 4;

    public enum NpcState { Passive, Alert, Aggressive};
    public const int NpcStateNumber = 3;

    public enum NpcAiPattern { Stationary, Flock, Waypoint, BodyNeeds, Roam};
    public const int NpcAiPatternNumber = 5;

	public enum CharacterActionState { Moving, Airborne, Interacting, OverrideActions }; //reconsider
	public const int CharacterActionStateNumber = 4;

	public enum PreferredHand { Right, Left, Ambidextrous };
	public const int PreferredHandNumber = 3;

    public enum TimeOfDay { Midnight, AM1, AM2, AM3, AM4, AM5, Dawn, AM7, AM8, AM9, AM10, AM11, Noon, PM1, PM2, PM3, PM4, PM5, Dusk, PM7, PM8, PM9, PM10, PM11 };
    public const int NumberOfHours = 24;

    public enum WeatherCondition { Clear, Cloudy, Rain, Storm, Snow, Blizzard, Dry, Unknown };
	public const int WeatherConditionNumber = 8;

	//World NPC Biography Enums
	public enum Race { Human, Humanoid, Animal, Monster, Unknown };
	public const int RaceNumber = 5;

	public enum Gender { Male, Female }; //TODO: might want an option to pick randomly between Male and Female
	public const int GenderNumber = 2;

	//public enum Territory { Plains, Forest, Ocean, Desert, Mountains, Jungle, Underground
	//public const int TerritoryNumber = ??;
	public enum Faction { None, Citizen, Saved, Wild, Fallen };
	public const int FactionNumber = 5;

	public enum Class { Barbarian, Runt, Common, Special, Elite };
	public const int ClassNumber = 5;

    //where do these people come from?
    public enum Origin { Unknown, Wilderness, Toriel, Wontin }
    public const int OriginNumber = 3;

	public enum Personality { Abusive, Disrespectful, Sarcastic, Serious, Joking, Sincere, Flirtatious };
	public const int PersonalityNumber = 7;

	public enum FlockAlert { None, Warning, Danger, Missing };
	public const int FlockAlertNumber = 4;
}