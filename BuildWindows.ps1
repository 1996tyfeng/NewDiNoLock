$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectPath = Join-Path $repoRoot "NewDiNoLock"
$unityExe = "E:\Unity\Editor\Unity.exe"
$outputPath = Join-Path $repoRoot "Builds\Windows\NewDiNoLock.exe"
$logPath = Join-Path $repoRoot "Builds\Windows\BuildWindows.log"
$lockFile = Join-Path $projectPath "Temp\UnityLockfile"

Write-Host "NewDiNoLock Windows build"
Write-Host "Project: $projectPath"
Write-Host "Output : $outputPath"
Write-Host ""

if (!(Test-Path -LiteralPath $unityExe)) {
    throw "Unity executable not found: $unityExe"
}

if (Test-Path -LiteralPath $lockFile) {
    Write-Host "Unity Editor appears to be open for this project."
    Write-Host "Please close Unity first, then press Enter to continue."
    Read-Host | Out-Null
}

New-Item -ItemType Directory -Force -Path (Split-Path -Parent $outputPath) | Out-Null

$arguments = @(
    "-batchmode",
    "-quit",
    "-projectPath", $projectPath,
    "-executeMethod", "NewDiNoLock.Editor.BuildAutomation.BuildWindows",
    "-logFile", $logPath
)

Write-Host "Starting Unity build..."
$process = Start-Process -FilePath $unityExe -ArgumentList $arguments -Wait -PassThru

Write-Host ""
Write-Host "Unity exit code: $($process.ExitCode)"

if (Test-Path -LiteralPath $logPath) {
    Write-Host ""
    Write-Host "Last build log lines:"
    Get-Content -LiteralPath $logPath -Tail 80
}

if ($process.ExitCode -ne 0) {
    throw "Unity build failed. See log: $logPath"
}

if (!(Test-Path -LiteralPath $outputPath)) {
    throw "Build finished but executable was not found: $outputPath"
}

Write-Host ""
Write-Host "Build succeeded:"
Write-Host $outputPath
