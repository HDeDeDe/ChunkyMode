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
			Cache.TryAdd(BodyCatalog.FindBodyIndex("BeetleGuard"), CachedIndex.BeetleGuard);
			Cache.TryAdd(BodyCatalog.FindBodyIndex("VagrantBody"), CachedIndex.Vagrant);
			Cache.TryAdd(BodyCatalog.FindBodyIndex("BellBody"), CachedIndex.Bell);
			if (ChunkyOptionalMods.Hunk.enabled) Cache.TryAdd(BodyCatalog.FindBodyIndex("RobNemesisPlayerBody"), CachedIndex.RobNemesis);
			if (ChunkyOptionalMods.Spikestrip.enabled) Cache.TryAdd(BodyCatalog.FindBodyIndex("SigmaConstructBody"), CachedIndex.SigmaConstruct);
		}
	}
}