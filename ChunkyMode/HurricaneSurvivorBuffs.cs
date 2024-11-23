using System;
using System.Diagnostics.CodeAnalysis;
using BepInEx.Configuration;

namespace HDeMods {
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static class HurricaneSurvivorBuffs {
        public static ConfigEntry<float> RexHealOverride { get; set; }
        public static ConfigEntry<float> AcridHealOverride { get; set; }
        public static ConfigEntry<float> CaptainHealOverride { get; set; }
        public static ConfigEntry<float> VoidFiendHealOverride { get; set; }
        public static ConfigEntry<float> SeekerHealOverride { get; set; }
        public static ConfigEntry<float> FalseSonHealOverride { get; set; }
        public static ConfigEntry<float> ChefSotSHealOverride { get; set; }
        public static ConfigEntry<float> ChirrHealOverride { get; set; }
        public static ConfigEntry<float> AliemHealOverride { get; set; }
        public static ConfigEntry<float> SubmarinerHealOverride { get; set; }
        public static ConfigEntry<float> RavagerHealOverride { get; set; }
        public static ConfigEntry<float> PaladinBarrierOverride { get; set; }
        
        public static void ClampValues() {
            RexHealOverride.Value = Math.Clamp(RexHealOverride.Value, 0.5f, 2f);
            AcridHealOverride.Value = Math.Clamp(AcridHealOverride.Value, 0.5f, 2f);
            CaptainHealOverride.Value = Math.Clamp(CaptainHealOverride.Value, 0.5f, 2f);
            VoidFiendHealOverride.Value = Math.Clamp(VoidFiendHealOverride.Value, 0.5f, 2f);
            SeekerHealOverride.Value = Math.Clamp(SeekerHealOverride.Value, 0.5f, 2f);
            FalseSonHealOverride.Value = Math.Clamp(FalseSonHealOverride.Value, 0.5f, 2f);
            ChefSotSHealOverride.Value = Math.Clamp(ChefSotSHealOverride.Value, 0.5f, 2f);
            ChirrHealOverride.Value = Math.Clamp(ChirrHealOverride.Value, 0.5f, 2f);
            AliemHealOverride.Value = Math.Clamp(AliemHealOverride.Value, 0.5f, 2f);
            SubmarinerHealOverride.Value = Math.Clamp(SubmarinerHealOverride.Value, 0.5f, 2f);
            RavagerHealOverride.Value = Math.Clamp(RavagerHealOverride.Value, 0.5f, 2f);
            PaladinBarrierOverride.Value = Math.Clamp(PaladinBarrierOverride.Value, 0f, 100f);
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
            CaptainHealOverride = HurricanePlugin.instance.Config.Bind<float>(
                "Healing Buffs",
                "Captain Override",
                1f,
                "The amount to multiply healing beacon healing by.");
            VoidFiendHealOverride = HurricanePlugin.instance.Config.Bind<float>(
                "Healing Buffs",
                "Void Fiend Override",
                1f,
                "The amount to multiply 【Sup??ress』 healing by.");
            SeekerHealOverride = HurricanePlugin.instance.Config.Bind<float>(
                "Healing Buffs",
                "Seeker Override",
                1.5f,
                "The amount to multiply Unseen Hand and Meditate healing by.");
            FalseSonHealOverride = HurricanePlugin.instance.Config.Bind<float>(
                "Healing Buffs",
                "False Son Override",
                1f,
                "The amount to multiply Lunar Tampering healing by.");
            ChefSotSHealOverride = HurricanePlugin.instance.Config.Bind<float>(
                "Healing Buffs",
                "Chef Override",
                2f,
                "The amount to multiply Chef's Kiss healing by.");
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
                1.5f,
                "The amount to multiply Consume healing by.");
            PaladinBarrierOverride = HurricanePlugin.instance.Config.Bind<float>(
                "Healing Buffs",
                "Paladin Override",
                81f,
                "The amount to subtract from barrier decay rate multiplier.");
        }
        
        public static void RegisterRiskOfOptions() {
            string format = "{0}x";
            HurricaneOptionalMods.RoO.AddFloatStep(RexHealOverride, 0.5f, 2f, 0.025f, format);
            HurricaneOptionalMods.RoO.AddFloatStep(AcridHealOverride, 0.5f, 2f, 0.025f, format);
            HurricaneOptionalMods.RoO.AddFloatStep(CaptainHealOverride, 0.5f, 2f, 0.025f, format);
            HurricaneOptionalMods.RoO.AddFloatStep(VoidFiendHealOverride, 0.5f, 2f, 0.025f, format);
            HurricaneOptionalMods.RoO.AddFloatStep(SeekerHealOverride, 0.5f, 2f, 0.025f, format);
            HurricaneOptionalMods.RoO.AddFloatStep(FalseSonHealOverride, 0.5f, 2f, 0.025f, format);
            HurricaneOptionalMods.RoO.AddFloatStep(ChefSotSHealOverride, 0.5f, 2f, 0.025f, format);
            HurricaneOptionalMods.RoO.AddFloatStep(ChirrHealOverride, 0.5f, 2f, 0.025f, format);
            HurricaneOptionalMods.RoO.AddFloatStep(AliemHealOverride, 0.5f, 2f, 0.025f, format);
            HurricaneOptionalMods.RoO.AddFloatStep(SubmarinerHealOverride, 0.5f, 2f, 0.025f, format);
            HurricaneOptionalMods.RoO.AddFloatStep(RavagerHealOverride, 0.5f, 2f, 0.025f, format);
            HurricaneOptionalMods.RoO.AddFloatStep(PaladinBarrierOverride, 0f, 100f, 1f, "{0}%");
            HurricaneOptionalMods.RoO.AddButton("Reset to default", "Healing Buffs", HurricaneOptionalMods.RoO.ResetToDefault);
        }
    }
}