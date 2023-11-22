// See https://aka.ms/new-console-template for more information
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Query.Internal;
using tt;
using Ark.Sqlite;

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
//test();
//test2();


void testcsv()
{
    var tg = new Ark.Sqlite.SqliteManager("Data Source=hello.db").ExecuteQuery(@"INSERT INTO records (
                        title,
                        [key],
                        value,
                        ip,
                        at
                    )
                    VALUES (
                        'title',
                        'key',
                        'value',
                        'ip',
                        '11/11/2022 6:09:25 PM'
                    );
");
    var tt = new Ark.Sqlite.SqliteManager("Data Source=hello.db").ExecuteSelect<Record>("select * from records;").ToList();
    var str = tt.ToCsv<Record>();
    Console.WriteLine(str);
}
//testcsv();

void TestDbCreateTbl()
{
    new Ark.Sqlite.SqliteManager("Data Source=hello.db").CreateTable("tabl_test_3",
        new Dictionary<string, ColumnProp>() 
        {
            { "col_id", new ColumnProp() { Seq = 0, DataType = typeof(int), Constraints = new Constraint[] { Constraint.NotNull, Constraint.Primary_AutoIncrement } } },
            { "col_1", new ColumnProp() { Seq = 1, DataType = typeof(string) } },
            { "col_2", new ColumnProp() { Seq = 2, DataType = typeof(string), Constraints = new Constraint[] { Constraint.Check }, CheckList = new object[] { "test_1", "test_2" } } }
        }
        );
}

//TestDbCreateTbl();

void InsertScriptTbl()
{
    new Ark.Sqlite.SqliteManager("Data Source=hello.db").InsertTable("tabl_test_3",
        new Dictionary<string, object>
        {
            //{ "col_id", 123 },
            { "col_1", "dfsdfdsf" },
            { "col_2", "test_2" }
            //{ "col_2", "wewewe" }
        }
        );
}

InsertScriptTbl();

void UpdateScriptTbl()
{
    new Ark.Sqlite.SqliteManager("Data Source=hello.db").UpdateTable("tabl_test_3",
        new Dictionary<string, object>
        {
            //{ "col_id", 123 },
            { "col_1", "12122" },
            { "col_2", "test_2" }
            //{ "col_2", "wewewe" }
        },
        new Dictionary<string, object>
        {
            { "col_1", "12122"  }
        }
        );
}

UpdateScriptTbl();

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