# Setup Guide
 
> [!CAUTION]
> Due to the nature of this mod, we cannot ensure that this will never be flagged as a cheat tool. If you would like to play normally without VR and matchmaking restrictions enforced by the mod for your safety, rename winhttp.dll to something else to stop the injector. **(RE)MOVING THE TABGVR PLUGIN IN THE FILES WILL NOT STOP THE INJECTOR AND EAC WILL DETECT SUSPICIOUS FILES!**

## Downloading a Build
Head over [here](https://github.com/RedBigz/TABGVR/actions?query=branch:main) to select an artifact.

> [!TIP]
> In the list you will find truncated SHA-1 hashes for the Git commit that it was compiled from. If you're checking out a certain repo in an IDE, that commit hash will match the one found by running the `git checkout -1 --oneline` command.

All you need to do to set up TABGVR is to download the artifact at the bottom page and drag the contents into the root of your TABG folder.

## Running the Game
Outright running the game through Steam will cause the game to run the EAC launcher, causing a crash and an untrusted file error. There are many ways of bypassing the launcher, and these will be detailed here:

### Running directly from the Game Folder
If there isn't one in the folder, create a `steam_appid.txt` file with `823130` in it.
Then, just run `TotallyAccurateBattlegrounds.exe` straight from File Explorer, or make a shortcut to it.

### Running through Steam using launch options
Just add this to your launch options:
```
"<steam library folder>\steamapps\common\TotallyAccurateBattlegrounds\TotallyAccurateBattlegrounds.exe" %command% <your other launch options here>
```
#### Example (C: Drive)
```
"C:\Program Files (x86)\Steam\steamapps\common\TotallyAccurateBattlegrounds\TotallyAccurateBattlegrounds.exe" %command%
```

## Reporting Bugs
If you encounter any issues, feel free to create an [issue](https://github.com/RedBigz/TABGVR/issues).

*Enjoy TABG VR! :3*