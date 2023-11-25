using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.Net;
using System.Web;

namespace Ark.View
{
    public class FileController : ControllerBase
    {
        public const int ReadStreamBufferSize = 1024 * 1024;
        public static readonly IReadOnlyCollection<char> InvalidFileNameChars = Array.AsReadOnly(Path.GetInvalidFileNameChars());
        [HttpPost]
        [Route("ark/upload")]
        public async Task<dynamic> Upload()
        {
            try
            {
                var msg = new List<dynamic>();
                foreach (var f in Request.Form.Files)
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
        [HttpGet]
        [Route("ark/file/stream/{file}")]
        public ActionResult<IAsyncEnumerable<string>> FileStream([FromRoute] string file)
        {
            var full_file_path = Path.Combine(Environment.CurrentDirectory, System.Web.HttpUtility.UrlDecode(file));
            return new ActionResult<IAsyncEnumerable<string>>(DoSomeProcessing(full_file_path));
        }
        async IAsyncEnumerable<string> DoSomeProcessing(string path)
        {
            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    yield return await reader.ReadLineAsync();
                }
            }
        }
        [HttpGet]
        [Route("ark/file/stream1/{file}")]
        public HttpResponseMessage FileStream1([FromRoute] string file)
        {
            //var full_file_path = Path.Combine(Environment.CurrentDirectory, System.Web.HttpUtility.UrlDecode(file));
            //List<dynamic> files = new List<dynamic>();
            //using (var reader = new StreamReader(full_file_path))
            //{
            //    while (!reader.EndOfStream)
            //    {
            //        yield return await reader.ReadLineAsync();
            //    }
            //}

            // This can prevent some unnecessary accesses. 
            // These kind of file names won't be existing at all. 
            file = HttpUtility.UrlDecode(file);
            if (string.IsNullOrWhiteSpace(file))
                throw new System.Web.Http.HttpResponseException(HttpStatusCode.NotFound);

            FileInfo fileInfo = new FileInfo(Path.Combine(Environment.CurrentDirectory, file));

            if (!fileInfo.Exists)
                throw new System.Web.Http.HttpResponseException(HttpStatusCode.NotFound);

            long totalLength = fileInfo.Length;

            StringValues? rangeHeader = Request.Headers.Range;
            HttpResponseMessage response = new HttpResponseMessage();

            response.Headers.AcceptRanges.Add("bytes");

            // The request will be treated as normal request if there is no Range header.
            if (rangeHeader.HasValue || !rangeHeader.Value.Any())
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new PushStreamContent((outputStream, httpContent, transpContext)
                =>
                {
                    using (outputStream) // Copy the file to output stream straightforward. 
                    using (Stream inputStream = fileInfo.OpenRead())
                    {
                        try
                        {
                            inputStream.CopyTo(outputStream, ReadStreamBufferSize);
                        }
                        catch (Exception error)
                        {
                            Debug.WriteLine(error);
                        }
                    }
                }, "text/csv");

                response.Content.Headers.ContentLength = totalLength;
                return response;
            }

            long start = 0, end = 0;

            // 1. If the unit is not 'bytes'.
            // 2. If there are multiple ranges in header value.
            // 3. If start or end position is greater than file length.
            if (rangeHeader.Value != "bytes" || rangeHeader.Value.Count > 1 ||
                !TryReadRangeItem(rangeHeader.Value, totalLength, out start, out end))
            {
                response.StatusCode = HttpStatusCode.RequestedRangeNotSatisfiable;
                response.Content = new StreamContent(Stream.Null);  // No content for this status.
                response.Content.Headers.ContentRange = new System.Net.Http.Headers.ContentRangeHeaderValue(totalLength);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(@"text/csv");

                return response;
            }

            var contentRange = new System.Net.Http.Headers.ContentRangeHeaderValue(start, end, totalLength);

            // We are now ready to produce partial content.
            response.StatusCode = HttpStatusCode.PartialContent;
            response.Content = new PushStreamContent((outputStream, httpContent, transpContext)
            =>
            {
                using (outputStream) // Copy the file to output stream in indicated range.
                using (Stream inputStream = fileInfo.OpenRead())
                    CreatePartialContent(inputStream, outputStream, start, end);

            }, "text/csv");

            response.Content.Headers.ContentLength = end - start + 1;
            response.Content.Headers.ContentRange = contentRange;

            return response;
        }

        private static bool AnyInvalidFileNameChars(string fileName)
        {
            return InvalidFileNameChars.Intersect(fileName).Any();
        }

        private static bool TryReadRangeItem(string range, long contentLength,
            out long start, out long end)
        {
            if (range != null)
            {
                start = int.Parse(range.Split('=')[1].Split('-')[0]);
                if (range != null)
                    end = int.Parse(range.Split('=')[1].Split('-')[1]);
                else
                    end = contentLength - 1;
            }
            else
            {
                end = contentLength - 1;
                //if (range.To != null)
                //    start = contentLength - range.To.Value;
                //else
                start = 0;
            }
            return (start < contentLength && end < contentLength);
        }

        private static void CreatePartialContent(Stream inputStream, Stream outputStream,
            long start, long end)
        {
            int count = 0;
            long remainingBytes = end - start + 1;
            long position = start;
            byte[] buffer = new byte[ReadStreamBufferSize];

            inputStream.Position = start;
            do
            {
                try
                {
                    if (remainingBytes > ReadStreamBufferSize)
                        count = inputStream.Read(buffer, 0, ReadStreamBufferSize);
                    else
                        count = inputStream.Read(buffer, 0, (int)remainingBytes);
                    outputStream.Write(buffer, 0, count);
                }
                catch (Exception error)
                {
                    Debug.WriteLine(error);
                    break;
                }
                position = inputStream.Position;
                remainingBytes = end - position + 1;
            } while (position <= end);
        }
    }
}
