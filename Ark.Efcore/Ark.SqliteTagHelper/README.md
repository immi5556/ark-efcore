# Arl.Sqlite 

## Simple c# library to enable sqlite query usage much simple & fast

nuget install:
[![Generic badge](https://img.shields.io/nuget/v/ark.sqlite?color=green&label=nuget&style=for-the-badge)](https://www.nuget.org/packages/Ark.Sqlite)
````
    NuGet\Install-Package Ark.Sqlite
````

1. Included sqlite-view - TagHelper

````
    use below code in DI registration at library level
        builder.Services.AddArkView();

    use below in _ViewImports.cshtml
        @addTagHelper *, Ark.Sqlite

    use below in the *.cshtml
        @using Ark.View;
        below html content will render as table
        <sqlite-view Connection-String="church.db" data-qry="select * from audit_log;"></sqlite-view>
````

- all the columns in the querywill be listed as below

PREVIEW
![sqlite-view preview](./sqlite-view-preview.PNG)

TO DOs:

- enable scoket to listen to chagnes and auto refresh
