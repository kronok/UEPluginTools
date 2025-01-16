@echo OFF

if [%1]==[] goto usage

call set_UE5_paths.bat
call set_build_paths.bat %1

rmdir /s /q %BUILD_PROJECT_PATH%
rmdir /s /q %BUILD_PLUGIN_PATH%
rmdir /s /q %BUILD_PACKAGE_PATH%
rmdir /s /q %BUILD_TEST_PATH%

goto :eof

:usage
@echo Usage: %0 ^<VersionString^>
@echo Example: %0 5p3
exit /B 1