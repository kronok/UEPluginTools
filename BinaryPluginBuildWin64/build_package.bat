@echo OFF

if [%1]==[] goto usage

call set_UE5_paths.bat
call set_plugin_config.bat
call set_build_paths.bat %1

echo %BUILD_PACKAGE_PATH%

rmdir /s /q %BUILD_PACKAGE_PATH%
xcopy %BUILD_PLUGIN_PATH% %BUILD_PACKAGE_PATH%\%SCYTHE_PLUGIN_NAME%\ /E /R
copy /Y %PLUGIN_SOURCE_PATH%\%SCYTHE_PLUGIN_NAME%\*.bat %BUILD_PACKAGE_PATH%\%SCYTHE_PLUGIN_NAME%

UEPluginPackager\UEPluginPackager.exe %BUILD_PACKAGE_PATH%\%SCYTHE_PLUGIN_NAME% -EnableByDefault
REM uncomment this to disable shipping w/ plugin build (deletes necessary files)
REM UEPluginPackager\UEPluginPackager.exe %BUILD_PACKAGE_PATH%\%SCYTHE_PLUGIN_NAME% -NoShipping -EnableByDefault

move /Y %BUILD_PACKAGE_PATH%\*.zip BUILDS\%1\
copy /Y %BUILD_PACKAGE_PATH%\*.txt BUILDS\%1\
 
goto :eof

:usage
@echo Usage: %0 ^<VersionString^>
@echo Example: %0 5p3
exit /B 1
