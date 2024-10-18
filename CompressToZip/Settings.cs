using System.Collections;
internal static class Settings {
//-----------------------------------------------------Customize--------------------------------------------------------
		// ReSharper disable once InconsistentNaming
		public const bool giveMePDBs = true;
		public const bool weave = true;
		
		public const string pluginName = HDeMods.ChunkyMode.PluginName;
		public const string pluginAuthor = HDeMods.ChunkyMode.PluginAuthor;
		public const string pluginVersion = HDeMods.ChunkyMode.PluginVersion;
		public const string changelog = "../CHANGELOG.md";
		public const string readme = "../README.md";
		public const string icon = "../Resources/ror2Assets/Assets/ChunkyDiffAssets/ChunkyDiffBundle/texChunkyModeDiffIcon.png";
		public const string riskOfRain2Install = @"C:\Program Files (x86)\Steam\steamapps\common\Risk of Rain 2\Risk of Rain 2_Data\Managed\";
		public static readonly ArrayList extraFiles = new ArrayList {
			new FileInfo("../Resources/ChunkyMode.language"),
			new FileInfo("../Resources/ror2Assets/Assets/AssetBundle/chunkydifficon")
		};
		public const string manifestWebsiteUrl = "https://github.com/HDeDeDe/ChunkyMode";
		public const string manifestDescription = "A difficulty aimed at bringing Eclipse level challenges while maintaining somewhat vanilla gameplay.";
		public const string manifestDependencies = "[\n" +
		                                          "\t\t\"bbepis-BepInExPack-5.4.2108\",\n" + 
		                                          "\t\t\"RiskofThunder-HookGenPatcher-1.2.3\",\n" + 
		                                          "\t\t\"RiskofThunder-R2API_Language-1.0.1\",\n" + 
		                                          "\t\t\"RiskofThunder-R2API_Difficulty-1.1.2\",\n" + 
		                                          "\t\t\"RiskofThunder-R2API_RecalculateStats-1.4.0\",\n" +
		                                          "\t\t\"RiskofThunder-R2API_Networking-1.0.2\",\n" +
		                                          "\t\t\"RiskofThunder-R2API_Director-2.2.1\",\n" +
		                                          "\t\t\"HDeDeDe-HealthComponentAPI-0.1.1\"\n" +
		                                          "\t]";
}