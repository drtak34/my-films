@echo off
IF EXIST AMCUpdater_UNMERGED.exe del AMCUpdater_UNMERGED.exe
ren AMCUpdater.exe AMCUpdater_UNMERGED.exe
ilmerge /out:AMCUpdater.exe AMCUpdater_UNMERGED.exe Grabber.dll XionControls.dll