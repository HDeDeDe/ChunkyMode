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
		Bison,
		Lemurian
	}
	internal static class HurricaneCachedIndexes {
		public static readonly Dictionary<BodyIndex, BodyCache> bodyIndex = new Dictionary<BodyIndex, BodyCache>();
		public static readonly Dictionary<BodyCache, BodyIndex> bodyCache = new Dictionary<BodyCache, BodyIndex>();
		public static int injector;
		private static RuleChoiceDef legacyChoiceDef;
		
		public static void GenerateCache() {
#if DEBUG
			CM.Log.Warning("Generating Cache!");
#endif
			injector = ProjectileCatalog.FindProjectileIndex("SyringeProjectileHealing");
			
			AddToCollection(BodyCatalog.FindBodyIndex("BeetleGuardBody"), BodyCache.BeetleGuard);
			AddToCollection(BodyCatalog.FindBodyIndex("VagrantBody"), BodyCache.Vagrant);
			AddToCollection(BodyCatalog.FindBodyIndex("BellBody"), BodyCache.Bell);
			AddToCollection(BodyCatalog.FindBodyIndex("FlyingVerminBody"), BodyCache.FlyingVermin);
			AddToCollection(BodyCatalog.FindBodyIndex("BisonBody"), BodyCache.Bison);
			AddToCollection(BodyCatalog.FindBodyIndex("LemurianBody"), BodyCache.Lemurian);
			if (HurricaneOptionalMods.Hunk.Enabled) AddToCollection(BodyCatalog.FindBodyIndex("RobNemesisPlayerBody"), BodyCache.RobNemesis);
			if (HurricaneOptionalMods.Spikestrip.Enabled) AddToCollection(BodyCatalog.FindBodyIndex("SigmaConstructBody"), BodyCache.SigmaConstruct);
			
			legacyChoiceDef = RuleCatalog.FindChoiceDef("Difficulty.ChunkyMode");
			legacyChoiceDef.availableInSinglePlayer = true;
			legacyChoiceDef.availableInMultiPlayer = true;
		}

		private static void AddToCollection(BodyIndex index, BodyCache cache) {
			bodyIndex.Add(index, cache);
			bodyCache.Add(cache, index);
		}
	}
}