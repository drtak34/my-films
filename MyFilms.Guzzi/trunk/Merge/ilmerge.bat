@echo off
IF EXIST MesFilms_UNMERGED.dll del MesFilms_UNMERGED.dll
ren MesFilms.dll MesFilms_UNMERGED.dll
rem ilmerge /out:MesFilms.dll MesFilms_UNMERGED.dll Cornerstone.MP.dll NLog.dll Grabber.dll TaskScheduler.dll
ilmerge /out:MesFilms.dll MesFilms_UNMERGED.dll Cornerstone.MP.dll NLog.dll AMCupdater.exe TaskScheduler.dll