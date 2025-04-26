# Schedule 1 Mods
Collection of mods for the game [Schedule 1]. Requires the use of [MelonLoader].

## Table of Contents
* [Available Mods]
* [Install]
* [Build]

## Available Mods
* **Deals Summary**
   
  Provides a summary of your active deals in the Journal app. Includes the totals of each product and packaging aggregates

  ![contract-aggregates-preview]

* **Counteroffer Asking Price Buttons**
  
  Adds buttons to add/subtract the asking price of the currently selected product in the counteroffer menu

  ![counteroffer-buttons-preview]

## Install
1. Download and setup [MelonLoader] by following the [instructions on the wiki]. Do note that Schedule 1 is an Il2Cpp game.
2. Download the [latest release]
3. (Optional) Verify the mod was built by Github from source by either:
   1. Using the [Github CLI] to verify the integrity and provenance using its associated cryptographically [signed attestations]

      `gh attestation verify schedule-one-mods_v[versionNumber].zip -R rfvgyhn/schedule-one-mods`
   2. Comparing the _shasum.txt_ contents with the _Create Checksums_ section of the job log of the [automated release]

      See [wiki] for instructions on how to check the checksums of the release artifacts.
4. Extract the DLLs from the zip
5. Copy desired DLLs into your `steamapps/common/Schedule I/Mods` folder

## Build

1. Install the [.Net SDK]

   Must be version 6 as this is what MelonLoader requires.
2. Clone repository
    ```
    $ git clone https://github.com/rfvgyhn/schedule-one-mods
    $ cd schedule-one-mods
   ```
3. Provide necessary game libraries

    Need to copy/symlink the following DLLs from the game's proxied DLLs to the `lib` directory. These can
be found in `steamapps/common/Schedule I/MelonLoader/Il2CppAssemblies` after running MelonLoader at least
once.
    * Assembly-CSharp
    * Il2Cppmscorlib
    * UnityEngine.CoreModule
    * UnityEngine.TextRenderingModule
    * UnityEngine.UI
4. Build
    ```
    $ dotnet publish -c Release
   ```
5. Copy resulting dll(s) from `src/[project]/bin/Release/net6.0/publish/[project].dll` to your game's `Mods` directory

## Automated Builds

Mods are automatically deployed to Github releases and Nexus Mods when a tag is found to match `ProjectName-v0.0.0`.

Build definition is at [.github/workflows/release.yaml].

Nexus Mods publishing makes use of [BUTR.NexusUploader] since Nexus Mods doesn't provide an API to upload mods.

[contract-aggregates-preview]: https://rfvgyhn.blob.core.windows.net/schedule1/contract-aggregates-preview.webp
[counteroffer-buttons-preview]: https://rfvgyhn.blob.core.windows.net/schedule1/counteroffer-buttons-preview.webp
[.Net SDK]: https://dotnet.microsoft.com/download/dotnet
[MelonLoader]: https://melonloader.co/
[Schedule 1]: https://www.scheduleonegame.com/
[Available Mods]: #available-mods
[Install]: #install
[Build]: #build
[instructions on the wiki]: https://melonwiki.xyz/#/?id=requirements
[latest release]: https://github.com/Rfvgyhn/schedule-one-mods/releases
[Github CLI]: https://cli.github.com/
[signed attestations]: https://docs.github.com/en/actions/security-for-github-actions/using-artifact-attestations/using-artifact-attestations-to-establish-provenance-for-builds
[automated release]: https://github.com/rfvgyhn/schedule-one-mods/actions
[wiki]: https://github.com/rfvgyhn/schedule-one-mods/wiki/Verify-Checksums-for-a-Release
[.github/workflows/release.yaml]: .github/workflows/release.yaml
[BUTR.NexusUploader]: https://github.com/BUTR/BUTR.NexusUploader