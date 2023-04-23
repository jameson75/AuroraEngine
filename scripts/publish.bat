@echo off
SET version=%1
SET packageEnvironment=%2
SET codeConfig=%3
SET source=0
SET localNugetDir="C:\.nuget-development\AuroraEngine"
mkdir %localNugetDir%

if "%version%" == "" goto MissingVersionParameterError
if "%packageEnvironment%" == "" set packageEnvironment="prod"
if "%codeConfig%" == "" set codeConfig="Debug"
if "%packageEnvironment%" == "prod" set source="https://api.nuget.org/v3/index.json"
if "%packageEnvironment%" == "dev" set source=%localNugetDir%
if "%source%" == "0" goto BadPackageEnviornmentError

echo Packing Aurora Engine...
nuget pack Package.AuroraEngine.%codeConfig%.nuspec -OutputDirectory ..\dist\packages -NonInteractive -Version %version%
if errorlevel 1 goto NugetError

echo Packing Aurora Editor...
nuget pack Package.AuroraEditor.%codeConfig%.nuspec -OutputDirectory ..\dist\packages -NonInteractive -Version %version%
if errorlevel 1 goto NugetError

echo Pushing Aurora Engine...
nuget push ..\dist\packages\CipherPark.AuroraEngine.%version%.nupkg %Nuget__AuroraEngineKey% -Source %source%
if errorlevel 1 goto NugetError

echo Pushing Aurora Editor...
nuget push ..\dist\packages\CipherPark.AuroraEditor.%version%.nupkg %Nuget__AuroraEngineKey% -Source %source%
if errorlevel 1 goto NugetError

echo [42mPublish finished successfully.[0m
goto End

:MissingVersionParameterError
echo [101mError: Version parameter required.[0m
goto End

:NugetError
echo [101mNuget error occured.[0m
goto End

:BadPackageEnviornmentError
echo [101mError: Unrecognized package environment. Environment must be 'dev' or 'prod'.[0m
goto End

:End
REM - We end without explicitly exiting the command session.
