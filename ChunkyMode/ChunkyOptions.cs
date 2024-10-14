using RiskOfOptions;
using RiskOfOptions.Options;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using RiskOfOptions.OptionConfigs;
using UnityEngine;

namespace HDeMods { namespace ChunkyOptionalMods {
	internal static class RoO {
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

	// Thanks to Bubbet for the suggestion to do this
	internal class ChunkyCheckBoxOption : CheckBoxOption {
		public ChunkyCheckBoxOption(ConfigEntry<bool> configEntry) : base(configEntry)
		{
		}

		public override void RegisterTokens()
		{
		}

        /*public new string Description {
			get => RoR2.Language.GetString(GetDescriptionToken());
			set => Log.Warning("Risk Of Options attempted to write the following to " + GetDescriptionToken() + ":\n" + value);
		}

        public override void SetDescription(string fallback, BaseOptionConfig config) {
	        if (!string.IsNullOrEmpty(config.description))
		        Description = config.description;
	        else
		        Description = fallback;
        }*/
	}
	
	internal class ChunkyIntSliderOption : IntSliderOption {

		public ChunkyIntSliderOption(ConfigEntry<int> configEntry, IntSliderConfig config) : base(configEntry, config)
		{
		}

		public override void RegisterTokens()
		{
		}
		
		/*public new string Description {
			get => RoR2.Language.GetString(GetDescriptionToken());
			set => Log.Warning("Risk Of Options attempted to write the following to " + GetDescriptionToken() + ":\n" + value);
		}
		
		public override void SetDescription(string fallback, BaseOptionConfig config) {
			if (!string.IsNullOrEmpty(config.description))
				Description = config.description;
			else
				Description = fallback;
		}*/
	}
	
	internal class ChunkySliderOption : SliderOption {

		public ChunkySliderOption(ConfigEntry<float> configEntry, SliderConfig config) : base(configEntry, config)
		{
		}

		public override void RegisterTokens()
		{
		}
		
		/*public new string Description {
			get => RoR2.Language.GetString(GetDescriptionToken());
			set => Log.Warning("Risk Of Options attempted to write the following to " + GetDescriptionToken() + ":\n" + value);
		}
		
		public override void SetDescription(string fallback, BaseOptionConfig config) {
			if (!string.IsNullOrEmpty(config.description))
				Description = config.description;
			else
				Description = fallback;
		}*/
	}
}}