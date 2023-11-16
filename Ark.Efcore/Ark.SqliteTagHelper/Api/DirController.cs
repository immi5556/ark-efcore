using Microsoft.AspNetCore.Mvc;

namespace Ark.View
{
    public class DirController : ControllerBase
    {
        [HttpGet]
        [Route("ark/delete/{file_uri}")]
        public async Task<dynamic> Delete(string file_uri)
        {
            try
            {
                if (!System.IO.File.Exists(file_uri)) throw new ApplicationException("file not found");
                System.IO.File.Delete(file_uri);
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
