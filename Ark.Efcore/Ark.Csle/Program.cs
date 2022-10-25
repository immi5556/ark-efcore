// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

string constr = "Data Source=hello.db";
new Ark.Sqlite.SqliteManager(constr).CreateTable(@"CREATE TABLE  IF NOT EXISTS records (
    record_id INTEGER NOT NULL
                      CONSTRAINT PK_records PRIMARY KEY AUTOINCREMENT,
    title     TEXT,
    [key]     TEXT,
    value     TEXT,
    ip        TEXT,
    at        TEXT
);
");

new Ark.Sqlite.SqliteManager("Data Source=hello.db").ExecuteQuery(@"INSERT INTO records (
                        title,
                        [key],
                        value,
                        ip
                    )
                    VALUES (
                        'title',
                        'key',
                        'value',
                        'ip'
                    );
");
var ttt = new Ark.Sqlite.SqliteManager("Data Source=hello.db").ExecuteSelect<tt.Test>("select * from records");
Console.WriteLine("completed...{0}", ttt.Count());
namespace tt
{
    class Test
    {

    }
}