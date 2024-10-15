using System.Collections.Generic;
using ProperSave;
using System.Runtime.CompilerServices;

namespace HDeMods { namespace ChunkyOptionalMods {
	public static class Saving {
		private static bool? _enabled;

		public static bool enabled {
			get {
				if (_enabled == null) {
					_enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(ProperSavePlugin.GUID);
				}
				return (bool)_enabled;
			}
		}
		
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void SetUp() {
			ProperSave.Loading.OnLoadingStarted += LoadFromSave;
			ProperSave.SaveFile.OnGatherSaveData += SaveRunInfo;
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
		public static void SaveRunInfo(Dictionary<string, object> save) {
			ChunkySaveData tempRun = new ChunkySaveData();
			tempRun.isValidSave = true;
			
			tempRun.doEnemyBoostThisRun =ChunkyRunInfo.instance.doEnemyBoostThisRun;
			tempRun.doHealBuffThisRun = ChunkyRunInfo.instance.doHealBuffThisRun;
			tempRun.doGoldThisRun = ChunkyRunInfo.instance.doGoldThisRun;
			tempRun.doNerfsThisRun = ChunkyRunInfo.instance.doNerfsThisRun;
			tempRun.doLoiterThisRun = ChunkyRunInfo.instance.doLoiterThisRun;
			tempRun.enemyChanceToYapThisRun = ChunkyRunInfo.instance.enemyChanceToYapThisRun;
			tempRun.enemyYapCooldownThisRun = ChunkyRunInfo.instance.enemyYapCooldownThisRun;
			tempRun.loiterPenaltyTimeThisRun = ChunkyRunInfo.instance.loiterPenaltyTimeThisRun;
			tempRun.loiterPenaltyFrequencyThisRun = ChunkyRunInfo.instance.loiterPenaltyFrequencyThisRun;
			tempRun.loiterPenaltySeverityThisRun = ChunkyRunInfo.instance.loiterPenaltySeverityThisRun;
			
			save.Add("CHUNKYMODE_RunInfo",tempRun);
		}
	}
}}