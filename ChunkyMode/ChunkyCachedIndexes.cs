using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RoR2;

namespace HDeMods {
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	internal enum BodyCache {
		None,
		BeetleGuard,
		Vagrant,
		Bell,
		RobNemesis,
		SigmaConstruct,
		FlyingVermin,
		Bison
	}
	internal static class ChunkyCachedIndexes {
		public static readonly Dictionary<BodyIndex, BodyCache> bodyIndex = new Dictionary<BodyIndex, BodyCache>();
		public static readonly Dictionary<BodyCache, BodyIndex> bodyCache = new Dictionary<BodyCache, BodyIndex>();
		public static int injector;

		public static void GenerateCache() {
#if DEBUG
			CM.Log.Fatal("Generating Cache!");
#endif
			injector = ProjectileCatalog.FindProjectileIndex("SyringeProjectileHealing");
			
			AddToCollection(BodyCatalog.FindBodyIndex("BeetleGuardBody"), BodyCache.BeetleGuard);
			AddToCollection(BodyCatalog.FindBodyIndex("VagrantBody"), BodyCache.Vagrant);
			AddToCollection(BodyCatalog.FindBodyIndex("BellBody"), BodyCache.Bell);
			AddToCollection(BodyCatalog.FindBodyIndex("FlyingVerminBody"), BodyCache.FlyingVermin);
			AddToCollection(BodyCatalog.FindBodyIndex("BisonBody"), BodyCache.Bison);
			if (ChunkyOptionalMods.Hunk.Enabled) AddToCollection(BodyCatalog.FindBodyIndex("RobNemesisPlayerBody"), BodyCache.RobNemesis);
			if (ChunkyOptionalMods.Spikestrip.Enabled) AddToCollection(BodyCatalog.FindBodyIndex("SigmaConstructBody"), BodyCache.SigmaConstruct);
		}

		private static void AddToCollection(BodyIndex index, BodyCache cache) {
			bodyIndex.Add(index, cache);
			bodyCache.Add(cache, index);
		}
	}
}