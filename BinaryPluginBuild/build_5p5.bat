set EngineVersion=5.5
set USEVER=5p5

call set_UE5_paths.bat
call set_plugin_config.bat
call set_build_paths.bat %USEVER%

call build_clean.bat %USEVER%

xcopy %USEVER% %BUILD_PROJECT_PATH%\ /E /R


cd %BUILD_PROJECT_PATH%
call fetch_plugin.bat
echo %EngineVersion% > Plugins\%GS_PLUGIN_NAME%\Source\GS_UNREAL_VERSION.txt
"C:\Program Files (x86)\Epic Games\Launcher\Engine\Binaries\Win64\UnrealVersionSelector.exe" /projectfiles %cd%\UEToolboxBuild%USEVER%.uproject

set PLUGINBUILDPATH=%cd%\..\%BUILD_PLUGIN_PATH%
call %UE5P5ROOT%/Engine/Build/BatchFiles/RunUAT.bat BuildPlugin -plugin="%cd%\Plugins\%GS_PLUGIN_NAME%\%GS_PLUGIN_NAME%.uplugin" -package=%PLUGINBUILDPATH%

cd ..

call build_package.bat %USEVER%
call build_test.bat %USEVER%






