set EngineVersion=5.5
set USEVER=5p5

call set_UE5_paths.bat
call set_plugin_config.bat
call set_build_paths.bat %USEVER%

call build_clean.bat %USEVER%

xcopy %USEVER% %BUILD_PROJECT_PATH%\ /E /R


cd %BUILD_PROJECT_PATH%
call fetch_plugin.bat
echo %EngineVersion% > Plugins\%SCYTHE_PLUGIN_NAME%\Source\SCYTHE_UNREAL_VERSION.txt
"C:\Games\Epic Games\Launcher\Engine\Binaries\Win64\UnrealVersionSelector.exe" /projectfiles %cd%\UEToolboxBuild%USEVER%.uproject

echo "UE5p5Root:"
echo %UE5P5ROOT%

set PLUGINBUILDPATH=%cd%\..\%BUILD_PLUGIN_PATH%
call %UE5P5ROOT%/Engine/Build/BatchFiles/RunUAT.bat BuildPlugin -plugin="%cd%\Plugins\%SCYTHE_PLUGIN_NAME%\%SCYTHE_PLUGIN_NAME%.uplugin" -package=%PLUGINBUILDPATH%

cd ..

call build_package.bat %USEVER%
call build_test.bat %USEVER%






