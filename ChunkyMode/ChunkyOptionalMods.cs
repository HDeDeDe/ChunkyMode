using System.Runtime.CompilerServices;
using RoR2;
using MaterialHud;
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

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void AddLegacyDifficulty() {
            RiskUIPlugin.DifficultyIconMap["CHUNKYMODEDIFFMOD_NAME"] =
                ChunkyMode.HurricaneBundle.LoadAsset<Sprite>("texChunkyModeRiskUI");
        }
    }
} }