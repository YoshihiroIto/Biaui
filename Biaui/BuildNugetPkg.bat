cd /d %~dp0

set biaui_build_nuget=1
cd source

dotnet clean
dotnet build -c Release

cd ..
set biaui_build_nuget=

