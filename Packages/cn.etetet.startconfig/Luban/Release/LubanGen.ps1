# 设置变量
$ErrorActionPreference = "Stop"

$PACKAGE = "Packages/cn.etetet.startconfig"
$CONFIG_NAME = "Release"


$WORKSPACE = "Packages/cn.etetet.yiuiluban"
$GEN_CLIENT = "Packages/cn.etetet.yiuiluban/.Tools/Luban/Luban.dll"
$SOURCE_GEN_CLIENT = "Packages/cn.etetet.yiuiluban/DontNet~/luban/src/Luban/bin/Debug/net10.0/Luban.dll"
$CUSTOM = "Packages/cn.etetet.yiuiluban/.ToolsGen/Custom"
if (!(Test-Path $GEN_CLIENT)) {
    $GEN_CLIENT = $SOURCE_GEN_CLIENT
}

$DotNet = if ($IsWindows) { "dotnet.exe" } else { "dotnet" }
New-Item -ItemType Directory -Force -Path (Join-Path $PACKAGE "Luban/$CONFIG_NAME/Datas") | Out-Null

function Invoke-Luban
{
    & $DotNet $GEN_CLIENT @args
    if ($LASTEXITCODE -ne 0)
    {
        throw "Luban 导出失败，ExitCode=$LASTEXITCODE Args=$($args -join ' ')"
    }
}

Invoke-Luban --customTemplateDir $CUSTOM -t all -c cs-code -d cs-code-data --conf $PACKAGE/Luban/$CONFIG_NAME/luban.conf -x outputCodeDir=$PACKAGE/CodeMode/Model/Server/StartConfig -x outputDataDir=$PACKAGE/CodeMode/Config/Server/$CONFIG_NAME -x configGroup=$CONFIG_NAME
Write-Host "==================== $CONFIG_NAME Server完成 ===================="
