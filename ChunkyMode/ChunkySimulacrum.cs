using System.Diagnostics.CodeAnalysis;
using RoR2;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace HDeMods {
	internal static class ChunkySimulacrum {
		private static bool rngExtracted = false;
		private static Xoroshiro128Plus rng;
		
		public static void OnAllEnemiesDefeatedServer(InfiniteTowerWaveController waveController) {
			ChunkyMode.waveStarted = false;
		}
		public static void InfiniteTowerRun_BeginNextWave(On.RoR2.InfiniteTowerRun.orig_BeginNextWave beginNextWave, InfiniteTowerRun self) {
			ChunkyMode.waveStarted = true;
			beginNextWave(self);
		}
		public static void InfiniteTowerWaveController_Initialize(ILContext il) {
			ILCursor c = new ILCursor(il);
			if (!c.TryGotoNext(moveType: MoveType.After,
			    x => x.MatchLdfld<Run>("difficultyCoefficient"))
			    ) {
				CM.Log.Fatal("Failed to hook into InfiniteTowerWaveController.Initialize!");
				return;
			}

			c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, float>>(b => b * 1.1f);
		}

		public static void InfiniteTowerWaveController_FixedUpdate(ILContext il) {
			ILCursor c = new ILCursor(il);
			if (!c.TryGotoNext(moveType: MoveType.After,
				    x => x.MatchLdfld<InfiniteTowerWaveController>("maxSquadSize"))
			   ) {
				CM.Log.Fatal("Failed to hook into InfiniteTowerWaveController.FixedUpdate!");
				return;
			}

			c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<int, int>>(b => {
				if (!ChunkyRunInfo.instance.doEnemyBoostThisRun) return b;
				return (int)(b * 1.5f);
			});
		}

		[SuppressMessage("ReSharper", "InconsistentNaming")]
		public static void ExtractRNGFromCombatDirector(ILContext il) {
			ILCursor c = new ILCursor(il);
			if (!c.TryGotoNext(moveType: MoveType.After,
				    x => x.MatchLdfld<CombatDirector>("rng")
			    )) {
				CM.Log.Error("Could not get RNG! Uninstalling hook!");
				IL.RoR2.CombatDirector.AttemptSpawnOnTarget -= ExtractRNGFromCombatDirector;
				rngExtracted = false;
				return;
			}

			c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<Xoroshiro128Plus, Xoroshiro128Plus>>(
				rngThing => {
					rng = rngThing;
					rngExtracted = true;
					return rngThing;
				});
		}

		public static void CombatDirector_PrepareNewMonsterWave(On.RoR2.CombatDirector.orig_PrepareNewMonsterWave prep,
			CombatDirector self, DirectorCard card) {
			if (!ChunkyRunInfo.instance.limitPestsThisRun || !rngExtracted) {
				prep(self, card);
				return;
			}

			GameObject blindPest = BodyCatalog.GetBodyPrefab(ChunkyCachedIndexes.bodyCache[BodyCache.FlyingVermin]);
			GameObject lemurian = BodyCatalog.GetBodyPrefab(ChunkyCachedIndexes.bodyCache[BodyCache.Lemurian]);
			
			for (GameObject desiredMonster = card.spawnCard.prefab.GetComponent<CharacterMaster>().bodyPrefab;
			     desiredMonster == blindPest || desiredMonster == lemurian;
			     desiredMonster = card.spawnCard.prefab.GetComponent<CharacterMaster>().bodyPrefab)
			{
#if DEBUG
				CM.Log.Warning(desiredMonster  + " detected, checking if we have too many.");
#endif
				int totalEnemies = 0;
				totalEnemies += TeamComponent.GetTeamMembers(TeamIndex.Monster).Count;
				totalEnemies += TeamComponent.GetTeamMembers(TeamIndex.Void).Count;
				totalEnemies += TeamComponent.GetTeamMembers(TeamIndex.Lunar).Count;
				
				int tenPercent =
					(int)(totalEnemies * (ChunkyRunInfo.instance.limitPestsAmountThisRun / 100f));
				
#if DEBUG
				CM.Log.Warning("Total enemies: " + totalEnemies);
				CM.Log.Warning("Too many Pest? " + (ChunkyMode.totalBlindPest > tenPercent));
				CM.Log.Warning("Too many Lemurians? " + (ChunkyMode.totalLemurians > totalEnemies * tenPercent));
#endif
				
				// Something's wrong
				if ((ChunkyMode.totalBlindPest > tenPercent && desiredMonster == blindPest) || 
				    (ChunkyMode.totalLemurians > tenPercent && desiredMonster == lemurian)) {
					CM.Log.Warning("Too many bastards. Generating new director card.");
					card = GenerateNewDirectorCard();
					continue;
				}

				break;
			}
			prep(self, card);
		}

		private static DirectorCard GenerateNewDirectorCard() {
			return ClassicStageInfo.instance.monsterSelection.Evaluate(rng.nextNormalizedFloat);
		}
	}
}