using System.Runtime.CompilerServices;
using RoR2;
using UnityEngine;

namespace HDeMods { namespace ChunkyOptionalMods {
	internal static class Hunk {
        public static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rob.Hunk");
    }
    
    internal static class Spikestrip {
        public static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("_com.prodzpod.ProdzpodSpikestripContent");
        
    }

    internal static class RiskUI {
        public static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("bubbet.riskui");
        public static Sprite providedSprite;

        public static Sprite ProvideIcon(On.RoR2.DifficultyDef.orig_GetIconSprite getIconSprite, DifficultyDef self) {
            if (self != ChunkyMode.ChunkyModeDifficultyDef) return getIconSprite(self);
            return providedSprite;
        }
    }
} }