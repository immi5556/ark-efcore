using Microsoft.AspNetCore.Mvc;

namespace Ark.View
{
    public class SqliteController : ControllerBase
    {
        [HttpGet]
        [Route("ark/sqlite/delete/{table}/{del_cond}/{conn_str}")]
        public async Task<dynamic> Delete(string table,string del_cond, string conn_str)
        {
            try
            {
                var dyn = new Ark.Sqlite.SqliteManager($"Data Source=./{conn_str}").ExecuteQuery($"delete from {table} where {del_cond}");
                return new
                {
                    errored = false,
                    message = "deleted success"
                };
            }
            catch (Exception ex) 
            {
                return new
                {
                    errored = true,
                    message = ex.Message
                };
            }
        }
    }
}
