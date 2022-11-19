using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.Sqlite
{
    public static class ExtnCls
    {
        public static string ToCsv(this DataTable dtDataTable)
        {
            return dtDataTable.ToCsv(true);
        }
        public static string ToCsv(this DataTable dtDataTable, bool header)
        {
            var fn = "temp_local.csv";
            dtDataTable.ToCsv(fn, header);
            return System.IO.File.ReadAllText(fn);
        }
        public static void ToCsv(this DataTable dtDataTable, string strFilePath, bool header)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers    
            if (header)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    sw.Write(dtDataTable.Columns[i]);
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
        public static string ToCsv<T>(this List<T> items) where T : class
        {
            return items.ToCsv<T>(new string[] { });
        }
        //only supported priitive datatype at root level
        public static string ToCsv<T>(this List<T> items, string[] exclude) where T : class
        {
            var output = "";
            var delimiter = ',';
            var properties = typeof(T).GetProperties()
             .Where(n =>
             (n.PropertyType == typeof(string)
             || n.PropertyType == typeof(bool)
             || n.PropertyType == typeof(char)
             || n.PropertyType == typeof(byte)
             || n.PropertyType == typeof(decimal)
             || n.PropertyType == typeof(int)
             || n.PropertyType == typeof(DateTime)
             || n.PropertyType == typeof(DateTime?))
             && !(exclude ?? new string[] { }).Any(n1 => n.Name.Equals(n1, StringComparison.OrdinalIgnoreCase)));
            using (var sw = new StringWriter())
            {
                var header = properties
                .Select(n => n.Name)
                .Aggregate((a, b) => a + delimiter + b);
                sw.WriteLine(header);
                foreach (var item in items)
                {
                    var row = properties
                    .Select(n => n.GetValue(item, null))
                    .Select(n => n == null ? "" : n.ToString())
                    .Select(n => $"\"{n.Replace("\"", "")}\"")
                    .Aggregate((a, b) => a + delimiter + b);
                    sw.WriteLine(row);
                }
                output = sw.ToString();
            }
            return output;
        }
    }
}
