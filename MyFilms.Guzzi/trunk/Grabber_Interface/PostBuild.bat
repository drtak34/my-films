REM %1 = Project Directory
REM %2 = $(ConfigurationName) Debug/Release
echo %1 >%1\postbuild.log
echo %2 >>%1\postbuild.log
rem dir >>%1\postbuild.log
rem copy %1\bin\Debug\*.* %1\Merge >>%1\postbuild.log
del %1\Merge\*.pdb
xcopy %1\bin\%2\*.* %1\Merge /s/r/y >>%1\postbuild.log
cd %1\Merge
rem dir >>%1\postbuild.log
call ilmerge.bat
dir >>%1\postbuild.log
