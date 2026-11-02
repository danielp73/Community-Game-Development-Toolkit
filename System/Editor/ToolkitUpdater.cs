using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public static class ToolkitUpdater
{
    private const string DownloadUrl = "https://share.communitygametoolkit.org/toolkit/latest.zip";
    private const string ToolkitFolder = "Assets/Community-Game-Development-Toolkit";
    private const long MaxScriptBytes = 8 * 1024 * 1024;
    private const long MaxTotalBytes = 64 * 1024 * 1024;
    private static bool busy;

    private static string Root => Path.GetFullPath(ToolkitFolder);

    [MenuItem("Game-Toolkit/Update Toolkit")]
    private static async void UpdateToolkit()
    {
        busy = true;
        string temporary = Path.Combine(Path.GetTempPath(), "ToolkitUpdate-" + Guid.NewGuid().ToString("N"));
        bool reloadLocked = false;
        bool refreshDisabled = false;
        bool filesTouched = false;
        try
        {
            Directory.CreateDirectory(temporary);
            string archivePath = Path.Combine(temporary, "latest.zip");
            using (var request = UnityWebRequest.Get(DownloadUrl))
            {
                request.downloadHandler = new DownloadHandlerFile(archivePath);
                request.timeout = 120;
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Update Toolkit", "Downloading toolkit scripts…", request.downloadProgress))
                    {
                        request.Abort();
                        return;
                    }
                    await Awaitable.NextFrameAsync();
                }
                if (request.result != UnityWebRequest.Result.Success)
                    throw new IOException("Download failed: " + request.error);
            }

            // Fully validate and read the ZIP before modifying any project file.
            var scripts = ReadScripts(archivePath);
            foreach (string relative in scripts.Keys)
                CheckDestination(relative);

            EditorUtility.DisplayProgressBar("Update Toolkit", "Installing toolkit scripts…", 1f);
            EditorApplication.LockReloadAssemblies();
            reloadLocked = true;
            AssetDatabase.DisallowAutoRefresh();
            refreshDisabled = true;
            var originals = new Dictionary<string, byte[]>();
            try
            {
                foreach (var script in scripts)
                {
                    string destination = CheckDestination(script.Key);
                    originals.Add(destination, File.Exists(destination) ? File.ReadAllBytes(destination) : null);
                    Directory.CreateDirectory(Path.GetDirectoryName(destination));
                    filesTouched = true;
                    File.WriteAllBytes(destination, script.Value);
                }
            }
            catch
            {
                foreach (var original in originals)
                {
                    try
                    {
                        if (original.Value == null) File.Delete(original.Key);
                        else File.WriteAllBytes(original.Key, original.Value);
                    }
                    catch (Exception rollbackError) { Debug.LogException(rollbackError); }
                }
                throw;
            }
            Debug.Log($"Toolkit updated: {scripts.Count} scripts installed from {DownloadUrl}");
        }
        catch (Exception error)
        {
            Debug.LogException(error);
            EditorUtility.DisplayDialog("Update Toolkit", "The update could not be completed.\n\n" + error.Message, "OK");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            try { if (Directory.Exists(temporary)) Directory.Delete(temporary, true); }
            catch (Exception cleanupError) { Debug.LogWarning(cleanupError.Message); }
            busy = false;
            try
            {
                if (refreshDisabled) AssetDatabase.AllowAutoRefresh();
                if (filesTouched) AssetDatabase.Refresh();
            }
            finally { if (reloadLocked) EditorApplication.UnlockReloadAssemblies(); }
        }
    }

    [MenuItem("Game-Toolkit/Update Toolkit", true)]
    private static bool CanUpdate() => !busy && !EditorApplication.isCompiling && !EditorApplication.isPlayingOrWillChangePlaymode;

    [MenuItem("Game-Toolkit/Developer/Build Update ZIP")]
    private static void BuildUpdateZip()
    {
        string output = EditorUtility.SaveFilePanel("Build toolkit update ZIP", "", "latest", "zip");
        if (string.IsNullOrEmpty(output)) return;
        string temporary = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".zip");
        try
        {
            using (var archive = ZipFile.Open(temporary, ZipArchiveMode.Create))
            {
                foreach (string folder in new[] { "System", "CGDT Components" })
                {
                    foreach (string file in Directory.GetFiles(Path.Combine(Root, folder), "*.cs", SearchOption.AllDirectories))
                    {
                        string relative = file.Substring(Root.Length + 1).Replace('\\', '/');
                        CheckDestination(relative);
                        archive.CreateEntryFromFile(file, relative, System.IO.Compression.CompressionLevel.Optimal);
                    }
                }
            }
            int count = ReadScripts(temporary).Count;
            File.Copy(temporary, output, true);
            EditorUtility.DisplayDialog("Toolkit update ZIP", $"Created {output}\n\nContains {count} scripts only. Upload it as latest.zip to:\n{DownloadUrl}", "OK");
        }
        catch (Exception error)
        {
            Debug.LogException(error);
            EditorUtility.DisplayDialog("Build Update ZIP", error.Message, "OK");
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
    }

    private static Dictionary<string, byte[]> ReadScripts(string path)
    {
        var scripts = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
        long total = 0;
        using (var archive = ZipFile.OpenRead(path))
        {
            foreach (var entry in archive.Entries)
            {
                string name = entry.FullName;
                if (name.EndsWith("/", StringComparison.Ordinal)) continue;
                ValidateName(name);
                if (entry.Length <= 0 || entry.Length > MaxScriptBytes || (total += entry.Length) > MaxTotalBytes)
                    throw new InvalidDataException("The ZIP contains empty or oversized scripts.");
                if (scripts.ContainsKey(name)) throw new InvalidDataException("Duplicate script in ZIP: " + name);
                using (var input = entry.Open())
                using (var output = new MemoryStream())
                {
                    var buffer = new byte[8192];
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (output.Length + read > entry.Length) throw new InvalidDataException("Invalid script size: " + name);
                        output.Write(buffer, 0, read);
                    }
                    if (output.Length != entry.Length) throw new InvalidDataException("Incomplete script: " + name);
                    scripts.Add(name, output.ToArray());
                }
            }
        }
        if (scripts.Count == 0) throw new InvalidDataException("The ZIP contains no toolkit scripts.");
        return scripts;
    }

    private static void ValidateName(string name)
    {
        if (!(name.StartsWith("System/", StringComparison.Ordinal) || name.StartsWith("CGDT Components/", StringComparison.Ordinal)) ||
            !name.EndsWith(".cs", StringComparison.Ordinal) || name.IndexOfAny(new[] { '\\', ':', '\0' }) >= 0)
            throw new InvalidDataException("Only toolkit scripts are allowed in the ZIP: " + name);
        foreach (string part in name.Split('/'))
            if (part.Length == 0 || part == "." || part == "..")
                throw new InvalidDataException("Unsafe ZIP path: " + name);
    }

    private static string CheckDestination(string relative)
    {
        ValidateName(relative);
        if (!Directory.Exists(Root)) throw new DirectoryNotFoundException("Toolkit directory not found: " + Root);
        string destination = Path.GetFullPath(Path.Combine(Root, relative));
        if (!destination.StartsWith(Root + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            throw new InvalidDataException("Script path escapes toolkit directory.");
        // Reject links anywhere along the destination path, including the toolkit's parents.
        for (string current = destination; !string.IsNullOrEmpty(current); current = Path.GetDirectoryName(current))
            if ((File.Exists(current) || Directory.Exists(current)) &&
                (File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
                throw new IOException("Cannot update through a symbolic link: " + current);
        return destination;
    }
}
