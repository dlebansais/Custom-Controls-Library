if not exist "C:\Projects\Version Tools\VersionBuilder.exe" goto error

"C:\Projects\Version Tools\VersionBuilder.exe" %1 %2 %3
goto end

:error
rem echo Failed to update version.
goto end

:end
