# Компиляция TABGVR

## Требования
Вам понадобится:
- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Git](https://git-scm.com/)
- Базовые умения работы с терминалом

## Клонирование
Пропишите:
```shell
git clone https://github.com/RedBigz/TABGVR --recurse-submodules
```

## Сборка нужных файлов

### Скачивание готового бандла (легко)
Готовый бандл содержит необходимые файлы для запуска мода.

> [!NOTE]
> Для того, чтобы установить бандл, вам будет нужна программа наподобие [7-Zip](https://www.7-zip.org/) для извлечения файлов tar.xz.

Для начала установите [TABG.VR.QuickInstallerContainer.tar.xz](https://redbigz.com/lfs/TABG.VR.QuickInstallerContainer.tar.xz) и распакуйте архив где угодно. Это понадобится вам позже.

Скопируйте файлы из `BepInEx/core` в новую папку с названием `GameReferences` в корне репозитория.

Теперь, следуйте инструкции для легальной сборки файлов *только* в инструкции ниже.

### Самостоятельная сборка (средне)

Сначала установите [BepInEx](https://github.com/BepInEx/BepInEx) в TABG.

#### Легальная сборка файлов игры 
Процесс билда для этого мода делает относительно лёгким сборку файлов игры. Если TABG установлен на вашем диске C:, **вы можете пропустить этот шаг**, так как процесс билда автоматически обнаружит файлы игры.

Если нет, просто перейдите в TABG в вашей библиотеке Steam, нажмите `Настройки > Управление > Просмотреть локальные файлы` и скопируйте путь папки TABG на будущее.

#### Сборка плагинов Unity
Вам нужно будет установить [Unity 2021.3.22f1](https://unity.com/releases/editor/whats-new/2021.3.22) и создать VR-проект.

Сначала удалите плагин Burst (так как он будет мешать работе Interaction Toolkit во время исполнения). Затем, перейдите в настройки проекта и отключите IL2CPP, ибо нам будут нужны только Mono DLLы. Теперь просто сбилдите проект.

Создайте папку с названием `GameReferences` в корне репозитория и скопируйте туда эти файлы из `<название проекта>_Data/Managed`:
- Unity.InputSystem.dll
- Unity.XR.CoreUtils.dll
- Unity.XR.Interaction.Toolkit.dll
- Unity.XR.Management.dll
- Unity.XR.OpenXR.dll

Также скопируйте файлы выше в `BepInEx/core` в вашей директории TABG.

Кроме того, скопируйте эти файлы из `<название проекта>_Data/Plugins/x86_64` в `TotallyAccurateBattlegrounds_Data/Plugins/x86_64` вашей директории TABG:
- openxr_loader.dll
- UnityOpenXR.dll

Создайте папку `UnitySubsystems` в `TotallyAccurateBattlegrounds_Data` с папкой `UnityOpenXR`.
В этой папке, создайте `UnitySubsystemsManifest.json` со следующим содержанием:
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

## Билд проекта
Вам нужно сбилдить проект так же, как вы бы сделали это в обычном .NET проекте.

```shell
dotnet build -c Release TABGVR/TABGVR.csproj
```
*Если TABG установлен у вас в месте, отличном от диска C:, добавьте `-p:TABGFolder="<Путь к TABG>"` к команде.*

Скопируйте `TABGVR/bin/Release/netstandard2.1/TABGVR.dll` в `<путь к бандлу>/BepInEx/plugins` (готовый бандл) или `<директория TABG>/BepInEx/plugins` (самостоятельная сборка)

**Если вы используете готовый бандл, скопируйте файлы бандла в корень папки TABG.**

## Запуск игры
Чтобы запустить игру, см. [SETUP_RU.md](SETUP_RU.md).