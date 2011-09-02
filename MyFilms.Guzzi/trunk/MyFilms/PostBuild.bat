REM %1 = Project Directory
REM %2 = $(ConfigurationName) Debug/Release
echo %1 >%1\postbuild.log
echo %2 >>%1\postbuild.log
xcopy %1\bin\Debug\*.* %1\Merge /s/r/y >>%1\postbuild.log
cd %1\Merge
rem dir >>%1\postbuild.log
call ilmerge.bat  >>%1\postbuild.log
dir >>%1\postbuild.log
