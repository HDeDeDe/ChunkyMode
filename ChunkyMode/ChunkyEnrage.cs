using System.Runtime.CompilerServices;

namespace HDeMods {
	public class ChunkyEnrage {
		private static bool? _enabled;

		public static bool enabled {
			get {
				if (_enabled == null) {
					_enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(EnrageArtifact.PluginGUID);
				}
				return (bool)_enabled;
			}
		}
		
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void PerformCrime() {
			Log.Info("Artifact of the Enraged detected, doing a crime.");
			EnrageArtifact.DoACrime();
		}
	}
}