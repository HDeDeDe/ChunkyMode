using System.Collections.Generic;
using RoR2;

namespace HDeMods {
	public enum CachedIndex{
		None,
		BeetleGuard,
		Vagrant,
		Bell,
		RobNemesis,
		SigmaConstruct
	}
	public static class ChunkyCachedBodyIndex {
		public static Dictionary<BodyIndex, CachedIndex> Cache = new Dictionary<BodyIndex, CachedIndex>();

		public static void GenerateCache() {
#if DEBUG
			Log.Fatal("Generating Cache!");
#endif
			Cache.Add(BodyCatalog.FindBodyIndex("BeetleGuardBody"), CachedIndex.BeetleGuard);
			Cache.Add(BodyCatalog.FindBodyIndex("VagrantBody"), CachedIndex.Vagrant);
			Cache.Add(BodyCatalog.FindBodyIndex("BellBody"), CachedIndex.Bell);
			if (ChunkyOptionalMods.Hunk.enabled) Cache.Add(BodyCatalog.FindBodyIndex("RobNemesisPlayerBody"), CachedIndex.RobNemesis);
			if (ChunkyOptionalMods.Spikestrip.enabled) Cache.Add(BodyCatalog.FindBodyIndex("SigmaConstructBody"), CachedIndex.SigmaConstruct);
		}
	}
}