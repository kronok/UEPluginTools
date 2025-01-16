@echo OFF

if [%1]==[] goto usage

set PLUGIN_SOURCE_PATH=%cd%\PluginSources
set SOURCE_PATH=%1
set BUILD_PROJECT_PATH=%1_build
set BUILD_PLUGIN_PATH=%1_build_plugin
set BUILD_PACKAGE_PATH=%1_build_plugin_binarypkg
set BUILD_TEST_PATH=%1_build_test


goto :eof

:usage
@echo Usage: %0 ^<VersionString^>
@echo Example: %0 5p3
exit /B 1
