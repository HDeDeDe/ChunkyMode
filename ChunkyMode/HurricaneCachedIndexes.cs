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
		Lemurian,
		RobPaladin
	}
	internal static class HurricaneCachedIndexes {
		public static readonly Dictionary<BodyIndex, BodyCache> bodyIndex = new Dictionary<BodyIndex, BodyCache>();
		public static readonly Dictionary<BodyCache, BodyIndex> bodyCache = new Dictionary<BodyCache, BodyIndex>();
		public static int injector;
		private static RuleChoiceDef legacyChoiceDef;
		private static RuleChoiceDef hurricaneChoiceDef;
		
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
			AddToCollection(BodyCatalog.FindBodyIndex("RobPaladinBody"), BodyCache.RobPaladin);
			if (HurricaneOptionalMods.Hunk.Enabled) AddToCollection(BodyCatalog.FindBodyIndex("RobNemesisPlayerBody"), BodyCache.RobNemesis);
			if (HurricaneOptionalMods.Spikestrip.Enabled) AddToCollection(BodyCatalog.FindBodyIndex("SigmaConstructBody"), BodyCache.SigmaConstruct);
			
			hurricaneChoiceDef = RuleCatalog.FindChoiceDef("Difficulty.Hurricane");
			hurricaneChoiceDef.availableInSinglePlayer = false;
			hurricaneChoiceDef.availableInMultiPlayer = false;
			
			legacyChoiceDef = RuleCatalog.FindChoiceDef("Difficulty.ChunkyMode");
			legacyChoiceDef.availableInSinglePlayer = true;
			legacyChoiceDef.availableInMultiPlayer = true;
			legacyChoiceDef.tooltipNameToken = "CHUNKYMODEDIFFMOD_NAME";
			
			Hurricane.LegacyDifficultyDef.nameToken = "CHUNKYMODEDIFFMOD_NAME";
		}

		private static void AddToCollection(BodyIndex index, BodyCache cache) {
			bodyIndex.Add(index, cache);
			bodyCache.Add(cache, index);
		}
	}
}