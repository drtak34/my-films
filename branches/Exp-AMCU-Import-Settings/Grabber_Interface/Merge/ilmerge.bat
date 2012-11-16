@echo off
IF EXIST MyFilms_Grabber_Interface_UNMERGED.exe del MyFilms_Grabber_Interface_UNMERGED.exe
ren MyFilms_Grabber_Interface.exe MyFilms_Grabber_Interface_UNMERGED.exe
ilmerge /out:MyFilms_Grabber_Interface.exe MyFilms_Grabber_Interface_UNMERGED.exe NLog.dll Grabber.dll
