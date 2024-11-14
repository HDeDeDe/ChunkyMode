using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using RiskOfOptions;
using RiskOfOptions.Options;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using RiskOfOptions.Components.Options;
using RiskOfOptions.Components.Panel;
using RiskOfOptions.OptionConfigs;
using UnityEngine;
using UnityEngine.Events;

namespace HDeMods { namespace HurricaneOptionalMods {
	[SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    internal static class RoO {
            public static bool Enabled =>
                BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");

            private static string modGUID;
            private static string modNAME;
            public delegate void LogDebugFunc(object data);
            private static LogDebugFunc logMe;

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void Init(string modGuid, string modName,LogDebugFunc debugFunc) {
                logMe = debugFunc;
                modGUID = modGuid;
                modNAME = modName;
            }
            

        
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void AddCheck(ConfigEntry<bool> option, bool requireRestart = false) {
                LocalizedCheckBoxOption boxOption = new LocalizedCheckBoxOption(option, requireRestart);
                ModSettingsManager.AddOption(boxOption, modGUID,
                    modNAME);
#if DEBUG
                logMe(boxOption.GetNameToken());
                logMe(boxOption.GetDescriptionToken());
#endif
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void AddInt(ConfigEntry<int> option, int minimum, int maximum) {
                LocalizedIntSliderOption sliderOption =
                    new LocalizedIntSliderOption(option, new IntSliderConfig() { min = minimum, max = maximum });
                ModSettingsManager.AddOption(sliderOption, modGUID,
                    modNAME);

#if DEBUG
                logMe(sliderOption.GetNameToken());
                logMe(sliderOption.GetDescriptionToken());
#endif
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void AddFloat(ConfigEntry<float> option, float minimum, float maximum,
                string format = "{0:0}%") {
                LocalizedSliderOption sliderOption = new LocalizedSliderOption(option,
                    new SliderConfig() { min = minimum, max = maximum, FormatString = format });
                ModSettingsManager.AddOption(sliderOption, modGUID,
                    modNAME);

#if DEBUG
                logMe(sliderOption.GetNameToken());
                logMe(sliderOption.GetDescriptionToken());
#endif
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void AddFloatStep(ConfigEntry<float> option, float minimum, float maximum, float step,
                string format = "{0:0}%") {
                LocalizedSliderStepOption stepSliderOption = new LocalizedSliderStepOption(option, new StepSliderConfig()
                    { min = minimum, max = maximum, FormatString = format, increment = step });
                ModSettingsManager.AddOption(stepSliderOption, modGUID,
                    modNAME);

#if DEBUG
                logMe(stepSliderOption.GetNameToken());
                logMe(stepSliderOption.GetDescriptionToken());
#endif
            }
            
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void AddButton(string name, string category, UnityAction onButtonPressed) {
                LocalizedButtonOption buttonOption = new LocalizedButtonOption(name, category, "", "", onButtonPressed);
                ModSettingsManager.AddOption(buttonOption, modGUID,
                    modNAME);

#if DEBUG
                logMe(buttonOption.GetNameToken());
                logMe(buttonOption.GetDescriptionToken());
                logMe(buttonOption.GetButtonLabelToken());
#endif
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void SetSprite(Sprite sprite) => ModSettingsManager.SetModIcon(sprite);

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void SetDescriptionToken(string description) => 
                ModSettingsManager.SetModDescriptionToken(description);
            
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            public static void ResetToDefault() {
                ModOptionPanelController options =
                    // I'm too lazy to find a proper way of doing this
                    GameObject.Find("SettingsPanelTitle(Clone)").GetComponent<ModOptionPanelController>();
                foreach (ModSetting setting in options._modSettings) {
                    if (setting.GetType() == typeof(GenericButtonController)) continue;
                    AccessTools.Method(setting.GetType(), "ResetToDefault")?.Invoke(setting, null);
                }
            }
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