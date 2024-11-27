# Compiling TABGVR

## Prerequisites
You will need:
- [.NET SDK 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Git](https://git-scm.com/)
- Basic terminal skills

## Cloning the repository
Run:
```shell
git clone https://github.com/RedBigz/TABGVR --recurse-submodules
```

## Gathering Required Files

### Downloading the pre-made bundle (easy)
The pre-made bundle contains the required assemblies for running the mod.

> [!NOTE]
> To set up the bundle, you will need a program like [7-Zip](https://www.7-zip.org/) to extract tar.xz files.

Firstly, download [TABG.VR.QuickInstallerContainer.tar.xz](https://redbigz.com/lfs/TABG.VR.QuickInstallerContainer.tar.xz) and unzip it wherever. You will need this for later.

Copy the contents of `TABGVR/core` into a new folder called `GameReferences` in the root of the repository.

Now follow the guide for gathering managed assemblies legally *only* in the sourcing guide below.

### Sourcing them yourself (intermediate)

Firstly, install [BepInEx](https://github.com/BepInEx/BepInEx) to TABG.

#### Gathering game files legally
The build process for this mod makes it fairly easy to source game assemblies. If you have installed TABG on your C:\ drive, **you can skip this step**, as the build process will automatically detect the game files.

If not, just go to TABG in Steam, press `Settings > Manage > Browse local files` and note down the path of TABG for later.

#### Gathering Unity plugins
You will need to install [Unity 2021.3.22f1](https://unity.com/releases/editor/whats-new/2021.3.22) and make a VR project.

Firstly, uninstall the Burst plugin (as it will interfere with the Interaction Toolkit at runtime). Secondly, go to your project settings and disable IL2CPP, as we need Mono DLLs. Now just build the project. 

Make a folder called `GameReferences` in the root of the repository and copy these files from `<project name>_Data/Managed` over to it:
- Unity.InputSystem.dll
- Unity.XR.CoreUtils.dll
- Unity.XR.Interaction.Toolkit.dll
- Unity.XR.Management.dll
- Unity.XR.OpenXR.dll

Also, copy the above files to `BepInEx/core` in your TABG directory.

Additionally, copy these files from `<project name>_Data/Plugins/x86_64` to `TotallyAccurateBattlegrounds_Data/Plugins/x86_64` in your TABG directory:
- openxr_loader.dll
- UnityOpenXR.dll

Make a `UnitySubsystems` folder in `TotallyAccurateBattlegrounds_Data` with a subfolder called `UnityOpenXR`.
In this folder, create a `UnitySubsystemsManifest.json` with these contents:
```json
{
    "name": "OpenXR XR Plugin",
    "version": "1.8.2",
    "libraryName": "UnityOpenXR",
    "displays": [
        {
            "id": "OpenXR Display"
        }
    ],
    "inputs": [
        {
            "id": "OpenXR Input"
        }
    ]
}
```

## Building
You just need to build the mod as you would for a .NET project.

```shell
dotnet build -c release
```
*If you have installed TABG in a place other than your C:\ drive, append `-p:TABGFolder="<Your TABG directory>"` to the command.*

Copy `TABGVR/bin/Release/netstandard2.1/TABGVR.dll` to `<bundle>/TABGVR/plugins` (pre-made bundle) or `<TABG directory>/BepInEx/plugins` (sourced)

**If you are using the pre-made bundle, copy the entire contents of the bundle into the root of your TABG folder.**

## Running
To run the game, refer to [SETUP.md](SETUP.md).