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
			
			ChunkyRunInfo.Instance.doEnemyBoostThisRun = tempRun.doEnemyBoostThisRun;
			ChunkyRunInfo.Instance.doHealBuffThisRun = tempRun.doHealBuffThisRun;
			ChunkyRunInfo.Instance.doGoldThisRun = tempRun.doGoldThisRun;
			ChunkyRunInfo.Instance.doNerfsThisRun = tempRun.doNerfsThisRun;
			ChunkyRunInfo.Instance.doLoiterThisRun = tempRun.doLoiterThisRun;
			ChunkyRunInfo.Instance.enemyChanceToYapThisRun = tempRun.enemyChanceToYapThisRun;
			ChunkyRunInfo.Instance.enemyYapCooldownThisRun = tempRun.enemyYapCooldownThisRun;
			ChunkyRunInfo.Instance.loiterPenaltyTimeThisRun = tempRun.loiterPenaltyTimeThisRun;
			ChunkyRunInfo.Instance.loiterPenaltyFrequencyThisRun = tempRun.loiterPenaltyFrequencyThisRun;
			ChunkyRunInfo.Instance.loiterPenaltySeverityThisRun = tempRun.loiterPenaltySeverityThisRun;
			
			ChunkyRunInfo.preSet = true;
		}
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void SaveRunInfo(Dictionary<string, object> save) {
			ChunkySaveData tempRun = new ChunkySaveData();
			tempRun.isValidSave = true;
			
			tempRun.doEnemyBoostThisRun =ChunkyRunInfo.Instance.doEnemyBoostThisRun;
			tempRun.doHealBuffThisRun = ChunkyRunInfo.Instance.doHealBuffThisRun;
			tempRun.doGoldThisRun = ChunkyRunInfo.Instance.doGoldThisRun;
			tempRun.doNerfsThisRun = ChunkyRunInfo.Instance.doNerfsThisRun;
			tempRun.doLoiterThisRun = ChunkyRunInfo.Instance.doLoiterThisRun;
			tempRun.enemyChanceToYapThisRun = ChunkyRunInfo.Instance.enemyChanceToYapThisRun;
			tempRun.enemyYapCooldownThisRun = ChunkyRunInfo.Instance.enemyYapCooldownThisRun;
			tempRun.loiterPenaltyTimeThisRun = ChunkyRunInfo.Instance.loiterPenaltyTimeThisRun;
			tempRun.loiterPenaltyFrequencyThisRun = ChunkyRunInfo.Instance.loiterPenaltyFrequencyThisRun;
			tempRun.loiterPenaltySeverityThisRun = ChunkyRunInfo.Instance.loiterPenaltySeverityThisRun;
			
			save.Add("CHUNKYMODE_RunInfo",tempRun);
		}
	}
}}