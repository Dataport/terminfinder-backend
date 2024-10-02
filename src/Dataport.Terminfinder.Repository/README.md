# Repository

## Add Migration

Navigate to the directory `Dataport.Terminfinder.Repository`

```bash
dotnet-ef migrations add <MIGRATIONNAME> --startup-project ../Dataport.Terminfinder.WebAPI
```

Use `dotnet tool update --global dotnet-ef` to update the global tools to the latest available version.  
