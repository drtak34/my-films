@echo off
IF EXIST AMCUpdater_UNMERGED.exe del AMCUpdater_UNMERGED.exe
ren AMCUpdater.exe AMCUpdater_UNMERGED.exe
rem ilmerge /out:AMCUpdater.exe AMCUpdater_UNMERGED.exe Grabber.dll XionControls.dll Nlog.dll MyFilms.dll /allowDup:Grabber.MyFilmsIMDB
ilmerge /out:AMCUpdater.exe AMCUpdater_UNMERGED.exe Grabber.dll XionControls.dll Nlog.dll
