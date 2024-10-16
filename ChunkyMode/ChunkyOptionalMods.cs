using System.Runtime.CompilerServices;

namespace HDeMods { namespace ChunkyOptionalMods {
	internal static class Hunk {
        public static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rob.Hunk");
    }
    internal static class Spikestrip {
        public static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("_com.prodzpod.ProdzpodSpikestripContent");
        
    }
} }