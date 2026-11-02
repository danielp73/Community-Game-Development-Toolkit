# Updating the toolkit

In Unity, choose **Game-Toolkit → Update Toolkit** to download and install scripts from:

https://share.communitygametoolkit.org/toolkit/latest.zip

There are no version numbers or update checks. Each use downloads the current ZIP. The command is unavailable during play mode, compilation, or another update.

Only `.cs` files within `Assets/Community-Game-Development-Toolkit/System/` and `Assets/Community-Game-Development-Toolkit/CGDT Components/` can be installed. Matching scripts are overwritten, new scripts are added, and scripts missing from the ZIP are retained. Existing `.meta` files and other assets are untouched. Unity generates metadata for new scripts after the final refresh. Any local edits to included toolkit scripts will be replaced.

## Making the ZIP

1. Open the Unity project containing the scripts you want to distribute and wait for compilation to finish.
2. Choose **Game-Toolkit → Developer → Build Update ZIP**.
3. Save the file as `latest.zip` outside the Unity project's `Assets` folder.
4. Upload it to your web server so the exact HTTPS URL above returns the ZIP file directly, without a login or an HTML download page. The required server filesystem path depends on your hosting configuration.
5. Try **Update Toolkit** in a disposable copy of a toolkit project before distributing the release.

The ZIP builder includes scripts from both owned folders, including the updater itself, and excludes `.meta` files, scenes, images, prefabs, documentation, and Git files. ZIP paths are relative to the toolkit folder, for example:

```
System/Editor/ToolkitUpdater.cs
System/Editor/Toolkit Server/Editor/ToolkitServerWindow.cs
CGDT Components/Player/KeyboardMove.cs
```

Do not add an outer folder or an `Assets/` prefix. A normal repository download ZIP is not the updater format. The updater rejects non-script files, duplicate paths, paths outside the two owned folders, symbolic-link destinations, empty scripts, individual scripts larger than 8 MiB, and total script content larger than 64 MiB. It reads and validates the entire archive before writing, suppresses automatic refresh and assembly reload while installing, and restores original file contents if a write fails (restore errors are reported in the Unity Console).

To publish another release, build a fresh ZIP and replace the file at the same URL. The Unity server window and its development-mode setting do not affect updates.
