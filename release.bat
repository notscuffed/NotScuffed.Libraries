@ECHO OFF
setlocal EnableDelayedExpansion

SET output=D:\Projects\Nuget
SET src=%~dp0
SET release=bin\Release

for %%l in (
    NotScuffed.Http
    NotScuffed.Threading
	NotScuffed.Linq
	NotScuffed.Strings
	NotScuffed.IO
) do (

    :: Clean old release
    echo [90m* Cleaning old release for [35m%%l[0m
    del /Q "%src%\%%l\%release%\*">nul

    :: Clean nuget cache
    echo [90m* Cleaning nuget cache for [35m%%l[0m
    del /S /Q "%userprofile%\.nuget\packages\%%l\">nul

    :: Compile libary
    echo [90m* Compiling [35m%%l[0m
    dotnet pack --verbosity=quiet --configuration=Release "%src%\%%l">nul
    if !ERRORLEVEL! NEQ 0 ( 
        echo [91mFailed to compile %%l[0m
        pause
        exit
    )
    
    :: Copy nuget package
    echo [90m* Copying [35m%%l[0m
    copy /Y "%src%\%%l\%release%\*.nupkg" "%output%">nul
    if !ERRORLEVEL! NEQ 0 ( 
        echo [91mFailed to copy %%l[0m
        pause
        exit
    )
)
