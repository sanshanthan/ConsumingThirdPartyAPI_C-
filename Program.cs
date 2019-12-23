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

            //TO GET TOKEN FROM ECM
            GETokenFromAtlasECM();
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

        //TO GET TOKEN FROM ECM
        public static void GETokenFromAtlasECM()
        {
            string Tokenuri = "https://webserverdev.iitvision.com:8493/darbarAPI/oauth/token";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Tokenuri);
            request.Method = "POST";
            string postData = "grant_type=client_credentials&client_id=evis&client_secret=g63EgQhyuMG7u8tT";
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            //1.1 Get the response content
            Stream newStream = request.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            HttpWebResponse respofnse = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(respofnse.GetResponseStream(), Encoding.UTF8);
            var respofnskuie = reader.ReadToEnd();
            dynamic obj = JsonConvert.DeserializeObject(respofnskuie);
            var AccessTokenValue = obj.access_token;
            accessToken = AccessTokenValue;
            Console.WriteLine(AccessTokenValue);
        }
    }
}
