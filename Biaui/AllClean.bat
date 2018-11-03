rem echo off
cd /d %~dp0

set PS=%SystemRoot%\system32\WindowsPowerShell\v1.0\powershell.exe

%PS% -NoProfile -ExecutionPolicy ByPass -NonInteractive -File external/build.ps1 -Script source/build.cake -target=Clean -configuration=Debug;Release

