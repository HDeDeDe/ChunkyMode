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
			}
			
			CM.Log.Warning("Chunky RunInfo not present, skipping step.");
		}
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static void SaveRunInfo(Dictionary<string, object> save) {
			if (Run.instance.selectedDifficulty != ChunkyMode.ChunkyModeDifficultyIndex) return;
			ChunkySaveData tempRun = new ChunkySaveData {
				isValidSave = true,
				doEnemyBoostThisRun = ChunkyRunInfo.instance.doEnemyBoostThisRun,
				doHealBuffThisRun = ChunkyRunInfo.instance.doHealBuffThisRun,
				doGoldThisRun = ChunkyRunInfo.instance.doGoldThisRun,
				doNerfsThisRun = ChunkyRunInfo.instance.doNerfsThisRun,
				doLoiterThisRun = ChunkyRunInfo.instance.doLoiterThisRun,
				enemyChanceToYapThisRun = ChunkyRunInfo.instance.enemyChanceToYapThisRun,
				enemyYapCooldownThisRun = ChunkyRunInfo.instance.enemyYapCooldownThisRun,
				loiterPenaltyTimeThisRun = ChunkyRunInfo.instance.loiterPenaltyTimeThisRun,
				loiterPenaltyFrequencyThisRun = ChunkyRunInfo.instance.loiterPenaltyFrequencyThisRun,
				loiterPenaltySeverityThisRun = ChunkyRunInfo.instance.loiterPenaltySeverityThisRun,
				experimentCursePenaltyThisRun = ChunkyRunInfo.instance.experimentCursePenaltyThisRun,
				experimentCurseRateThisRun = ChunkyRunInfo.instance.experimentCurseRateThisRun,
				experimentLimitPestsThisRun = ChunkyRunInfo.instance.experimentLimitPestsThisRun,
				experimentLimitPestsAmountThisRun = ChunkyRunInfo.instance.experimentLimitPestsAmountThisRun
			};

			save.Add("CHUNKYMODE_RunInfo",tempRun);
		}
	}
}}