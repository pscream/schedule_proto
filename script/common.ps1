function Invoke-SqlScripts ([string] $configFile, [string] $scriptFolder, [string] $globalScriptPattern) {

    # Get configuration data from a config file
    $config = Get-Content $configFile -Raw -Verbose | ConvertFrom-Json
    
    $connectionString = "Server=$($config.serverInstance);Database=$($config.databaseName);Trusted_Connection=True;MultipleActiveResultSets=True"
    Write-Output "ConnectionString=""$connectionString"""
    
    # Enumerate sql script files
    # Note asterix in "$scriptDir\*". Without it '-Include' doesn't work properly and returns no lines 
    $scriptFiles = Get-ChildItem -Path "$scriptFolder\*" -Include "*.sql" | Where-Object {!$_.PSIsContainer} | Sort-Object {$_.Name} | Foreach-Object {$_.Name}
    
    # Iterate for each script file in the folder
    $scriptFiles | ForEach-Object { 
        Write-Output "$($_):" 
        if ( $_ -match $globalScriptPattern ) {
            # If it's the create database script then we should use '-ServerInstance' instead of '-ConnectionString'
            Write-Output "We will run it at the global level!"
            Invoke-Sqlcmd -InputFile "$scriptFolder\$_" -ServerInstance $config.serverInstance -OutputSqlErrors $true -Verbose
        }
        else {
            # If it's just a regular script we will use '-ConnectionString'
            Invoke-Sqlcmd -InputFile "$scriptFolder\$_" -ConnectionString $connectionString -OutputSqlErrors $true -Verbose
        }
    }
    
}