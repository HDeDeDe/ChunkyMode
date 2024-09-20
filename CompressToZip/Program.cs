using System.IO.Compression;

string targetFile = "../ChunkyMode/bin/ChunkyMode.zip";

FileInfo chunkyModeDLL;
FileInfo chunkyDiffIcon = new FileInfo("../Resources/chunkydifficon");
FileInfo changelog = new FileInfo("../CHANGELOG.md");
FileInfo readme = new FileInfo("../README.md");
FileInfo icon = new FileInfo("../Resources/icon.png");
FileInfo manifest = new FileInfo("../Resources/manifest.json");

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
archive.CreateEntryFromFile(manifest.FullName, manifest.Name, CompressionLevel.Optimal);

archive.Dispose();