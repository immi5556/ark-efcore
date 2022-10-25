# ark-efcore
 efcore multi provider datacontxt

 
---migrations

install gloablly cli tool
> dotnet tool install --global dotnet-ef

> dotnet ef migrations add basev1 --project ./Ark.Efcore.Web -- --provider Sqlite

> dotnet ef migrations add <Migration Name> --project ./Ark.Postgres -- --provider Postgres
