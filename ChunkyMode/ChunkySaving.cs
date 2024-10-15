using System.Collections.Generic;
using ProperSave;
using System.Runtime.CompilerServices;

namespace HDeMods { namespace ChunkyOptionalMods {
	public static class Saving {
		private static bool? enabled;

		public static bool Enabled {
			get {
				if (enabled == null) {
					enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(ProperSavePlugin.GUID);
				}
				return (bool)enabled;
			}
		}
		
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void SetUp() {
			Loading.OnLoadingStarted += LoadFromSave;
			SaveFile.OnGatherSaveData += SaveRunInfo;
		}
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static void LoadFromSave(SaveFile save) {
			if (!Loading.IsLoading) return;
			ChunkySaveData tempRun = save.GetModdedData<ChunkySaveData>("CHUNKYMODE_RunInfo");
			if (!tempRun.isValidSave) return;
			
			ChunkyRunInfo.instance.doEnemyBoostThisRun = tempRun.doEnemyBoostThisRun;
			ChunkyRunInfo.instance.doHealBuffThisRun = tempRun.doHealBuffThisRun;
			ChunkyRunInfo.instance.doGoldThisRun = tempRun.doGoldThisRun;
			ChunkyRunInfo.instance.doNerfsThisRun = tempRun.doNerfsThisRun;
			ChunkyRunInfo.instance.doLoiterThisRun = tempRun.doLoiterThisRun;
			ChunkyRunInfo.instance.enemyChanceToYapThisRun = tempRun.enemyChanceToYapThisRun;
			ChunkyRunInfo.instance.enemyYapCooldownThisRun = tempRun.enemyYapCooldownThisRun;
			ChunkyRunInfo.instance.loiterPenaltyTimeThisRun = tempRun.loiterPenaltyTimeThisRun;
			ChunkyRunInfo.instance.loiterPenaltyFrequencyThisRun = tempRun.loiterPenaltyFrequencyThisRun;
			ChunkyRunInfo.instance.loiterPenaltySeverityThisRun = tempRun.loiterPenaltySeverityThisRun;
			
			ChunkyRunInfo.preSet = true;
		}
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static void SaveRunInfo(Dictionary<string, object> save) {
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
				loiterPenaltySeverityThisRun = ChunkyRunInfo.instance.loiterPenaltySeverityThisRun
			};

			save.Add("CHUNKYMODE_RunInfo",tempRun);
		}
	}
}}