using BepInEx;
using BepInEx.Logging;
using System.Diagnostics.CodeAnalysis;
using R2API;

namespace HDeMods {
	[BepInDependency(DifficultyAPI.PluginGUID)]
	[BepInDependency(LanguageAPI.PluginGUID)]
	[BepInDependency(RecalculateStatsAPI.PluginGUID)]
	[BepInDependency(DirectorAPI.PluginGUID)]
	[BepInDependency(R2API.Networking.NetworkingAPI.PluginGUID)]
	[BepInDependency(HealthComponentAPI.PluginGUID)]
	[BepInDependency(InterlopingArtifactPlugin.PluginGUID)]
	[BepInDependency("com.TeamMoonstorm", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency(ProperSave.ProperSavePlugin.GUID, BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("_com.prodzpod.ProdzpodSpikestripContent", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.rob.Hunk", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("bubbet.riskui", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.TheTimeSweeper.Aliem", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.kenko.Submariner", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.rob.Ravager", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.HDeDeDe.Hurricane", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency(Inferno.Main.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency(Downpour.DownpourPlugin.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]
	[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "StringLiteralTypo")]
	public class HurricanePlugin : BaseUnityPlugin {
		public const string PluginGUID = "com." + PluginAuthor + "." + PluginName;
		public const string PluginAuthor = "HDeDeDe";
		public const string PluginName = "ChunkyMode";
		public const string PluginVersion = "0.5.0";
		public static HurricanePlugin instance;

		private void Awake() {
			if (instance != null) {
				CM.Log.Error("There can be only 1 instance of " + PluginName + "!");
				Destroy(this);
				return;
			}
            
			CM.Log.Init(Logger);
			instance = this;
			Hurricane.StartUp();
		}

		private void FixedUpdate() {
			SadomasochismWish.CheckIfRunIsValid();
		}
	}

	namespace CM
	{
		internal static class Log
		{
			private static ManualLogSource logSource;

			internal static void Init(ManualLogSource log) => logSource = log;

			internal static void Debug(object data) => logSource.LogDebug(data);
			internal static void Error(object data) => logSource.LogError(data);
			internal static void Fatal(object data) => logSource.LogFatal(data);
			internal static void Info(object data) => logSource.LogInfo(data);
			internal static void Message(object data) => logSource.LogMessage(data);
			internal static void Warning(object data) => logSource.LogWarning(data);
		}
	}
}
