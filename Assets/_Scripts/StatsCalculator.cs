using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shepherd {
	public static class StatsCalculator {
		public static float CalculateBaseDamage(WorldItemStats wStats, WorldItemStats hitBodyStats, float multiplier = 1f) { //CharacterBody dealtBody) {
			float totalDamage = 0f;

			float rawDmg = ( (wStats._physical * multiplier) - hitBodyStats._physical) + ( (wStats._finesse * multiplier) - hitBodyStats._finesse);

			float magicalDmg = ((wStats._magical * multiplier) - hitBodyStats._magical);

			//water <-> fire cancel each other out, therefore calculate the difference
			float fireDmg = (((wStats._fire * multiplier) - hitBodyStats._fire) <= 0f) ? 0f : ((wStats._fire * multiplier) - hitBodyStats._fire);
			float waterDmg = (((wStats._water * multiplier) - hitBodyStats._water) <= 0f) ? 0f : ((wStats._water * multiplier) - hitBodyStats._water);
			float totalFireWaterDmg = Mathf.Abs (fireDmg - waterDmg);

			//wind <-> earth cancel each other out, therefore calculate the difference
			float windDmg = (((wStats._wind * multiplier) - hitBodyStats._wind) <= 0f) ? 0f : ((wStats._wind * multiplier) - hitBodyStats._wind);
			float earthDmg = (((wStats._earth * multiplier) - hitBodyStats._earth) <= 0f) ? 0f : ((wStats._earth * multiplier) - hitBodyStats._earth);
			float totalWindEarthDmg = Mathf.Abs (windDmg - earthDmg);

			//light <-> dark cancel each other out, therefore calculate the difference
			float lightDmg = (((wStats._light * multiplier) - hitBodyStats._light) <= 0f) ? 0f : ((wStats._light * multiplier) - hitBodyStats._light);
			float darkDmg = (((wStats._dark * multiplier) - hitBodyStats._dark) <= 0f) ? 0f : ((wStats._dark * multiplier) - hitBodyStats._dark);
			float totalLightDarkDmg = Mathf.Abs (lightDmg - darkDmg);

			totalDamage += rawDmg + magicalDmg + totalFireWaterDmg + totalWindEarthDmg + totalLightDarkDmg;

			//chance for the weapon to do critcal damage based off of luck... about 150%
			int _rollWeapon = Random.Range (1, 101);
			//Debug.Log ("Weapon Goal: " + wStats._luck + "/100\nGot: " + _rollWeapon + "/100");  
			if(_rollWeapon < wStats._luckDmg){
				totalDamage *= 1.5f;
			}

			//chance for the entity hit to get critical armour based off of luck... about 150%
			int _rollPlayer = Random.Range (1, 101);
			//Debug.Log ("Armour Goal: " + hitBody._luck + "/100\nGot: " + _rollPlayer + "/100");
			if (_rollPlayer < hitBodyStats._luck) {
				totalDamage /= 1.5f;
			}

			//do at least one damage if everything is protected
			if (totalDamage <= 0f) {
				Debug.Log ("Protected");
				totalDamage = 1f;
			}

			return totalDamage;
		}

		public static float CalculateImmunityDamage(WorldItemStats wStats, WorldItemStats hitBodyStats) {
			float totalDamage = 0f;
			float bleed = hitBodyStats._bleed - wStats._bleed;
			if (bleed > 0f) {
				bleed = 0f;
			}
			float poison = hitBodyStats._poison - wStats._poison;
			if (poison > 0f) {
				poison = 0f;
			}

			//cold damage cancels out heat damage, so a weapon doesn't benefit from having both heat/cold damage!
			float heat = hitBodyStats._heat - (wStats._heat - wStats._freeze);
			if (heat > 0f) {
				heat = 0f;
			}

			//heat damage cancels out cold damage, so a weapon doesn't benefit from having both heat/cold damage!
			float cold = hitBodyStats._freeze - (wStats._freeze - wStats._heat);
			if (cold > 0f) {
				cold = 0f;
			}

			float disease = hitBodyStats._disease - wStats._disease;
			if (disease > 0f) {
				disease = 0f;
			}

			float dizzy = hitBodyStats._dizzy - wStats._dizzy;
			if (dizzy > 0f) {
				dizzy = 0f;
			}

			totalDamage += Mathf.Abs(bleed + poison + heat + cold + disease + dizzy);
			if (totalDamage <= 0f) {
				Debug.Log ("Immune");
				totalDamage = 0f;
			}
				
			return totalDamage;
		}

		public static float CalculateRawDamageOfWeapon(WorldItemStats wStats) {
			return (wStats._physical + wStats._finesse + wStats._magical);
		}
	}
}