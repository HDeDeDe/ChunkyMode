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
		public static readonly Dictionary<BodyIndex, BodyCache> body = new Dictionary<BodyIndex, BodyCache>();
		public static int injector;

		public static void GenerateCache() {
#if DEBUG
			Log.Fatal("Generating Cache!");
#endif
			injector = ProjectileCatalog.FindProjectileIndex("SyringeProjectileHealing");
			body.Add(BodyCatalog.FindBodyIndex("BeetleGuardBody"), BodyCache.BeetleGuard);
			body.Add(BodyCatalog.FindBodyIndex("VagrantBody"), BodyCache.Vagrant);
			body.Add(BodyCatalog.FindBodyIndex("BellBody"), BodyCache.Bell);
			body.Add(BodyCatalog.FindBodyIndex("FlyingVerminBody"), BodyCache.FlyingVermin);
			body.Add(BodyCatalog.FindBodyIndex("BisonBody"), BodyCache.Bison);
			if (ChunkyOptionalMods.Hunk.Enabled) body.Add(BodyCatalog.FindBodyIndex("RobNemesisPlayerBody"), BodyCache.RobNemesis);
			if (ChunkyOptionalMods.Spikestrip.Enabled) body.Add(BodyCatalog.FindBodyIndex("SigmaConstructBody"), BodyCache.SigmaConstruct);
		}
	}
}