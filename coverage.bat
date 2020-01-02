rem @echo off

if not exist ".\packages\OpenCover.4.7.922\tools\OpenCover.Console.exe" goto error1
if "%WINAPPDRIVER_DIR%" == "" goto error2
if not exist "%WINAPPDRIVER_DIR%/WinAppDriver.exe" goto error2
if "%VSTESTPLATFORM_DIR%" == "" goto error3
if not exist "%VSTESTPLATFORM_DIR%/VSTest.Console.exe" goto error3
if not exist ".\Busy Indicator\bin\x64\Debug\BusyIndicator.dll" goto error4

if exist .\Test\Test-BusyIndicator\obj\x64\Debug\Coverage-BusyIndicator-Debug_coverage.xml del .\Test\Test-BusyIndicator\obj\x64\Debug\Coverage-BusyIndicator-Debug_coverage.xml

call .\coverage\app.bat Debug
call .\coverage\wait.bat 20

call ..\Certification\set_tokens.bat
if exist .\Test\Test-BusyIndicator\obj\x64\Debug\Coverage-BusyIndicator-Debug_coverage.xml .\packages\Codecov.1.9.0\tools\codecov -f ".\Test\Test-BusyIndicator\obj\x64\Debug\Coverage-BusyIndicator-Debug_coverage.xml" -t "%CUSTOMCONTROLSLIBRARY_CODECOV_TOKEN%"
goto end

:error1
echo ERROR: OpenCover.Console not found. Restore it with Nuget.
goto end

:error2
echo ERROR: WinAppDriver not found. Example: set WINAPPDRIVER_DIR=C:\Program Files\Windows Application Driver
goto end

:error3
echo ERROR: Visual Studio 2019 not found. Example: set VSTESTPLATFORM_DIR=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\Extensions\TestPlatform
goto end

:error4
echo ERROR: BusyIndicator.dll not built.
goto end

:end