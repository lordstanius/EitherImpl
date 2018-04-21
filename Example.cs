using System;
using System.IO;
using System.Net;

namespace EitherImpl
{
    /// <summary>
    /// How to consume Result class
    /// </summary>
    public class Example
    {
        static void Main(string[] args)
        {
            Uri address = new Uri("http://www.digger.org/index.html");

            Result<Resource, Error> result = FetchContent(address);

            string content = result.Try(res => res.Data).Catch(err => err.Message);
            Console.WriteLine(content);

            // or
            result
                .Try(res => Console.WriteLine(res.Data))
                .Catch(err => Console.WriteLine(err.Message));
        }

        static Result<Resource, Error> FetchContent(Uri address)
        {
            // Uncomment line below to see what happens in a case of failure
            //return Result.Create<Resource, Error>(new NotFound());

            var request = WebRequest.Create(address);
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return Result.Create<Resource, Error>(new NotFound());
                }

                if (response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.TemporaryRedirect)
                {
                    Uri redirectUri = new Uri(response.Headers[HttpResponseHeader.Location]);
                    return Result.Create<Resource, Error>(new Moved(redirectUri));
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return Result.Create<Resource, Error>(new Error());
                }

                Stream dataStream = response.GetResponseStream();
                string data = new StreamReader(dataStream).ReadToEnd();
                return Result.Create<Resource, Error>(new Resource(data));
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.Timeout)
            {
                return Result.Create<Resource, Error>(new TimeOut());
            }
            catch (WebException ex)
            {
                return Result.Create<Resource, Error>(new Error(ex.Message));
            }
        }

        /// <summary>
        /// Base "failure" type
        /// </summary>
        class Error
        {
            public string Message { get; protected set; }

            public Error()
            {
                Message = "Operation failed";
            }

            public Error(string message) : this()
            {
                Message = $"{Message}: {message}";
            }
        }

        class NotFound : Error
        {
            public NotFound() : base("Resource not found")
            {
            }
        }

        class Moved : Error
        {
            public Uri RedirectUri { get; }

            public Moved(Uri redirectUri) : base($"Moved to {redirectUri}")
            {
                RedirectUri = redirectUri;
            }
        }

        class TimeOut : Error
        {
            public TimeOut() : base("Connection timeout")
            {
            }
        }

        /// <summary>
        /// This is "success" type
        /// </summary>
        class Resource
        {
            public string Data { get; }

            public Resource(string data)
            {
                Data = data;
            }
        }
    }
}

/*
This is free and unencumbered software released into the public domain.
Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
For more information, please refer to <http://unlicense.org/>
*/
