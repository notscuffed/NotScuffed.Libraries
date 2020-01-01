@ECHO OFF
call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\Tools\VsDevCmd.bat">nul
@ECHO OFF

SET output=D:\Projects\Nuget
SET src=%~dp0
SET release=bin\Release

for %%l in (
    NotScuffed.Http
    NotScuffed.Threading
) do (

    :: Clean old release
    echo [90m* Cleaning old release for [35m%%l[0m
    del /Q "%src%\%%l\%release%\*">nul

    :: Compile libary
    echo [90m* Compiling [35m%%l[0m
    msbuild -t:pack -p:Configuration=Release "%src%\%%l">nul
    if NOT %ERRORLEVEL% EQU 0 ( 
        echo [91mFailed to compile %%l[0m
        pause
        exit
    )
    
    :: Copy nuget package
    echo [90m* Copying [35m%%l[0m
    copy /Y "%src%\%%l\%release%\*.nupkg" "%output%">nul
    if NOT %ERRORLEVEL% EQU 0 ( 
        echo [91mFailed to copy %%l[0m
        pause
        exit
    )
)
