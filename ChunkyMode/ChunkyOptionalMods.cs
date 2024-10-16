using System.Runtime.CompilerServices;

namespace HDeMods { namespace ChunkyOptionalMods {
	internal class Hunk {
		private static bool? _enabled;

        public static bool enabled {
            get {
                if (_enabled == null) {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rob.Hunk");
                }
                return (bool)_enabled;
            }
        }
    }
    internal class Spikestrip {
        private static bool? _enabled;

        public static bool enabled {
            get {
                if (_enabled == null) {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("_com.prodzpod.ProdzpodSpikestripContent");
                }
                return (bool)_enabled;
            }
        }
        
    }
} }