using Microsoft.AspNetCore.Mvc;

namespace Ark.View
{
    public class FileController : ControllerBase
    {
        [HttpPost]
        [Route("ark/upload")]
        public async Task<dynamic> Upload()
        {
            try
            {
                var msg = new List<dynamic>();
                foreach(var f in Request.Form.Files)
                {
                    if (!Directory.Exists("./ArkUpload")) Directory.CreateDirectory("./ArkUpload");
                    var uq_fn = $"./ArkUpload/{System.IO.Path.GetFileNameWithoutExtension(f.FileName)}_{DateTime.Now.ToString("yyyMMddhhmmssfff")}{System.IO.Path.GetExtension(f.FileName)}";
                    using (FileStream strm = new FileStream($"{uq_fn}", FileMode.CreateNew))
                    {
                        f.CopyTo(strm);
                    }
                    msg.Add(new
                    {
                        file_url = uq_fn
                    });
                }
                return new
                {
                    errored = false,
                    msg = msg,
                    message = "uploaded success"
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
