using System.Collections.Generic;
using ProperSave;
using System.Runtime.CompilerServices;
using RoR2;

namespace HDeMods { namespace HurricaneOptionalMods {
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
			
			if (save.ModdedData.TryGetValue("CHUNKYMODE_RunInfo", out ProperSave.Data.ModdedData rawDataLegacy) &&
			    rawDataLegacy?.Value is HurricaneSaveData saveDataLegacy && saveDataLegacy.isValidSave) {
				HurricaneRunInfo.saveData = saveDataLegacy;
				HurricaneRunInfo.preSet = true;
				return;
			}
			
			if (save.ModdedData.TryGetValue("HURRICANE_RunInfo", out ProperSave.Data.ModdedData rawData) &&
			    rawData?.Value is HurricaneSaveData saveData && saveData.isValidSave) {
				HurricaneRunInfo.saveData = saveData;
				HurricaneRunInfo.preSet = true;
				return;
			}
			
			CM.Log.Warning("CHUNKYMODE_RunInfo not present, skipping step.");
		}
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static void SaveRunInfo(Dictionary<string, object> save) {
			if (Run.instance.selectedDifficulty != Hurricane.LegacyDifficultyIndex) return;
			HurricaneSaveData tempRun = new HurricaneSaveData {
				isValidSave = true,
				doEnemyLimitBoost = HurricaneRunInfo.instance.doEnemyLimitBoost,
				doHealingBuffs = HurricaneRunInfo.instance.doHealingBuffs,
				doGoldPenalty = HurricaneRunInfo.instance.doGoldPenalty,
				doEnemyNerfs = HurricaneRunInfo.instance.doEnemyNerfs,
				enemyChanceToYap = HurricaneRunInfo.instance.enemyChanceToYap,
				enemyYapCooldown = HurricaneRunInfo.instance.enemyYapCooldown,
				limitPest = HurricaneRunInfo.instance.limitPest,
				limitPestAmount = HurricaneRunInfo.instance.limitPestAmount,
				rexHealOverride = HurricaneRunInfo.instance.rexHealOverride,
				acridHealOverride = HurricaneRunInfo.instance.acridHealOverride,
				captainHealOverride = HurricaneRunInfo.instance.captainHealOverride,
				voidFiendHealOverride = HurricaneRunInfo.instance.voidFiendHealOverride,
				seekerHealOverride = HurricaneRunInfo.instance.seekerHealOverride,
				falseSonHealOverride = HurricaneRunInfo.instance.falseSonHealOverride,
				chefSotSHealOverride = HurricaneRunInfo.instance.chefSotSHealOverride,
				chirrHealOverride = HurricaneRunInfo.instance.chirrHealOverride,
				aliemHealOverride = HurricaneRunInfo.instance.aliemHealOverride,
				submarinerHealOverride = HurricaneRunInfo.instance.submarinerHealOverride,
				ravagerHealOverride = HurricaneRunInfo.instance.ravagerHealOverride,
			};

			save.Add("CHUNKYMODE_RunInfo",tempRun);
			//save.Add("HURRICANE_RunInfo",tempRun);
		}
	}
}}