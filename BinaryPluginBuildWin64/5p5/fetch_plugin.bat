set PLUGIN_SOURCES_DIR=..\PluginSources

rmdir /s /q Plugins

xcopy %PLUGIN_SOURCES_DIR%\Scythe\ .\Plugins\Scythe\ /E /R

