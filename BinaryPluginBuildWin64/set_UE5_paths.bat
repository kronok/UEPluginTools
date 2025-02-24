@echo off

for /f %%i in ('get_reg_key_value.bat "HKEY_LOCAL_MACHINE\SOFTWARE\EpicGames\Unreal Engine\5.3" InstalledDirectory') do set UE5P3ROOT=%%i
REM set UE5P3ROOT=<hardcode path>

for /f %%j in ('get_reg_key_value.bat "HKEY_LOCAL_MACHINE\SOFTWARE\EpicGames\Unreal Engine\5.4" InstalledDirectory') do set UE5P4ROOT=%%j
REM set UE5P4ROOT=<hardcode path>

for /f %%k in ('get_reg_key_value.bat "HKEY_LOCAL_MACHINE\SOFTWARE\EpicGames\Unreal Engine\5.5" InstalledDirectory') do set UE5P5ROOT=%%k
set UE5P5ROOT="C:\Epic Games\UE_5.5"

REM future use
REM for /f %%k in ('get_reg_key_value.bat "HKEY_LOCAL_MACHINE\SOFTWARE\EpicGames\Unreal Engine\5.6" InstalledDirectory') do set UE5P6ROOT=%%k
REM set UE5P6ROOT=<hardcode path>

REM @echo %UE5P3ROOT%
REM @echo %UE5P4ROOT%
REM @echo %UE5P5ROOT%
REM @echo %UE5P6ROOT%
