@echo off
IF EXIST MyFilms_UNMERGED.dll del MyFilms_UNMERGED.dll
ren MyFilms.dll MyFilms_UNMERGED.dll
ilmerge /out:MyFilms.dll MyFilms_UNMERGED.dll NLog.dll Grabber.dll TaskScheduler.dll
rem ilmerge /out:MesFilms.dll MesFilms_UNMERGED.dll Cornerstone.MP.dll NLog.dll AMCupdater.exe TaskScheduler.dll