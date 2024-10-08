using System.Collections.Generic;
using ProperSave;
using System.Runtime.CompilerServices;

namespace HDeMods {
	public static class ChunkySaving {
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
			ChunkyRunInfo tempRun = save.GetModdedData<ChunkyRunInfo>("CHUNKYMODE_RunInfo");
			if (tempRun == null) return;
			ChunkyRunInfo.Instance = tempRun;
			ChunkyRunInfo.preSet = true;
		}
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void SaveRunInfo(Dictionary<string, object> save) {
			save.Add("CHUNKYMODE_RunInfo",ChunkyRunInfo.Instance);
		}
	}
}