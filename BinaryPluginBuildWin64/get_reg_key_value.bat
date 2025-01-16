@echo OFF

if [%1]==[] goto usage
if [%2]==[] goto usage


setlocal ENABLEEXTENSIONS
set KEY_NAME=%1
set VALUE_NAME=%2

FOR /F "usebackq tokens=1-3" %%A IN (`REG QUERY %KEY_NAME% /v %VALUE_NAME% 2^>nul`) DO (
    set ValueName=%%A
    set ValueType=%%B
    set ValueValue=%%C
)
if defined ValueName (
	@echo %ValueValue%
) else (
	@echo %KEY_NAME% %VALUE_NAME% DOES NOT EXIST
)
goto :eof

:usage
@echo Usage: %0 ^<RegistryKeyPath^> ^<KeyName^>
exit /B 1

