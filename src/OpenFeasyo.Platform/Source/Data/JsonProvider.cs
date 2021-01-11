/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Lubos Omelina
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
using System;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Microsoft.Win32;

namespace OpenFeasyo.Platform.Data
{
    public class JsonProvider : DataProvider
    {
        private string Username { get; set; }
        private string Password { get; set; }
        private string Credentials { get { return Username + ":" + Password; } }
        private bool AllowAllCertificates { get; set; }

        public JsonProvider(string username = "", string password = "")
        {
            Username = username;
            Password = password;


            object reg =
#if ANDROID || __MACOS__
                1;
#else
                Registry.GetValue(RegistryElements.REGISTRY_ROOT_SECTION, RegistryElements.REGISTRY_ALLOW_ALL_CERTIFICATES, null);
#endif
                if (reg != null)
            {
                AllowAllCertificates = ((Int32)reg) == 1;
            }
         }

        public void SetUser(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public object MakeRequest(string requestUrl, Type t)
        {
            try
            {
                if (callback == null && AllowAllCertificates)
                {
                    // validate cert by calling a function
                    callback = new RemoteCertificateValidationCallback(ValidateRemoteCertificate);
                    ServicePointManager.ServerCertificateValidationCallback += callback;
                }
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.Accept = "application/json";
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(Credentials)));
                request.PreAuthenticate = true;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(t);
                    return jsonSerializer.ReadObject(response.GetResponseStream());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }


        //
        // This is a way how to aprove Selfsigned Certificates (not used anymore):
        //

        // callback used to validate the certificate in an SSL conversation
        private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            bool result = false;
            //if (cert.Subject.Equals("CN=ict4rehab-dev.ulb.ac.be, OU=CODE-WIT, O=ULB, L=Brussels, S=Brussels, C=BE") &&
            //    cert.GetCertHashString().Equals("3D19F570F72D4F7CEE63BB1E9A187DAAD23D738E"))
            {
                result = true;
            }

            return result;
        }

        private static RemoteCertificateValidationCallback callback = null;

        public bool MakePostRequest(string requestUrl, string data, string contentType)
        {
            if (callback == null && AllowAllCertificates)
            {
                // validate cert by calling a function
                callback = new RemoteCertificateValidationCallback(ValidateRemoteCertificate);
                ServicePointManager.ServerCertificateValidationCallback += callback;
            }
            ASCIIEncoding encoding = new ASCIIEncoding();
            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            request.Accept = "application/json";
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(Credentials)));
            request.PreAuthenticate = true;
            byte[] binaryData = encoding.GetBytes(data);

            request.Method = "POST";
            request.ContentType = contentType;
            request.ContentLength = binaryData.Length;

            using (Stream newStream = request.GetRequestStream())
            {
                newStream.Write(binaryData, 0, binaryData.Length);
                newStream.Flush();
            }

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Network error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    return true;
                }
            }
            catch (WebException e)
            {
                return false;
            }
        }

        public int PostObject(string requestUrl, string data, string contentType)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
            httpWebRequest.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(Credentials)));
            httpWebRequest.PreAuthenticate = true;

            httpWebRequest.ContentType = contentType;
            httpWebRequest.Method = "POST";
                 using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                    }
                    return GetLocationNumber(httpResponse.Headers);

                }
            return -1;
        }

        private int GetLocationNumber(WebHeaderCollection collection)
        {
            string[] k = collection.GetValues("Location");
            if (k.Length > 0)
            {
                string line = k[0];
                int pos = line.LastIndexOf('/');
                string num = line.Substring(pos + 1);
                int ret = Int32.Parse(num);
                return ret;
            }
            return -1;
        }


        public bool UploadFile(string requestUrl, string pathToFile)
        {
            try
            {

                var request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = "PUT";
                request.ContentType = "application/octet-stream";
                request.Accept = "*/*";
                //request.UseDefaultCredentials = true;
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(Credentials)));

                //ASCIIEncoding encoding = new ASCIIEncoding();
                //HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                //request.Accept = "application/octet-stream";
                //request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(Credentials)));
                request.PreAuthenticate = true;


                FileInfo fileInfo = new FileInfo(pathToFile);
                long lenth = fileInfo.Length;

                FileStream fs = new FileStream(pathToFile, FileMode.Open, FileAccess.Read);
                BinaryReader reader = new BinaryReader(fs);

                byte[] buffer = new byte[1024];

                request.ContentLength = lenth;

                using (Stream newStream = request.GetRequestStream())
                {
                    int len;
                    while ((len = reader.Read(buffer, 0, 1024)) > 0)
                    {
                        newStream.Write(buffer, 0, len);
                    }


                }

                using (WebResponse response = request.GetResponse())
                {
                    if (response == null) Console.WriteLine("Response is null");
                    using (StreamReader r = new StreamReader(response.GetResponseStream()))
                    {
                        Console.WriteLine(r.ReadToEnd());
                    }
                }

                reader.Close();

            }
            catch (Exception e)
            {

                return false;
            }
            return true;
        }



    }
}
