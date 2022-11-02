# Arl.Sqlite 

## Simple c# library to enable sqlite query usage much simple & fast

nuget install:
[![Generic badge](https://img.shields.io/nuget/v/ark.sqlite?color=green&label=nuget&style=for-the-badge)](https://www.nuget.org/packages/Ark.Sqlite)
````
    NuGet\Install-Package Ark.Sqlite
````
### Documentation
1. Execute Query
````
    new Ark.Sqlite.SqliteManager("Data Source=database.db").ExecuteQuery("insert into table ..........");
    new Ark.Sqlite.SqliteManager("Data Source=database.db").ExecuteQuery("update table ..........");

````
2. Attach Event

````
    new Ark.Sqlite.SqliteManager("Data Source=database.db").ExecuteSelect<T>(select * from table)
````

