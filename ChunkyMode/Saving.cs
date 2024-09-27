using System.Collections.Generic;
using ProperSave;
using System.Runtime.CompilerServices;

namespace ChunkyMode {
	public class Saving {
		private static bool? _enabled;

		public static bool enabled {
			get {
				if (_enabled == null) {
					_enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("KingEnderBrine-ProperSave-2.11.1");
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
			RunInfo tempRun = save.GetModdedData<RunInfo>("CHUNKYMODE_RunInfo");
			if (tempRun == null) return;
			RunInfo.Instance = tempRun;
			RunInfo.preSet = true;
		}
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static void SaveRunInfo(Dictionary<string, object> save) {
			save.Add("CHUNKYMODE_RunInfo",RunInfo.Instance);
		}
	}
}