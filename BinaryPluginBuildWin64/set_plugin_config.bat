@echo off

REM parse PLUGIN_CONFIG.txt file to get custom strings

REM find PluginName=<PluginName> string
set SCYTHE_PLUGIN_NAME=MyPluginName
for /f "tokens=2 delims==" %%a in ('type PLUGIN_CONFIG.txt^|find "PluginName="') do set SCYTHE_PLUGIN_NAME=%%a

