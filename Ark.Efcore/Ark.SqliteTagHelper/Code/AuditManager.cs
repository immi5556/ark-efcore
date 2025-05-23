namespace Ark
{
    public class AuditManager
    {
        public static List<dynamic> UknownLogs { get; set; } = new List<dynamic>();
        public static Func<string?, string> cleanup = str => Ark.Sqlite.SqliteManager.ReplaceSpecialChar(str ?? "", new Dictionary<string, string?>() { { "'", "''" } });
        static string con_str = $"Data Source=./ark_audit_{ark.net.util.DateUtil.CurrentMonthStamp()}.db";
        static AuditManager()
        {
        }
        static string _tbl = $"audit_trace_{ark.net.util.DateUtil.CurrentDateStamp()}";
        public static async Task CreateAuditTraceTable()
        {
            new Sqlite.SqliteManager(con_str).CreateTable(_tbl, new Dictionary<string, Sqlite.ColumnProp>()
            {
                { "id", new Sqlite.ColumnProp() { Seq = 0, DataType = typeof(long), ConstraintName = $"PK_Audit_Trace_{ark.net.util.DateUtil.CurrentTimeStamp()}", Constraints = new Sqlite.Constraint[] { Sqlite.Constraint.Primary_AutoIncrement, Sqlite.Constraint.NotNull } } },
                { "ref_key", new Sqlite.ColumnProp() { Seq = 3, DataType = typeof(string) } },
                { "ref_val", new Sqlite.ColumnProp() { Seq = 5, DataType = typeof(string) } },
                { "log_type", new Sqlite.ColumnProp() { Seq = 10, DataType = typeof(string) } },
                { "message", new Sqlite.ColumnProp() { Seq = 15, DataType = typeof(float) } },
                { "details", new Sqlite.ColumnProp() { Seq = 16, DataType = typeof(string) } },
                { "by", new Sqlite.ColumnProp() { Seq = 30, DataType = typeof(string) } },
                { "ip", new Sqlite.ColumnProp() { Seq = 35, DataType = typeof(string) } },
                { "at", new Sqlite.ColumnProp() { Seq = 40, DataType = typeof(DateTime) } }
            });
        }
        public static async Task LogError(Exception exp, string? ref_key, string? ref_val, string? msg)
        {
            try
            {
                await CreateAuditTraceTable();
                new Sqlite.SqliteManager(con_str).InsertTable(_tbl, new Dictionary<string, object>()
                {
                    { "ref_key", cleanup(ref_key) },
                    { "ref_val", cleanup(ref_val) },
                    { "log_type", "exception" },
                    { "message", cleanup(msg) },
                    { "details", exp?.ToString() },
                    { "by", "web_hook" },
                    { "ip", "" },
                    { "at", cleanup(DateTime.UtcNow.ToString("yyyyMMdd.hhmmss.fff.zzz")) }
                });
            }
            catch (Exception ex)
            {
                UknownLogs.Add(new { at = DateTime.Now, action_type = "AuditError", error = ex.ToString(), msg = "Audit log errorred." });
                //TODO: what to do
            }
        }
        public static async Task Log(string log_type, string? ref_key, string? ref_val, string? msg, string? detail)
        {
            try
            {
                await CreateAuditTraceTable();
                new Sqlite.SqliteManager(con_str).InsertTable(_tbl, new Dictionary<string, object>()
                {
                    { "ref_key", cleanup(ref_key) },
                    { "ref_val", cleanup(ref_val) },
                    { "log_type", log_type }, //info, warn, suc
                    { "message", msg },
                    { "details", detail },
                    { "by", "web_hook" },
                    { "ip", "" },
                    { "at", cleanup(DateTime.UtcNow.ToString("yyyyMMdd.hhmmss.fff.zzz")) }
                });
            }
            catch (Exception ex)
            {
                UknownLogs.Add(new { at = DateTime.Now, action_type = "AuditError", error = ex.ToString(), msg = "Audit trace log errorred." });
                //TODO: what to do
            }
        }
    }
}
