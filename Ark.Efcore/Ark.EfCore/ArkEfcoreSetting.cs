using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.EfCore
{
    public class ArkEfcoreSetting
    {
        public string provider { get; set; } //sql, mysql, posgres, sqlite
        public string connection_string { get; set; }
        public string assembly_name { get; set; } // dll path for migration
    }
    //public record Provider(string Name, string Assembly)
    //{
    //    public static Provider Sqlite = new(nameof(Sqlite), typeof(Sqlite.Marker).Assembly.GetName().Name!);
    //    public static Provider Postgres = new(nameof(Postgres), typeof(Postgres.Marker).Assembly.GetName().Name!);
    //}
}
