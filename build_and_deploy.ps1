# Script de Auto-Compilação e Deploy para Frozen_Elsa
$ProjectRoot = "c:\Users\amaur\Documents\Frozen_Elsa-main"
$TargetDir = "C:\cs2-ds\game\csgo\addons\counterstrikesharp\plugins\Frozen_Elsa"
$BuildConfiguration = "Release" # Pode ser 'Debug' se preferir

Write-Host "--- Iniciando Compilacao ---" -ForegroundColor Cyan

# 1. Compilar o projeto
Set-Location $ProjectRoot
dotnet build --configuration $BuildConfiguration

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro na compilacao! Abortando." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "Compilacao concluida com sucesso." -ForegroundColor Green

# 2. Definir caminhos de origem
$RuntimeVersion = "net8.0" # Altere se a versao do .NET mudar
$SourceBin = Join-Path $ProjectRoot "bin\$BuildConfiguration\$RuntimeVersion"

# 3. Garantir que a pasta de destino existe
if (!(Test-Path $TargetDir)) {
    New-Item -ItemType Directory -Force -Path $TargetDir | Out-Null
    Write-Host "Criada pasta de destino: $TargetDir" -ForegroundColor Yellow
}

# 4. Copiar arquivos relevantes (.dll e pasta lang)
Write-Host "Copiando arquivos para o servidor..." -ForegroundColor Cyan

# Copiar DLL
Copy-Item -Path (Join-Path $SourceBin "Frozen_Elsa.dll") -Destination (Join-Path $TargetDir "Frozen_Elsa.dll") -Force

# Copiar Pasta Lang (se existir)
$SourceLang = Join-Path $SourceBin "lang"
if (Test-Path $SourceLang) {
    $TargetLang = Join-Path $TargetDir "lang"
    Copy-Item -Path $SourceLang -Destination $TargetDir -Recurse -Force
}

# 5. Concluir
Write-Host "--- Plugin Atualizado no Servidor! ---" -ForegroundColor Green
Write-Host "Caminho: $TargetDir"
pause
