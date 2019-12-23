using System;
using System.IO;
using System.Net;
using System.Text;

namespace Consume3rdPartyAPI
{
    class Program
    {
        static void Main(string[] args)
        {
             //TO CHECK CONNECTIVITY
            HttpGetSuccessfullyconnectedToAtlasECMMessage();
        }

        //TO CHECK CONNECTIVITY
        public static void HttpGetSuccessfullyconnectedToAtlasECMMessage()
        {
            string Tokenuri = "https://webserverdev.iitvision.com:8493/darbarAPI/rest/hello";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Tokenuri);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            HttpWebResponse respofnse = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(respofnse.GetResponseStream(), Encoding.UTF8);
            var respofnskuie = reader.ReadToEnd();
            Console.WriteLine(respofnskuie);
        }
    }
}
