# Schedule Trial: Quartz.NET vs Hangfire

1. Create and init the database
```
PS> cd script
PS> .\drop-database.ps1
...
PS> cd script
PS> .\init-database.ps1
```

2. Run migrations
```
PS> cd src/DataContext/QuartzContext
PS> dotnet add package Microsoft.EntityFrameworkCore.Tools --version 3.1.8
PS> dotnet ef database update 
```