param(
    [alias("v")]$verbosity='quiet', 
    [alias("c")]$targetConfiguration='Release', 
    [alias("tv")]$targetVersion='1.2.*',
    $msbuild='C:\Program Files (x86)\msbuild\14.0\bin\msbuild.exe')

#Echo parameters
Write-Host Parameters:
Write-Host TargetVersion [tv]: $targetVersion
Write-Host Verbosity [v]: $verbosity
Write-Host Configuration [c]: $targetConfiguration
Write-Host MsBuild path [msbuild]: $msbuild
Write-Host

$currentPath = $PSScriptRoot + '\'

#sources
$projectPath           = '.\CrashReport.Client.csproj'

#do work
$projectPath           = $repositoryPath + $projectPath

function getVersionString($targetVersion){
    $baseDate = [DateTime]::ParseExact('01/01/2000', 'dd/MM/yyyy', [System.Globalization.CultureInfo]::InvariantCulture)
    $span = New-TimeSpan -Start $baseDate -End (Get-Date)
    $build = $span.days

    $baseDate = [DateTime]::Today
    $span = New-TimeSpan -Start $baseDate -End (Get-Date)
    $revision = [math]::Floor($span.TotalSeconds / 2)
    
    $version = "$build.$revision"

    $result =  ($targetVersion | % {$_ -replace '(\*)', $version})
    
    return $result
}

function setAssemblyVersion($projectFile, $version, $lang='cs'){
    if($lang -eq 'cs'){
        $versionRegex = '(\[assembly\: AssemblyVersion\(")([^"]*)("\)\])'
    } else {
        $versionRegex = '(\<Assembly\: AssemblyVersion\(")([^"]*)("\)\>)'    
    }

    $pathToFile = (Split-Path -Path $projectFile -Parent)
    (Get-Content "$pathToFile\Properties\AssemblyInfo.$lang") | % { $_ -replace $versionRegex, "`${1}$version`${3}" } | set-content "$pathToFile\Properties\AssemblyInfo.$lang"
}

#updating assembly info
Write-Host "Updating assembly info..." -foregroundcolor "green"
$version = getVersionString $targetVersion
Write-Host "Setting version: $version"
setAssemblyVersion $projectPath $version
Write-Host

#compile application
Write-Host "Compiling..." -foregroundcolor "green"
$params = 'Platform=AnyCPU;VisualStudioVersion=14.0'
$toolVersion = '14.0'
$configuration = 'Configuration="' + $targetConfiguration + '"'
& $msbuild /v:$verbosity /tv:$toolVersion /p:$params /p:$configuration /t:Rebuild $projectPath
Write-Host

Write-Host "Restoring assembly info..." -foregroundcolor "green"
setAssemblyVersion $projectPath $targetVersion
Write-Host

Write-Host "Done" -foregroundcolor "green"