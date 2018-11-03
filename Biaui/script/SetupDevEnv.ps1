[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12

# Output directory
if (Test-Path external)
{
    Remove-Item -path external -recurse -force
}
New-Item -name external -type directory


# --[Cake]--------------------------------------------------------------
Invoke-WebRequest https://cakebuild.net/download/bootstrapper/windows -OutFile external\build.ps1


# --[DocFX]--------------------------------------------------------------
Invoke-WebRequest -Uri https://github.com/dotnet/docfx/releases/download/v2.39.2/docfx.zip -OutFile docfx.zip
Expand-Archive -Path docfx.zip -DestinationPath external\docfx
Remove-Item docfx.zip



