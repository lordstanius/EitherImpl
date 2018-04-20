using System;
using System.IO;
using System.Net;

namespace EitherImpl
{
    public class Failure
    {
    }

    public class NotFound : Failure
    {
    }

    public class Moved : Failure
    {
        public Uri RedirectUri { get; }

        public Moved(Uri redirectUri)
        {
            RedirectUri = redirectUri;
        }
    }

    public class Timeout : Failure
    {
    }

    public class Resource
    {
        public string Data { get; }

        public Resource(string data)
        {
            Data = data;
        }
    }

    class Program
    {
        static Result<Resource, Failure> Fetch(Uri address)
        {
            //return Result.Create<Resource, Failure>(new Failure());

            var request = WebRequest.Create(address);
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return Result.Create<Resource, Failure>(new NotFound());
                }

                if (response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.TemporaryRedirect)
                {
                    Uri redirectUri = new Uri(response.Headers[HttpResponseHeader.Location]);
                    return Result.Create<Resource, Failure>(new Moved(redirectUri));
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return Result.Create<Resource, Failure>(new Failure());
                }

                Stream dataStream = response.GetResponseStream();
                string data = new StreamReader(dataStream).ReadToEnd();
                return Result.Create<Resource, Failure>(new Resource(data));
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.Timeout)
            {
                return Result.Create<Resource, Failure>(new Timeout());
            }
            catch (WebException)
            {
                return Result.Create<Resource, Failure>(new Failure());
            }
        }

        static void Main(string[] args)
        {
            Uri address = new Uri("http://www.digger.org/windows.html");
            var result = Fetch(address);

            string report = result.Handle(res => res.Data).Reduce(err => "Failed");

            Console.WriteLine(report);
        }
    }
}
