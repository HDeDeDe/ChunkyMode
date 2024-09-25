using RiskOfOptions;
using RiskOfOptions.Options;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using UnityEngine;

namespace ChunkyMode {
	public static class ChunkyModeOptions {
		private static bool? _enabled;

		public static bool enabled {
			get {
				if (_enabled == null) {
					_enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
				}
				return (bool)_enabled;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddCheck(ConfigEntry<bool> option) {
			ModSettingsManager.AddOption(new CheckBoxOption(option));
		}
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void SetSprite(Sprite sprite) {
			ModSettingsManager.SetModIcon(sprite);
		}
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void SetDescription(string description) {
			ModSettingsManager.SetModDescription(description);
		}
	}
}