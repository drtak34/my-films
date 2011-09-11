REM %1 = Project Directory
REM %2 = $(ConfigurationName) Debug/Release
echo %1 >%1\postbuild.log
echo %2 >>%1\postbuild.log
del %1\Merge\*.pdb
xcopy %1\bin\%2\*.* %1\Merge /s/r/y >>%1\postbuild.log
cd %1\Merge
call ilmerge.bat
dir >>%1\postbuild.log
