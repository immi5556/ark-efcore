using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.Sqlite
{
    public class TableScript
    {
        string GetSqliteType(Type col_type)
        {
            if (col_type == null) throw new ArgumentNullException("col_type");
            else if (typeof(string) == col_type) return "TEXT";
            else if (typeof(int) == col_type || typeof(long) == col_type) return "INTEGER";
            else if (typeof(float) == col_type || typeof(decimal) == col_type) return "REAL";
            else if (typeof(byte[]) == col_type) return "BLOB";
            else if (typeof(DateTime) == col_type) return "TEXT";
            else return "TEXT";
        }
        string GetSqliteConstraints(string col_name, ColumnProp col_prop)
        {
            if (col_prop == null) throw new ArgumentNullException("col_prop");
            if (col_prop.Constraints == null) return "";
            if (col_prop.Constraints.Count() == 0) return "";
            var constraints = "";
            foreach (var v in col_prop.Constraints)
            {
                constraints = constraints + $" {GetSqliteConstraint(col_name, v, col_prop.ConstraintName, col_prop.CheckList)}";
            }
            return constraints;
        }
        string GetSqliteConstraint(string col_name, Constraint constraint, string constraintname, object[] checks)
        {
            if (constraint == Constraint.None) return "";
            if (constraint == Constraint.NotNull) return $"CONSTRAINT {(!string.IsNullOrEmpty(constraintname) ? constraintname : $"CST_NN_{SqliteManager.RandomString(7)}")} NOT NULL";
            if (constraint == Constraint.Primary) return $"CONSTRAINT {(!string.IsNullOrEmpty(constraintname) ? constraintname : $"CST_PK_{SqliteManager.RandomString(7)}")} PRIMARY KEY";
            if (constraint == Constraint.Primary_AutoIncrement) return $"CONSTRAINT {(!string.IsNullOrEmpty(constraintname) ? constraintname : $"CST_PKAI_{SqliteManager.RandomString(5)}")} PRIMARY KEY AUTOINCREMENT";
            if (constraint == Constraint.Check)
            {
                if (checks == null) throw new InvalidDataException("check_list");
                if (checks.Count() == 0) throw new InvalidDataException("check_list_empty");
                return $"CONSTRAINT {(!string.IsNullOrEmpty(constraintname) ? constraintname : $"CST_CHK_{SqliteManager.RandomString(6)}")} CHECK ({col_name} in ({(string.Join(',', checks.ToList().Select(x => $"\"{x}\"")))}))";
            }
            if (constraint == Constraint.Default) return $"CONSTRAINT {(!string.IsNullOrEmpty(constraintname) ? constraintname : $"CST_DEF_{SqliteManager.RandomString(5)}")}";
            return "";
            //var col_str = $"{SqliteManager.RemoveSpecialChar(col_name)} {GetSqliteType(prop.DataType)}";
        }
        public string GenerateCreateScript(string table, Dictionary<string, ColumnProp> col_param)
        {
            if (col_param == null) throw new ArgumentNullException("col_param");
            if (col_param.Count == 0) throw new ArgumentNullException("col_param_empty");
            string tbl = $"CREATE TABLE IF NOT EXISTS {table} (";
            int ix = 0;
            col_param.ToList().OrderBy(t => t.Value.Seq).ToList().ForEach(c =>
            {
                if (++ix == col_param.Count) tbl = tbl + $" {c.Key} {GetSqliteType(c.Value.DataType)} {GetSqliteConstraints(c.Key, c.Value)}";
                else tbl = tbl + $" {c.Key} {GetSqliteType(c.Value.DataType)} {GetSqliteConstraints(c.Key, c.Value)},";
            });
            return tbl + ");";
        }
        public string GenerateInsertScript(string table, Dictionary<string, object> col_param)
        {
            var str_ins = $@"INSERT INTO {table} (";
            var str_val = "";
            col_param.ToList().ForEach(c =>
            {
                str_ins = str_ins + $"{c.Key},";
                str_val = str_val + $"'{c.Value}',";
            });
            str_ins = str_ins.TrimEnd(',');
            str_val = str_val.TrimEnd(',');
            return str_ins + $") values ({str_val});";
        }
        public string GenerateUpdateScript(string table, Dictionary<string, object> col_update, Dictionary<string, object> col_where)
        {
            var str_upd = $@"UPDATE {table} set ";
            col_update.ToList().ForEach(c =>
            {
                str_upd = str_upd + $"{c.Key} = '{SqliteManager.ReplaceSpecialChar((c.Value ?? "").ToString(), new Dictionary<string, string?>() { { "'", "" } })}',";
            });
            str_upd = str_upd.TrimEnd(',');
            if (col_where.Count > 0) str_upd = str_upd + " where ";
            col_where.ToList().ForEach(c =>
            {
                str_upd = str_upd + $" {c.Key} = '{SqliteManager.ReplaceSpecialChar((c.Value ?? "").ToString(), new Dictionary<string, string?>() { { "'", "" } })}' AND";
            });
            str_upd = str_upd.TrimEnd("AND".ToCharArray());
            return $"{str_upd};";
        }
    }
    public class ColumnProp
    {
        //public string Name { get; set; }
        public Type DataType { get; set; }
        public Constraint[] Constraints { get; set; }
        public string ConstraintName { get; set; }
        public string Default { get; set; }
        public object[] CheckList { get; set; }
        public int Seq { get; set; }

    }
    public enum Constraint
    {
        None,
        Primary,
        Primary_AutoIncrement,
        Unique,
        Check,
        NotNull,
        Default
    }
}
