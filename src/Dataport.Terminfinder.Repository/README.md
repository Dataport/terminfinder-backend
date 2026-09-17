# Repository

Use `dotnet tool update --global dotnet-ef` to update the global tools to the latest available version.

## Add Migration

Navigate to the directory `Dataport.Terminfinder.Repository`

```bash
dotnet-ef migrations add <MIGRATIONNAME> --startup-project ../Dataport.Terminfinder.WebAPI
```

### Generate SQL File from existing Migration

The file is created from an existing migration to another. If you want to include everything, set `MIGRATIONID_FROM`
to `0`. The MigrationId is a combination of the timestamp and the migration name, e.g. `20260917102723_AddStatistics`.

```bash
dotnet-ef migrations script <MIGRATIONID_FROM> <MIGRATIONID_TO> --idempotent -o <OUTPUT_PATH> --startup-project ../Dataport.Terminfinder.WebAPI --context DataContext
```