@echo off

setlocal

set RESULTFILENAME=Coverage.xml
set OPENCOVER_VERSION=4.7.922
set OPENCOVER=OpenCover.%OPENCOVER_VERSION%

nuget install OpenCover -Version %OPENCOVER_VERSION% -OutputDirectory packages

if not exist ".\packages\%OPENCOVER%\tools\OpenCover.Console.exe" goto error_console1

if "%WINAPPDRIVER_DIR%" == "" goto error2
if not exist "%WINAPPDRIVER_DIR%/WinAppDriver.exe" goto error2
if "%VSTESTPLATFORM_DIR%" == "" goto error3
if not exist "%VSTESTPLATFORM_DIR%/VSTest.Console.exe" goto error3
if not exist ".\BusyIndicator\bin\x64\Debug\net48\BusyIndicator.dll" goto error4
if not exist ".\DialogValidation\bin\x64\Debug\net48\DialogValidation.dll" goto error4
if not exist ".\EditableTextBlock\bin\x64\Debug\net48\EditableTextBlock.dll" goto error4
if not exist ".\EnumComboBox\bin\x64\Debug\net48\EnumComboBox.dll" goto error4
if not exist ".\EnumRadioButton\bin\x64\Debug\net48\EnumRadioButton.dll" goto error4

if exist .\Test\%RESULTFILENAME% del .\Test\%RESULTFILENAME%

start cmd /k .\coverage\start_winappdriver.bat

call .\coverage\app.bat BusyIndicator Debug
call .\coverage\wait.bat 20

call .\coverage\app.bat DialogValidation Debug "unset"
call .\coverage\wait.bat 30

call .\coverage\app.bat DialogValidation Debug
"%VSTESTPLATFORM_DIR%\VSTest.Console.exe" ".\Test\Test-DialogValidation-UT\bin\x64\Debug\Test-DialogValidation-UT.dll" /Tests:TestDefault1
call .\coverage\wait.bat 2

call .\coverage\app.bat DialogValidation Debug
"%VSTESTPLATFORM_DIR%\VSTest.Console.exe" ".\Test\Test-DialogValidation-UT\bin\x64\Debug\Test-DialogValidation-UT.dll" /Tests:TestDefault2
call .\coverage\wait.bat 2

call .\coverage\app.bat EditableTextBlock Debug "escape1"
"%VSTESTPLATFORM_DIR%\VSTest.Console.exe" ".\Test\Test-EditableTextBlock-UT\bin\x64\Debug\Test-EditableTextBlock-UT.dll" /Tests:TestDefault1
call .\coverage\wait.bat 2

call .\coverage\app.bat EditableTextBlock Debug "escape2"
"%VSTESTPLATFORM_DIR%\VSTest.Console.exe" ".\Test\Test-EditableTextBlock-UT\bin\x64\Debug\Test-EditableTextBlock-UT.dll" /Tests:TestDefault1
call .\coverage\wait.bat 2

call .\coverage\app.bat EditableTextBlock Debug "escape3"
"%VSTESTPLATFORM_DIR%\VSTest.Console.exe" ".\Test\Test-EditableTextBlock-UT\bin\x64\Debug\Test-EditableTextBlock-UT.dll" /Tests:TestDefault1
call .\coverage\wait.bat 2

call .\coverage\app.bat EditableTextBlock Debug "escape4"
"%VSTESTPLATFORM_DIR%\VSTest.Console.exe" ".\Test\Test-EditableTextBlock-UT\bin\x64\Debug\Test-EditableTextBlock-UT.dll" /Tests:TestDefault2
call .\coverage\wait.bat 2

call .\coverage\app.bat EditableTextBlock Debug "escape5"
"%VSTESTPLATFORM_DIR%\VSTest.Console.exe" ".\Test\Test-EditableTextBlock-UT\bin\x64\Debug\Test-EditableTextBlock-UT.dll" /Tests:TestDefault1
call .\coverage\wait.bat 2

call .\coverage\app.bat EditableTextBlock Debug
"%VSTESTPLATFORM_DIR%\VSTest.Console.exe" ".\Test\Test-EditableTextBlock-UT\bin\x64\Debug\Test-EditableTextBlock-UT.dll" /Tests:TestDefault3
call .\coverage\wait.bat 2

call .\coverage\app.bat EnumComboBox Debug
"%VSTESTPLATFORM_DIR%\VSTest.Console.exe" ".\Test\Test-EnumComboBox-UT\bin\x64\Debug\Test-EnumComboBox-UT.dll" /Tests:TestDefault1
call .\coverage\wait.bat 2

call .\coverage\app.bat EnumRadioButton Debug
"%VSTESTPLATFORM_DIR%\VSTest.Console.exe" ".\Test\Test-EnumRadioButton-UT\bin\x64\Debug\Test-EnumRadioButton-UT.dll" /Tests:TestDefault1
call .\coverage\wait.bat 2

call .\coverage\app.bat ExtendedCommandControls Debug
"%VSTESTPLATFORM_DIR%\VSTest.Console.exe" ".\Test\Test-ExtendedCommandControls-UT\bin\x64\Debug\Test-ExtendedCommandControls-UT.dll" /Tests:TestDefault1
call .\coverage\wait.bat 2

start cmd /c .\coverage\stop_winappdriver.bat
call .\coverage\wait.bat 2

call ..\Certification\set_tokens.bat
if exist .\Test\%RESULTFILENAME% .\packages\Codecov.1.9.0\tools\codecov -f ".\Test\%RESULTFILENAME%" -t "%CUSTOMCONTROLSLIBRARY_CODECOV_TOKEN%"
goto end

:error_console1
echo ERROR: OpenCover.Console not found. Restore it with Nuget.
goto end

:error2
echo ERROR: WinAppDriver not found. Example: set WINAPPDRIVER_DIR=C:\Program Files\Windows Application Driver
goto end

:error3
echo ERROR: Visual Studio 2019 not found. Example: set VSTESTPLATFORM_DIR=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\Extensions\TestPlatform
goto end

:error4
echo ERROR: Some assemblies not built.
goto end

:end
