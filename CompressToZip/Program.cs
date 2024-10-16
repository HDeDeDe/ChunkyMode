using System.Collections;
using System.IO.Compression;
using System.Diagnostics;

//-----------------------------------------------------Customize--------------------------------------------------------
// ReSharper disable once InconsistentNaming
const bool giveMePDBs = true;

const string pluginName = HDeMods.ChunkyMode.PluginName;
const string pluginAuthor = HDeMods.ChunkyMode.PluginAuthor;
const string pluginVersion = HDeMods.ChunkyMode.PluginVersion;
const string changelog = "../CHANGELOG.md";
const string readme = "../README.md";
const string icon = "../Resources/ror2Assets/Assets/ChunkyDiffAssets/ChunkyDiffBundle/texChunkyModeDiffIcon.png";
const string riskOfRain2Install = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Risk of Rain 2\\Risk of Rain 2_Data\\Managed\\";
ArrayList extraFiles = new ArrayList {
	new FileInfo("../Resources/ChunkyMode.language"),
	new FileInfo("../Resources/ror2Assets/Assets/AssetBundle/chunkydifficon")
};
const string manifestWebsiteUrl = "https://github.com/HDeDeDe/ChunkyMode";
const string manifestDescription = "A difficulty aimed at bringing Eclipse level challenges while maintaining somewhat vanilla gameplay.";
const string manifestDependencies = "[\n" +
                                    "\t\t\"bbepis-BepInExPack-5.4.2108\",\n" + 
                                    "\t\t\"RiskofThunder-HookGenPatcher-1.2.3\",\n" + 
                                    "\t\t\"RiskofThunder-R2API_Language-1.0.1\",\n" + 
                                    "\t\t\"RiskofThunder-R2API_Difficulty-1.1.2\",\n" + 
                                    "\t\t\"RiskofThunder-R2API_RecalculateStats-1.4.0\",\n" +
                                    "\t\t\"RiskofThunder-R2API_Networking-1.0.2\",\n" +
                                    "\t\t\"RiskofThunder-R2API_Director-2.2.1\",\n" +
                                    "\t\t\"HDeDeDe-HealthComponentAPI-0.1.1\"\n" +
                                    "\t]";
//-----------------------------------------------------Stop-------------------------------------------------------------

const string targetFile = "../" + pluginName + "/bin/" + pluginName + ".zip";

#if DEBUG
const string dllPath = "../" + pluginName + "/bin/Debug/netstandard2.1/";
const string dllPathWindows = "..\\" + pluginName + "\\bin\\Debug\\netstandard2.1\\";
#endif

#if RELEASE
const string dllPath = "../" + pluginName + "/bin/Release/netstandard2.1/";
const string dllPathWindows = "..\\" + pluginName + "\\bin\\Release\\netstandard2.1\\";
#endif

Console.WriteLine("Weaving " + pluginName + ".dll");
if(File.Exists(dllPath + pluginName + ".prepatch")) File.Delete(dllPath + pluginName + ".prepatch");
File.Copy(dllPath + pluginName + ".dll", dllPath + pluginName + ".prepatch");

Process weaver = new Process();
if (giveMePDBs) weaver.StartInfo.FileName = @".\NetWeaver\Unity.UNetWeaver2.exe";
#pragma warning disable CS0162 // Unreachable code detected
else weaver.StartInfo.FileName = @".\NetWeaver\Unity.UNetWeaver.exe";
#pragma warning restore CS0162 // Unreachable code detected
weaver.StartInfo.Arguments = "\"" + riskOfRain2Install + "UnityEngine.CoreModule.dll\" " +
                             "\"" + riskOfRain2Install + "com.unity.multiplayer-hlapi.Runtime.dll\" " +
                             dllPathWindows + " " +
                             dllPathWindows + pluginName + ".dll " +
                             // Dependency folders
                             "\"" + riskOfRain2Install + "\" " +
                             dllPathWindows + " " +
                             "\"" + Environment.GetEnvironmentVariable("HOMEPATH") + "\\.nuget\\packages\\\"";
weaver.StartInfo.RedirectStandardOutput = true;
weaver.Start();
string output;
while ((output = weaver.StandardOutput.ReadLine()!) != null) {
	Console.WriteLine(output);
}
weaver.WaitForExit();

Console.WriteLine("Creating " + pluginName + ".Zip");
if (File.Exists(targetFile)) File.Delete(targetFile);

ZipArchive archive = ZipFile.Open(targetFile, ZipArchiveMode.Create);

archive.CreateEntryFromFile(changelog, "CHANGELOG.md", CompressionLevel.Optimal);
archive.CreateEntryFromFile(readme, "README.md", CompressionLevel.Optimal);
archive.CreateEntryFromFile(dllPath + pluginName + ".dll", pluginName + ".dll", CompressionLevel.Optimal);
if (giveMePDBs) archive.CreateEntryFromFile(dllPath + pluginName + ".pdb", pluginName + ".pdb", CompressionLevel.Optimal);
archive.CreateEntryFromFile(icon, "icon.png", CompressionLevel.Optimal);

foreach (FileInfo file in extraFiles) {
	archive.CreateEntryFromFile(file.FullName, file.Name, CompressionLevel.Optimal);
}

ZipArchiveEntry manifest = archive.CreateEntry("manifest.json", CompressionLevel.Optimal);
using (StreamWriter writer = new StreamWriter(manifest.Open())) {
	writer.WriteLine("{");
	writer.WriteLine("\t\"author\": \"" + pluginAuthor + "\",");
	writer.WriteLine("\t\"name\": \"" + pluginName + "\",");
	writer.WriteLine("\t\"version_number\": \"" + pluginVersion + "\",");
	writer.WriteLine("\t\"website_url\": \"" + manifestWebsiteUrl + "\",");
	writer.WriteLine("\t\"description\": \"" + manifestDescription + "\",");
	writer.WriteLine("\t\"dependencies\": " + manifestDependencies);
	writer.WriteLine("}");
	
	writer.Close();
}

archive.Dispose();