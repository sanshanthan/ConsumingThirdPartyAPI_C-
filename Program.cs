using System;
using System.Collections.Specialized;
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

            //DOWNLOAD FILE FROM ECM
            DownloadFilesFromECM();

           //UPLOAD FILE TO ECM
            UploadFilesToECM();
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

        //DOWNLOAD FILE FROM ECM
        static void DownloadFilesFromECM()
        {
            var pathapi = "https://webserverdev.iitvision.com:8493/darbarAPI/rest/docs/2607286c-61a2-4f65-bf36-49b91d776b99/attachments/201905/5674f64f-1efa-4c03-af54-8a01fa048440.C_MCSA.jpg";
            var AccessTokenValue = "2ea1d3aa-45e8-4db9-9bc3-a6681a73110b";
            var localFolder = @"c:\temp\1\";
            var filename = "file.jpg";
            try
            {
                WebRequest request = WebRequest.Create(pathapi);
                request.Method = "GET";
                request.Headers.Add("Authorization", "Bearer " + AccessTokenValue);
                request.Headers.Add("EcmUserId", "adam.burnet");
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream streams = response.GetResponseStream())
                    {
                        using (FileStream localFileStream = new FileStream(Path.Combine(localFolder, filename), FileMode.Create))
                        {
                            var buffer = new byte[4096];
                            long totalBytesRead = 0;
                            int bytesRead;

                            while ((bytesRead = streams.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                totalBytesRead += bytesRead;
                                localFileStream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //UPLOAD FILE TO ECM
        static void UploadFilesToECM()
        {
            var postUrl = "https://webserverdev.iitvision.com:8493/darbarAPI/rest/docs";

            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("meta", "{ \"summary\":\"test\", " +
                "\"owners\":[ \"AtlasECM_user\" ], " +
                "\"managers\":[ \"adam.burnet\" ], " +
                "\"receivedDate\":\"2016-07-25T16:54:34+1000\", " +
                "\"status\":\"20\", " +
                "\"folders\":[ \"b26e2b79-1693-4138-b42e-ac49b46104eb\" ], " +
                "\"docType\":\"INT\", " +
                "\"contentType\":\"1\", " +
                "\"extRefNo\":\"123456\", " +
                "\"htmlContent\":\"Sample Content\" }");
            string[] arr = new string[2];
            arr[0] = @"C:/MCSA.jpg";
            arr[1] = @"C:/tempsnip.png";
            var tets = UploadFilesToRemoteUrl(postUrl, arr, nvc);
            Console.WriteLine(tets);
            Console.ReadLine();
        }

        public static string UploadFilesToRemoteUrl(string url, string[] files, NameValueCollection formFields = null)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Headers.Add("Authorization", "Bearer 8a2ffb1c-43f1-4b9e-b64a-dca79aeda342");

            Stream memStream = new System.IO.MemoryStream();

            var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            var endBoundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--");
            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

            if (formFields != null)
            {
                foreach (string key in formFields.Keys)
                {
                    string formitem = string.Format(formdataTemplate, key, formFields[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    memStream.Write(formitembytes, 0, formitembytes.Length);
                }
            }

            string headerTemplate =
                "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                "Content-Type: application/octet-stream\r\n\r\n";

            for (int i = 0; i < files.Length; i++)
            {
                memStream.Write(boundarybytes, 0, boundarybytes.Length);
                var header = string.Format(headerTemplate, "uplTheFile", files[i]);
                var headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                memStream.Write(headerbytes, 0, headerbytes.Length);
                using (var fileStream = new FileStream(files[i], FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[1024];
                    var bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        memStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            request.ContentLength = memStream.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                memStream.Position = 0;
                byte[] tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            }

            using (var response = request.GetResponse())
            {
                Stream stream2 = response.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                return reader2.ReadToEnd();
            }
        }
    }
}
