using RiskOfOptions;
using RiskOfOptions.Options;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using RiskOfOptions.OptionConfigs;
using UnityEngine;
using UnityEngine.Events;

namespace HDeMods { namespace HurricaneOptionalMods {
	internal static class RoO {
            public static bool Enabled =>
                BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void AddCheck(ConfigEntry<bool> option, bool requireRestart = false) {
                LocalizedCheckBoxOption boxOption = new LocalizedCheckBoxOption(option, requireRestart);
                ModSettingsManager.AddOption(boxOption, HurricanePlugin.PluginGUID,
                    HurricanePlugin.PluginName);
#if DEBUG
                CM.Log.Debug(boxOption.GetNameToken());
                CM.Log.Debug(boxOption.GetDescriptionToken());
#endif
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void AddInt(ConfigEntry<int> option, int minimum, int maximum) {
                LocalizedIntSliderOption sliderOption =
                    new LocalizedIntSliderOption(option, new IntSliderConfig() { min = minimum, max = maximum });
                ModSettingsManager.AddOption(sliderOption, HurricanePlugin.PluginGUID,
                    HurricanePlugin.PluginName);

#if DEBUG
                CM.Log.Debug(sliderOption.GetNameToken());
                CM.Log.Debug(sliderOption.GetDescriptionToken());
#endif
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void AddFloat(ConfigEntry<float> option, float minimum, float maximum,
                string format = "{0:0}%") {
                LocalizedSliderOption sliderOption = new LocalizedSliderOption(option,
                    new SliderConfig() { min = minimum, max = maximum, FormatString = format });
                ModSettingsManager.AddOption(sliderOption, HurricanePlugin.PluginGUID,
                    HurricanePlugin.PluginName);

#if DEBUG
                CM.Log.Debug(sliderOption.GetNameToken());
                CM.Log.Debug(sliderOption.GetDescriptionToken());
#endif
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void AddFloatStep(ConfigEntry<float> option, float minimum, float maximum, float step,
                string format = "{0:0}%") {
                LocalizedSliderStepOption stepSliderOption = new LocalizedSliderStepOption(option, new StepSliderConfig()
                    { min = minimum, max = maximum, FormatString = format, increment = step });
                ModSettingsManager.AddOption(stepSliderOption, HurricanePlugin.PluginGUID,
                    HurricanePlugin.PluginName);

#if DEBUG
                CM.Log.Debug(stepSliderOption.GetNameToken());
                CM.Log.Debug(stepSliderOption.GetDescriptionToken());
#endif
            }
            
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void AddButton(string name, string category, UnityAction onButtonPressed) {
                LocalizedButtonOption buttonOption = new LocalizedButtonOption(name, category, "", "", onButtonPressed);
                ModSettingsManager.AddOption(buttonOption, HurricanePlugin.PluginGUID,
                    HurricanePlugin.PluginName);

#if DEBUG
                CM.Log.Debug(buttonOption.GetNameToken());
                CM.Log.Debug(buttonOption.GetDescriptionToken());
                CM.Log.Debug(buttonOption.GetButtonLabelToken());
#endif
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void SetSprite(Sprite sprite) => ModSettingsManager.SetModIcon(sprite);

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void SetDescriptionToken(string description) => 
                ModSettingsManager.SetModDescriptionToken(description);
        }
    }

    // Thanks to Bubbet for the suggestion to do this
    internal class LocalizedCheckBoxOption : CheckBoxOption {
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public LocalizedCheckBoxOption(ConfigEntry<bool> configEntry, bool restart) : base(configEntry, restart) {
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

    internal class LocalizedIntSliderOption : IntSliderOption {
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public LocalizedIntSliderOption(ConfigEntry<int> configEntry, IntSliderConfig config) : base(configEntry, config) {
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

    internal class LocalizedSliderOption : SliderOption {
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public LocalizedSliderOption(ConfigEntry<float> configEntry, SliderConfig config) : base(configEntry, config) {
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

    internal class LocalizedSliderStepOption : StepSliderOption {
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public LocalizedSliderStepOption(ConfigEntry<float> configEntry, StepSliderConfig config) : base(configEntry,
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
    internal class LocalizedButtonOption : GenericButtonOption {
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public LocalizedButtonOption(string name, string category, string description, string buttonText, UnityAction onButtonPressed) 
            : base(name, category, description, buttonText, onButtonPressed) {
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
}