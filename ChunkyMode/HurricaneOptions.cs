using RiskOfOptions;
using RiskOfOptions.Options;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using RiskOfOptions.OptionConfigs;
using UnityEngine;

namespace HDeMods { namespace HurricaneOptionalMods {
	internal static class RoO {
		public static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddCheck(ConfigEntry<bool> option) {
			HurricaneCheckBoxOption boxOption = new HurricaneCheckBoxOption(option);
			ModSettingsManager.AddOption(boxOption, HurricanePlugin.PluginGUID, HurricanePlugin.PluginName);
#if DEBUG
			CM.Log.Debug(boxOption.GetNameToken());
			CM.Log.Debug(boxOption.GetDescriptionToken());
#endif
		}
		
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddInt(ConfigEntry<int> option, int minimum, int maximum) {
			HurricaneIntSliderOption sliderOption = new HurricaneIntSliderOption(option, new IntSliderConfig() {min = minimum, max = maximum});
			ModSettingsManager.AddOption(sliderOption, HurricanePlugin.PluginGUID, HurricanePlugin.PluginName);
			
#if DEBUG
			CM.Log.Debug(sliderOption.GetNameToken());
			CM.Log.Debug(sliderOption.GetDescriptionToken());
#endif
		}
		
		public static void AddFloat(ConfigEntry<float> option, float minimum, float maximum, string format = "{0:0}%") {
			HurricaneSliderOption sliderOption = new HurricaneSliderOption(option, new SliderConfig() {min = minimum, max = maximum, FormatString = format});
			ModSettingsManager.AddOption(sliderOption, HurricanePlugin.PluginGUID, HurricanePlugin.PluginName);
			
#if DEBUG
			CM.Log.Debug(sliderOption.GetNameToken());
			CM.Log.Debug(sliderOption.GetDescriptionToken());
#endif
		}
		
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddFloatStep(ConfigEntry<float> option, float minimum, float maximum, float step,
			string format = "{0:0}%") {
			HurricaneStepSliderOption stepSliderOption = new HurricaneStepSliderOption(option, new StepSliderConfig()
				{ min = minimum, max = maximum, FormatString = format, increment = step });
			ModSettingsManager.AddOption(stepSliderOption, HurricanePlugin.PluginGUID,
				HurricanePlugin.PluginName);

#if DEBUG
                CM.Log.Debug(stepSliderOption.GetNameToken());
                CM.Log.Debug(stepSliderOption.GetDescriptionToken());
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
	internal class HurricaneCheckBoxOption : CheckBoxOption {
		public HurricaneCheckBoxOption(ConfigEntry<bool> configEntry) : base(configEntry) {
			RoR2.Language.onCurrentLanguageChanged+= ResetDescription;
		}

		public override void RegisterTokens() {
			Description = RoR2.Language.GetString(GetDescriptionToken());
		}

		private void ResetDescription() {
			Description = RoR2.Language.GetString(GetDescriptionToken());
		}
	}
	
	internal class HurricaneIntSliderOption : IntSliderOption {

		public HurricaneIntSliderOption(ConfigEntry<int> configEntry, IntSliderConfig config) : base(configEntry, config) {
			RoR2.Language.onCurrentLanguageChanged+= ResetDescription;
		}

		public override void RegisterTokens() {
			Description = RoR2.Language.GetString(GetDescriptionToken());
		}
		
		private void ResetDescription() {
			Description = RoR2.Language.GetString(GetDescriptionToken());
		}
	}
	
	internal class HurricaneSliderOption : SliderOption {

		public HurricaneSliderOption(ConfigEntry<float> configEntry, SliderConfig config) : base(configEntry, config) {
			RoR2.Language.onCurrentLanguageChanged+= ResetDescription;
		}

		public override void RegisterTokens() {
			Description = RoR2.Language.GetString(GetDescriptionToken());
		}
		
		private void ResetDescription() {
			Description = RoR2.Language.GetString(GetDescriptionToken());
		}
	}
	internal class HurricaneStepSliderOption : StepSliderOption {
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public HurricaneStepSliderOption(ConfigEntry<float> configEntry, StepSliderConfig config) : base(configEntry,
			config) {
			RoR2.Language.onCurrentLanguageChanged += ResetDescription;
		}
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public override void RegisterTokens() {
			Description = RoR2.Language.GetString(GetDescriptionToken());
		}
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private void ResetDescription() {
			Description = RoR2.Language.GetString(GetDescriptionToken());
		}
	}
}}