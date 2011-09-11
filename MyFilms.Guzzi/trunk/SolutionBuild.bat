REM %1 = Solution Directory
REM %2 = $(ConfigurationName) Debug/Release
echo %1 >%1\solutionbuild.log
echo %2 >>%1\solutionbuild.log
xcopy %1\AMCupdater\AMCupdater\Merge\AMCUpdater.exe %1\Installer\Base.12 /r/y >>%1\solutionbuild.log
xcopy %1\Grabber_Interface\Merge\MyFilms_Grabber_Interface.exe %1\Installer\Base.12 /r/y >>%1\solutionbuild.log
xcopy %1\Grabber_Plugin\bin\%2\MyVideoGrabber.dll %1\Installer\plugins.12\Windows /r/y >>%1\solutionbuild.log
xcopy %1\MyFilms\Merge\MyFilms.dll %1\Installer\plugins.12\Windows /r/y >>%1\solutionbuild.log
xcopy %1\AMCupdater\AMCupdaterSetup\%2\*.* %1\Installer\Thumbs\MyFilms\AMCinstaller /r/y >>%1\solutionbuild.log

cd %1\Installer
rem dir >>%1\solutionbuild.log

call mpe-maker.bat  >>%1\solutionbuild.log
rem "C:\Programme\Team MediaPortal\MediaPortal\MpeMaker.exe" MyFilms-12.xmp2 /V=1.0.0.0 /B

cd %1\Installer\MPE-Packages
dir >>%1\solutionbuild.log
