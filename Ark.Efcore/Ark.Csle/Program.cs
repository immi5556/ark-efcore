// See https://aka.ms/new-console-template for more information
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Query.Internal;

Console.WriteLine("Hello, World!");

void test()
{
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

    var tg = new Ark.Sqlite.SqliteManager("Data Source=hello.db").ExecuteQuery(@"INSERT INTO records (
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
    Console.WriteLine((long)tg);
    var ttt = new Ark.Sqlite.SqliteManager("Data Source=hello.db").ExecuteSelect<tt.Test>("select * from records");

    Console.WriteLine("completed...{0}", ttt.Count());
}

void test2()
{
    var tg = new Ark.Sqlite.SqliteManager("Data Source=hello.db").ExecuteQuery(@"INSERT INTO records (
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
    Console.WriteLine(tg);
}
void test3()
{
    var tg = new Ark.Sqlite.SqliteManager("Data Source=hello.db").ExecuteQuery(@"INSERT INTO records (
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
    Console.WriteLine(tg);
}
test();
test2();

namespace tt
{

    public class Record
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? record_id { get; set; }
        public string title { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public string ip { get; set; }
        public DateTime? at { get; set; } = DateTime.UtcNow;
    }

    class Test
    {

    }
}