using RoR2;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace HDeMods {
	internal static class ChunkySimulacrum {
		public static void OnAllEnemiesDefeatedServer(InfiniteTowerWaveController waveController) {
			ChunkyMode.waveStarted = false;
		}
		public static void InfiniteTowerRun_BeginNextWave(On.RoR2.InfiniteTowerRun.orig_BeginNextWave beginNextWave, InfiniteTowerRun self) {
			ChunkyMode.waveStarted = true;
			beginNextWave(self);
		}
		/*public static void InfiniteTowerWaveController_Initialize(ILContext il) {
			ILCursor c = new ILCursor(il);
			if (!c.TryGotoNext(moveType: MoveType.After,
			    x => x.MatchLdfld<Run>("difficultyCoefficient"))
			    ) {
				CM.Log.Fatal("Failed to hook into InfiniteTowerWaveController.Initialize!");
				return;
			}

			c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<float, float>>(b => b * 1.1f);
		}*/

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

		/*public static void CombatDirector_PrepareNewMonsterWave(On.RoR2.CombatDirector.orig_PrepareNewMonsterWave prep,
			CombatDirector self, DirectorCard card) {
			Log.Debug("New Wave starting!");
			prep(self, card);
		}*/
	}
}