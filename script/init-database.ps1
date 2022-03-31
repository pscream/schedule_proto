. .\common.ps1 # Import Invoke-SqlScripts

$configFile = "config.json"
$scriptFolderName = "init-data"

$rootDir=$MyInvocation.MyCommand.Path | Split-Path

Invoke-SqlScripts "$rootDir\$configFile" "$rootDir\$scriptFolderName" "\d\d\d-create-database.sql"
