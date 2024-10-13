using System.Collections.Generic;
using RoR2;

namespace HDeMods {
	internal enum BodyCache{
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
		public static Dictionary<BodyIndex, BodyCache> Body = new Dictionary<BodyIndex, BodyCache>();
		public static int Injector;

		public static void GenerateCache() {
#if DEBUG
			Log.Fatal("Generating Cache!");
#endif
			Injector = ProjectileCatalog.FindProjectileIndex("SyringeProjectileHealing");
			Body.Add(BodyCatalog.FindBodyIndex("BeetleGuardBody"), BodyCache.BeetleGuard);
			Body.Add(BodyCatalog.FindBodyIndex("VagrantBody"), BodyCache.Vagrant);
			Body.Add(BodyCatalog.FindBodyIndex("BellBody"), BodyCache.Bell);
			Body.Add(BodyCatalog.FindBodyIndex("FlyingVerminBody"), BodyCache.FlyingVermin);
			Body.Add(BodyCatalog.FindBodyIndex("BisonBody"), BodyCache.Bison);
			if (ChunkyOptionalMods.Hunk.enabled) Body.Add(BodyCatalog.FindBodyIndex("RobNemesisPlayerBody"), BodyCache.RobNemesis);
			if (ChunkyOptionalMods.Spikestrip.enabled) Body.Add(BodyCatalog.FindBodyIndex("SigmaConstructBody"), BodyCache.SigmaConstruct);
		}
	}
}