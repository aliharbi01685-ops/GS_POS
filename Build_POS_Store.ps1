# Build_POS_Store.ps1 — Clean English version for Windows

param (
    [string]$ProjectPath = ".\MAUI_InventoryApp_Arabic_Dark",
    [string]$OutputPath = ".\MAUI_InventoryApp_Arabic_Dark\publish"
)

Write-Host "Building GS_POS APK... Please wait..."

# Ensure project folder exists
if (!(Test-Path $ProjectPath)) {
    Write-Host "❌ Project folder not found!" -ForegroundColor Red
    exit
}

# Create publish folder if not exists
if (!(Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Force -Path $OutputPath | Out-Null
}

# Clean & restore
dotnet clean $ProjectPath
dotnet restore $ProjectPath

# Build APK
Write-Host "Compiling APK..."
dotnet publish $ProjectPath -f:net8.0-android -c Release -o $OutputPath

# Check result
$apk = Get-ChildItem -Path $OutputPath -Filter *.apk -Recurse | Select-Object -First 1

if ($apk) {
    Write-Host "✅ APK generated successfully!" -ForegroundColor Green
    Write-Host "📁 Path: $($apk.FullName)"
} else {
    Write-Host "❌ Failed to build APK. Check .NET MAUI installation." -ForegroundColor Red
}
