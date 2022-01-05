@echo off
SET version=%1

if "%version%" == "" goto MissingParameterError

echo Packing...
nuget pack -OutputDirectory ..\dist\packages -NonInteractive -Version %version%
if errorlevel 1 goto NugetError

echo Pushing...
nuget push ..\dist\packages\CipherPark.AuroraEngine.%version%.nupkg %Nuget__AuroraEngineKey% -Source https://api.nuget.org/v3/index.json
if errorlevel 1 goto NugetError

exit 0

:MissingParameterError
echo Error: Version parameter required.
goto EndOnError

:NugetError
echo Nuget error occured.
goto EndOnError

:EndOnError
REM - For errors, we end without explicitly exiting the command session.