using System.Collections.Generic;
using ProperSave;
using System.Runtime.CompilerServices;
using RoR2;

namespace HDeMods { namespace ChunkyOptionalMods {
	public static class Saving {
		public static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(ProperSavePlugin.GUID);
		
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void SetUp() {
			Loading.OnLoadingStarted += LoadFromSave;
			SaveFile.OnGatherSaveData += SaveRunInfo;
		}
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static void LoadFromSave(SaveFile save) {
			if (!Loading.IsLoading) return;
			
			if (save.ModdedData.TryGetValue("CHUNKYMODE_RunInfo", out ProperSave.Data.ModdedData rawData) &&
			    rawData?.Value is ChunkySaveData saveData && saveData.isValidSave) {
				ChunkyRunInfo.saveData = saveData;
				ChunkyRunInfo.preSet = true;
				return;
			}
			
			CM.Log.Warning("Chunky RunInfo not present, skipping step.");
		}
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static void SaveRunInfo(Dictionary<string, object> save) {
			if (Run.instance.selectedDifficulty != ChunkyMode.LegacyDifficultyIndex) return;
			ChunkySaveData tempRun = new ChunkySaveData {
				isValidSave = true,
				doEnemyBoostThisRun = ChunkyRunInfo.instance.doEnemyBoostThisRun,
				doHealBuffThisRun = ChunkyRunInfo.instance.doHealBuffThisRun,
				doGoldThisRun = ChunkyRunInfo.instance.doGoldThisRun,
				doNerfsThisRun = ChunkyRunInfo.instance.doNerfsThisRun,
				enemyChanceToYapThisRun = ChunkyRunInfo.instance.enemyChanceToYapThisRun,
				enemyYapCooldownThisRun = ChunkyRunInfo.instance.enemyYapCooldownThisRun,
				limitPestsThisRun = ChunkyRunInfo.instance.limitPestsThisRun,
				limitPestsAmountThisRun = ChunkyRunInfo.instance.limitPestsAmountThisRun
			};

			save.Add("CHUNKYMODE_RunInfo",tempRun);
		}
	}
}}