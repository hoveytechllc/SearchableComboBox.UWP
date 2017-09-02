
if (Test-Path ".\build")
{
    Remove-item -Force -Recurse -Path ".\build"
}

$vsWherePath = Join-Path (Get-Item Env:"ProgramFiles(x86)").Value "Microsoft Visual Studio\Installer\vswhere.exe"

if (!(Test-Path $vsWherePath))
{
    "vswhere.exe Could not be found!"
    return;
}
  
$vsWhereOutput = . $vsWherePath
$msBuildInstallationPath = [regex]::Match($vsWhereOutput, "installationPath: (.+) installationVersion").Groups[1].Value
$msBuildPath = Join-Path $msBuildInstallationPath "MSBuild\15.0\Bin\MSBuild.exe"

if (!(Test-Path $msBuildPath))
{
    "Could not find msbuild.exe"
    return;
}

. $msBuildPath /p:Configuration=Release

. nuget pack .\src\SearchableComboBox.UWP\SearchableComboBox.UWP.nuspec -OutputDirectory .\build

. nuget push .\build\HoveyTech.SearchableComboBox.UWP.*.nupkg -Source https://www.nuget.org/api/v2/package