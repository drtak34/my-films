@ECHO OFF

REM detect if BUILD_TYPE should be release or debug
if not %1!==Debug! goto RELEASE
:DEBUG
set BUILD_TYPE=Debug
goto START
:RELEASE
set BUILD_TYPE=Release
goto START


:START
REM Select program path based on current machine environment
set progpath=%ProgramFiles%
if not "%ProgramFiles(x86)%".=="". set progpath=%ProgramFiles(x86)%


REM set logfile where the infos are written to, and clear that file
set LOG=build_%BUILD_TYPE%.log
echo. > %LOG%


echo.
echo -=- MyFilms -=-
echo -=- build mode: %BUILD_TYPE% -=-
echo.

echo.
echo Building MyFilms...
"%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBUILD.exe" /target:Rebuild /property:Configuration=%BUILD_TYPE% "..\MyFilmsSuite.sln" >> %LOG%

rem echo Building MPEI
rem copy "..\MPEI\MyFilms_MP12.xmp2" "..\MPEI\MyFilms_MP12_COPY.xmp2"
rem "%progpath%\Team MediaPortal\MediaPortal\MpeMaker.exe" "..\MPEI\MyFilms_MP12_COPY.xmp2" /B >> %LOG%
rem del "..\MPEI\MyFilms_MP12_COPY.xmp2"

echo.
echo Building MyFilms done.
echo.
echo MPE package can be found in Installer\MPE-Packages
echo.
