@echo OFF

if [%1]==[] goto usage

call set_UE5_paths.bat
call set_plugin_config.bat
call set_build_paths.bat %1

rmdir /s /q %BUILD_TEST_PATH%
xcopy %1 %BUILD_TEST_PATH%\ /E /R
xcopy %BUILD_PACKAGE_PATH%\%SCYTHE_PLUGIN_NAME%  %BUILD_TEST_PATH%\Plugins\%SCYTHE_PLUGIN_NAME%\ /E /R

cd %BUILD_TEST_PATH%
"C:\Games\Epic Games\Launcher\Engine\Binaries\Win64\UnrealVersionSelector.exe" /projectfiles %cd%\UEToolboxBuild%1.uproject
cd ..

goto :eof

:usage
@echo Usage: %0 ^<VersionString^>
@echo Example: %0 5p3
exit /B 1

