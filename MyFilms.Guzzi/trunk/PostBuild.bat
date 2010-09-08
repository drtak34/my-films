REM %1 = Solution Directory
REM %2 = $(ConfigurationName) Debug/Release
echo %1 >%1\postbuild.log
echo %2 >>%1\postbuild.log
F:
cd F:\Team Mediaportal
dir >>%1\postbuild.log

copy %1\obj\Debug\MesFilms.dll MediaPortal\trunk\MediaPortal\xbmc\bin\Debug\plugins\Windows

