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
				doEnemyLimitBoost = ChunkyRunInfo.instance.doEnemyLimitBoost,
				doHealingBuffs = ChunkyRunInfo.instance.doHealingBuffs,
				doGoldPenalty = ChunkyRunInfo.instance.doGoldPenalty,
				doEnemyNerfs = ChunkyRunInfo.instance.doEnemyNerfs,
				enemyChanceToYap = ChunkyRunInfo.instance.enemyChanceToYap,
				enemyYapCooldown = ChunkyRunInfo.instance.enemyYapCooldown,
				limitPest = ChunkyRunInfo.instance.limitPest,
				limitPestAmount = ChunkyRunInfo.instance.limitPestAmount,
				rexHealOverride = ChunkyRunInfo.instance.rexHealOverride,
				acridHealOverride = ChunkyRunInfo.instance.acridHealOverride,
				chirrHealOverride = ChunkyRunInfo.instance.chirrHealOverride,
				aliemHealOverride = ChunkyRunInfo.instance.aliemHealOverride,
				submarinerHealOverride = ChunkyRunInfo.instance.submarinerHealOverride,
				ravagerHealOverride = ChunkyRunInfo.instance.ravagerHealOverride,
			};

			save.Add("CHUNKYMODE_RunInfo",tempRun);
		}
	}
}}