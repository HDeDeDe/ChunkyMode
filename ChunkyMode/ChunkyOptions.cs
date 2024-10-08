using RiskOfOptions;
using RiskOfOptions.Options;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using RiskOfOptions.OptionConfigs;
using UnityEngine;

namespace HDeMods {
	public static class ChunkyOptions {
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
			CheckBoxOption boxOption = new CheckBoxOption(option);
			ModSettingsManager.AddOption(boxOption, ChunkyMode.PluginGUID, ChunkyMode.PluginName);
#if DEBUG
			Log.Info(boxOption.GetNameToken());
			Log.Info(boxOption.GetDescriptionToken());
#endif
		}
		
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddInt(ConfigEntry<int> option) {
			IntSliderOption sliderOption = new IntSliderOption(option, new IntSliderConfig() {min = 0, max = 10000});
			ModSettingsManager.AddOption(sliderOption, ChunkyMode.PluginGUID, ChunkyMode.PluginName);
			
#if DEBUG
			Log.Info(sliderOption.GetNameToken());
			Log.Info(sliderOption.GetDescriptionToken());
#endif
		}
		
		public static void AddFloat(ConfigEntry<float> option) {
			SliderOption sliderOption = new SliderOption(option, new SliderConfig() {min = 0f, max = 600f});
			ModSettingsManager.AddOption(sliderOption, ChunkyMode.PluginGUID, ChunkyMode.PluginName);
			
#if DEBUG
			Log.Info(sliderOption.GetNameToken());
			Log.Info(sliderOption.GetDescriptionToken());
#endif
		}
		
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void SetSprite(Sprite sprite) {
			ModSettingsManager.SetModIcon(sprite);
		}
		
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void SetDescriptionToken(string description) {
			ModSettingsManager.SetModDescriptionToken(description);
		}
	}
}