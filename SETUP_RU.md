# Руководство по установке

> [!CAUTION]
> **Внимание:** Из-за того, как устроен мод, мы не можем гарантировать что мод не будет помечен как чит. Если вы хотите поиграть, как обычно, без VR и ограничений в матчмейкинге, которые принудительны для вашей безопасности, переименуйте winhttp.dll на любое другое имя для остановки инжектора. **УДАЛЕНИЕ/ПЕРЕМЕЩЕНИЕ ПЛАГИНА В ФАЙЛАХ НЕ ОСТАНОВИТ ИНЖЕКТОР И EAC ОБНАРУЖИТ ПОДОЗРИТЕЛЬНЫЕ ФАЙЛЫ!**

## Установка билда
Перейдите [сюда](https://github.com/RedBigz/TABGVR/actions) для выбора артефакта.

> [!TIP]
> **Совет:** в списке вы найдёте обрезанные SHA-1 хеши Git-коммита, из которого он был скомпилирован. Если Вы проверяете репозиторий в IDE, хэш этого коммита будет совпадать с найденным командой `git checkout -1 --oneline`.

Всё, что вам нужно сделать для установки TABGVR, это скачать артефакт в самом низу страницы и переместить его в корень вашей папки TABG.

## Запуск игры
Запуск игры напрямую через Steam приведёт к запуску игрой EAC лаунчера, что вызовет краш игры и ошибке о ненадёжном файле. Есть несколько вариантов обхода лаунчера, которые будут описаны здесь:

### Запуск напрямую из папки игры
Если в папке нет файла `steam_appid.txt`, создайте его с текстом `823130`.
Затем, просто запустите `TotallyAccurateBattlegrounds.exe` прямо из проводника, либо создайте для него ярлык.

### Запуск через параметры запуска Steam
Просто добавьте это в параметры запуска игры:
```
"<папка библиотеки steam>\steamapps\common\TotallyAccurateBattlegrounds\TotallyAccurateBattlegrounds.exe" %command% <остальные ваши параметры запуска здесь>"
```
#### Пример (диск C):
```
"C:\Program Files (x86)\Steam\steamapps\common\TotallyAccurateBattlegrounds\TotallyAccurateBattlegrounds.exe" %command%"
```

## Сообщить о баге
Если вы столкнётесь с проблемами, не стесняйтесь создать [issue](https://github.com/RedBigz/TABGVR/issues) в данном репозитории.

*Приятной игры в TABG VR! :3*