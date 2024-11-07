using RiskOfOptions;
using RiskOfOptions.Options;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using RiskOfOptions.OptionConfigs;
using UnityEngine;

namespace HDeMods { namespace ChunkyOptionalMods {
	internal static class RoO {
		public static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddCheck(ConfigEntry<bool> option) {
			ChunkyCheckBoxOption boxOption = new ChunkyCheckBoxOption(option);
			ModSettingsManager.AddOption(boxOption, ChunkyModePlugin.PluginGUID, ChunkyModePlugin.PluginName);
#if DEBUG
			CM.Log.Info(boxOption.GetNameToken());
			CM.Log.Info(boxOption.GetDescriptionToken());
#endif
		}
		
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddInt(ConfigEntry<int> option, int minimum, int maximum) {
			ChunkyIntSliderOption sliderOption = new ChunkyIntSliderOption(option, new IntSliderConfig() {min = minimum, max = maximum});
			ModSettingsManager.AddOption(sliderOption, ChunkyModePlugin.PluginGUID, ChunkyModePlugin.PluginName);
			
#if DEBUG
			CM.Log.Info(sliderOption.GetNameToken());
			CM.Log.Info(sliderOption.GetDescriptionToken());
#endif
		}
		
		public static void AddFloat(ConfigEntry<float> option, float minimum, float maximum, string format = "{0:0}%") {
			ChunkySliderOption sliderOption = new ChunkySliderOption(option, new SliderConfig() {min = minimum, max = maximum, FormatString = format});
			ModSettingsManager.AddOption(sliderOption, ChunkyModePlugin.PluginGUID, ChunkyModePlugin.PluginName);
			
#if DEBUG
			CM.Log.Info(sliderOption.GetNameToken());
			CM.Log.Info(sliderOption.GetDescriptionToken());
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
		public ChunkyCheckBoxOption(ConfigEntry<bool> configEntry) : base(configEntry) {
			RoR2.Language.onCurrentLanguageChanged+= ResetDescription;
		}

		public override void RegisterTokens() {
			Description = RoR2.Language.GetString(GetDescriptionToken());
		}

		private void ResetDescription() {
			Description = RoR2.Language.GetString(GetDescriptionToken());
		}
	}
	
	internal class ChunkyIntSliderOption : IntSliderOption {

		public ChunkyIntSliderOption(ConfigEntry<int> configEntry, IntSliderConfig config) : base(configEntry, config) {
			RoR2.Language.onCurrentLanguageChanged+= ResetDescription;
		}

		public override void RegisterTokens() {
			Description = RoR2.Language.GetString(GetDescriptionToken());
		}
		
		private void ResetDescription() {
			Description = RoR2.Language.GetString(GetDescriptionToken());
		}
	}
	
	internal class ChunkySliderOption : SliderOption {

		public ChunkySliderOption(ConfigEntry<float> configEntry, SliderConfig config) : base(configEntry, config) {
			RoR2.Language.onCurrentLanguageChanged+= ResetDescription;
		}

		public override void RegisterTokens() {
			Description = RoR2.Language.GetString(GetDescriptionToken());
		}
		
		private void ResetDescription() {
			Description = RoR2.Language.GetString(GetDescriptionToken());
		}
	}
}}