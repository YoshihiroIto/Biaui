echo off
cd /d %~dp0

set PS=%SystemRoot%\system32\WindowsPowerShell\v1.0\powershell.exe

%PS% -NoProfile -ExecutionPolicy ByPass -NonInteractive -File ./script/SetupDevEnv.ps1

