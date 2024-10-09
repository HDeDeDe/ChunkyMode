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
			ChunkyCheckBoxOption boxOption = new ChunkyCheckBoxOption(option);
			ModSettingsManager.AddOption(boxOption, ChunkyMode.PluginGUID, ChunkyMode.PluginName);
#if DEBUG
			Log.Info(boxOption.GetNameToken());
			Log.Info(boxOption.GetDescriptionToken());
#endif
		}
		
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddInt(ConfigEntry<int> option, int minimum, int maximum) {
			ChunkyIntSliderOption sliderOption = new ChunkyIntSliderOption(option, new IntSliderConfig() {min = minimum, max = maximum});
			ModSettingsManager.AddOption(sliderOption, ChunkyMode.PluginGUID, ChunkyMode.PluginName);
			
#if DEBUG
			Log.Info(sliderOption.GetNameToken());
			Log.Info(sliderOption.GetDescriptionToken());
#endif
		}
		
		public static void AddFloat(ConfigEntry<float> option, float minimum, float maximum) {
			ChunkySliderOption sliderOption = new ChunkySliderOption(option, new SliderConfig() {min = minimum, max = maximum});
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

	public class ChunkyCheckBoxOption : CheckBoxOption {
		public ChunkyCheckBoxOption(ConfigEntry<bool> configEntry) : base(configEntry)
		{
		}

		public override void RegisterTokens()
		{
		}
	}
	
	public class ChunkyIntSliderOption : IntSliderOption {

		public ChunkyIntSliderOption(ConfigEntry<int> configEntry, IntSliderConfig config) : base(configEntry, config)
		{
		}

		public override void RegisterTokens()
		{
		}
	}
	
	public class ChunkySliderOption : SliderOption {

		public ChunkySliderOption(ConfigEntry<float> configEntry, SliderConfig config) : base(configEntry, config)
		{
		}

		public override void RegisterTokens()
		{
		}
	}
}