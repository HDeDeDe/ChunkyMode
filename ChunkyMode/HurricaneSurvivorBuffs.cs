using System;
using System.Diagnostics.CodeAnalysis;
using BepInEx.Configuration;

namespace HDeMods {
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static class HurricaneSurvivorBuffs {
        public static ConfigEntry<float> RexHealOverride { get; set; }
        public static ConfigEntry<float> AcridHealOverride { get; set; }
        public static ConfigEntry<float> ChirrHealOverride { get; set; }
        public static ConfigEntry<float> AliemHealOverride { get; set; }
        public static ConfigEntry<float> SubmarinerHealOverride { get; set; }
        public static ConfigEntry<float> RavagerHealOverride { get; set; }
        
        public static void ClampValues() {
            RexHealOverride.Value = Math.Clamp(RexHealOverride.Value, 0.5f, 2f);
            AcridHealOverride.Value = Math.Clamp(AcridHealOverride.Value, 0.5f, 2f);
            ChirrHealOverride.Value = Math.Clamp(ChirrHealOverride.Value, 0.5f, 2f);
            AliemHealOverride.Value = Math.Clamp(AliemHealOverride.Value, 0.5f, 2f);
            SubmarinerHealOverride.Value = Math.Clamp(SubmarinerHealOverride.Value, 0.5f, 2f);
            RavagerHealOverride.Value = Math.Clamp(RavagerHealOverride.Value, 0.5f, 2f);
        }
        
        public static void RegisterOptions() {
            RexHealOverride = HurricanePlugin.instance.Config.Bind<float>(
                "Healing Buffs",
                "REX Override",
                1.5f,
                "The amount to multiply Tangling Growth and DIRECTIVE: Inject healing by.");
            AcridHealOverride = HurricanePlugin.instance.Config.Bind<float>(
                "Healing Buffs",
                "Acrid Override",
                2f,
                "The amount to multiply regenerative buff healing by.");
            ChirrHealOverride = HurricanePlugin.instance.Config.Bind<float>(
                "Healing Buffs",
                "Chirr Override",
                2f,
                "The amount to multiply Soothing Venom buff healing by.");
            AliemHealOverride = HurricanePlugin.instance.Config.Bind<float>(
                "Healing Buffs",
                "Alien Hominid Override",
                2f,
                "The amount to multiply Chomp healing by.");
            SubmarinerHealOverride = HurricanePlugin.instance.Config.Bind<float>(
                "Healing Buffs",
                "Submariner Override",
                2f,
                "The amount to multiply N'kuhanna's Restoration healing by.");
            RavagerHealOverride = HurricanePlugin.instance.Config.Bind<float>(
                "Healing Buffs",
                "Ravager Override",
                2f,
                "The amount to multiply Consume healing by.");
        }
        
        public static void RegisterRiskOfOptions() {
            string format = "{0}x";
            HurricaneOptionalMods.RoO.AddFloatStep(RexHealOverride, 0.5f, 2f, 0.025f, format);
            HurricaneOptionalMods.RoO.AddFloatStep(AcridHealOverride, 0.5f, 2f, 0.025f, format);
            HurricaneOptionalMods.RoO.AddFloatStep(ChirrHealOverride, 0.5f, 2f, 0.025f, format);
            HurricaneOptionalMods.RoO.AddFloatStep(AliemHealOverride, 0.5f, 2f, 0.025f, format);
            HurricaneOptionalMods.RoO.AddFloatStep(SubmarinerHealOverride, 0.5f, 2f, 0.025f, format);
            HurricaneOptionalMods.RoO.AddFloatStep(RavagerHealOverride, 0.5f, 2f, 0.025f, format);
            HurricaneOptionalMods.RoO.AddButton("Reset to default", "Healing Buffs", HurricaneOptionalMods.RoO.ResetToDefault);
        }
    }
}