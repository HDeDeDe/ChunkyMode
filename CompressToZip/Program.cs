using System.IO.Compression;

string targetFile = "../ChunkyMode/bin/ChunkyMode.zip";

FileInfo chunkyModeDLL;
FileInfo chunkyDiffIcon = new FileInfo("../Resources/chunkydifficon");
FileInfo changelog = new FileInfo("../CHANGELOG.md");
FileInfo readme = new FileInfo("../README.md");
FileInfo icon = new FileInfo("../Resources/icon.png");

string manifestAuthor = ChunkyMode.ChunkyMode.PluginAuthor;
string manifestName = ChunkyMode.ChunkyMode.PluginName;
string manifestVersionNumber = ChunkyMode.ChunkyMode.PluginVersion;
string manifestWebsiteUrl = "https://github.com/HDeDeDe/ChunkyMode";
string manifestDescription = "A difficulty aimed at bringing Eclipse level challenges while maintaining somewhat vanilla gameplay.";
string manifestDependencies = "[\n" +
                              "\t\t\"bbepis-BepInExPack-5.4.2108\",\n" + 
                              "\t\t\"RiskofThunder-HookGenPatcher-1.2.3\",\n" + 
                              "\t\t\"RiskofThunder-R2API_Language-1.0.1\",\n" + 
                              "\t\t\"RiskofThunder-R2API_Difficulty-1.1.2\",\n" + 
                              "\t\t\"RiskofThunder-R2API_RecalculateStats-1.4.0\"\n" +
                              "\t]";

#if DEBUG
chunkyModeDLL = new FileInfo("../ChunkyMode/bin/Debug/netstandard2.1/ChunkyMode.dll");
#endif

#if RELEASE
chunkyModeDLL = new FileInfo("../ChunkyMode/bin/Release/netstandard2.1/ChunkyMode.dll");
#endif

Console.WriteLine("Creating ChunkyMode.Zip");
if (File.Exists(targetFile)) File.Delete(targetFile);

ZipArchive archive = ZipFile.Open(targetFile, ZipArchiveMode.Create);

archive.CreateEntryFromFile(chunkyDiffIcon.FullName, chunkyDiffIcon.Name, CompressionLevel.Optimal);
archive.CreateEntryFromFile(changelog.FullName, changelog.Name, CompressionLevel.Optimal);
archive.CreateEntryFromFile(readme.FullName, readme.Name, CompressionLevel.Optimal);
archive.CreateEntryFromFile(chunkyModeDLL.FullName, chunkyModeDLL.Name, CompressionLevel.Optimal);
archive.CreateEntryFromFile(icon.FullName, icon.Name, CompressionLevel.Optimal);
ZipArchiveEntry manifest = archive.CreateEntry("manifest.json", CompressionLevel.Optimal);
using (StreamWriter writer = new StreamWriter(manifest.Open())) {
	writer.WriteLine("{");
	writer.WriteLine("\t\"author\": \"" + manifestAuthor + "\",");
	writer.WriteLine("\t\"name\": \"" + manifestName + "\",");
	writer.WriteLine("\t\"version_number\": \"" + manifestVersionNumber + "\",");
	writer.WriteLine("\t\"website_url\": \"" + manifestWebsiteUrl + "\",");
	writer.WriteLine("\t\"description\": \"" + manifestDescription + "\",");
	writer.WriteLine("\t\"dependencies\": " + manifestDependencies);
	writer.WriteLine("}");
	
	writer.Close();
}

archive.Dispose();